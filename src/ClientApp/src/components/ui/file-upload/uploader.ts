import axios, { AxiosProgressEvent, AxiosRequestConfig, Canceler } from "axios";
import { FileInfo, FileObj, FileStatus, Queue } from "./types";


class Uploader {
	//events 
	public OnProgress?: (fileName: string, progress: number) => void;
	public OnStatusChange?: (fileName: string, status: FileStatus, message: string) => void;
	public OnCompleted?: () => void;
	public OnStarted?: () => void;

	private uploadInProgress: boolean = false;
	private concurrency: number = 5
	private destroyed: boolean = false;
	private inProgressCount: number = 0;
	private uploadCompleted: boolean = true;

	private inQueue = new Queue<FileObj>;
	private inProgress = new Map<string, FileObj>();
	private uploadUrl: string;

	constructor(url: string) {
		this.uploadUrl = url;
	}

	public Start(files: FileInfo[], concurrency: number) {
		this.concurrency = concurrency;
		let newFiles = files.filter((x: FileInfo) => x.status == FileStatus.Ready).map((f: FileInfo) => {
			let value: FileObj = {
				file: f.file,
				status: FileStatus.Ready,
				name: f.file.name,
				lastProgress: 0
			}
			return value;
		});
		console.log("newFiles.length", newFiles.length);
		if (newFiles.length == 0)
			return;

		if (this.uploadInProgress) {
			this.updateQueue(newFiles)
			return;
		}
		this.uploadInProgress = true;
		this.uploadCompleted = false;

		for (let i = 0; i < newFiles.length; i++) {
			const file = newFiles[i];
			if (i < this.concurrency) {
				file.status = FileStatus.InProgress;
				this.inProgressCount++;
				console.log("inProgressCount", this.inProgressCount);
				this.inProgress.set(file.name, file);
			} else {
				this.inQueue.enqueue(file);
			}
		}
		if (this.OnStarted)
			this.OnStarted();

		for (const [_, fileObj] of this.inProgress) {
			this.uploadFile(fileObj);
		}
	}

	private updateQueue(files: FileObj[]) {
		this.uploadCompleted = false;
		this.destroyed = false;
		for (let i = 0; i < files.length; i++) {
			const file = files[i]!;
			file.status = FileStatus.Queued;
			this.inQueue.enqueue(file);
		}
		this.processQueue();
	}

	private processQueue() {
		if (this.inQueue.size() === 0 && this.inProgressCount == 0 || this.destroyed) {
			if (!this.uploadCompleted) {
				this.uploadCompleted = true;
				this.sendCompletedEvent();
			}
			return;
		}
		if (this.inProgress.size === this.concurrency) {
			//return this.sendUpdateEvent();
			return;
		}
		while (this.inProgressCount < this.concurrency) {
			let file = this.inQueue.dequeue();
			if (file) {
				file.status = FileStatus.InProgress;
				this.inProgress.set(file.name, file);
				this.inProgressCount++;
				console.log("inProgressCount", this.inProgressCount);
				this.uploadFile(file);
			}
			else
				break;
		}
	}

	public IsUploading(): boolean {
		return !this.uploadCompleted;
	}

	public Cancel(fileName: string) {
		let file = this.inProgress.get(fileName);
		if (file) {
			this.inProgress.delete(fileName);
			this.cancelOperation(file);
			this.sendStatusChange(fileName, FileStatus.Cancelled, "Cancelled by user");
		}
	}

	private cancelOperation = (file: FileObj) => {
		if (file.status === FileStatus.InProgress) {
			file.cancel?.();
		}
	};

	public Destroy() {
		this.destroyed = true;
		for (let [, file] of this.inProgress as Map<string, FileObj>) {
			if (file.status === FileStatus.InProgress) {
				this.cancelOperation(file);
				this.inProgress.delete(file.file.name);
			}
		}
		this.inQueue.clear();
	}


	private sendStatusChange(fileName: string, status: FileStatus, message: string) {
		if (this.OnStatusChange) {
			this.OnStatusChange(fileName, status, message);
		}
	}

	private sendCompletedEvent() {
		if (this.OnCompleted) {
			console.log("sendCompletedEvent");
			this.OnCompleted();
		}
	}

	private sendProgressEvent(fileName: string, progress: number) {
		if (this.OnProgress) {
			this.OnProgress(fileName, progress);
		}

	}

	private uploadFile(file: FileObj) {
		try {
			const formData = new FormData();
			formData.append(file.name, file.file);
			const axiosRequestArgs: AxiosRequestConfig = {
				url: this.uploadUrl,
				method: "PUT",
				headers: {
					"Content-Type": "multipart/form-data",
				},
				data: formData,
				onUploadProgress: ({ loaded, total }: AxiosProgressEvent) => {
					const fileName = file.name;
					loaded = isNaN(Number(loaded)) ? 0 : Number(loaded);
					total = isNaN(Number(total)) ? 0 : Number(total);
					console.log()
					const progress = Math.round((loaded / total) * 100);
					//	if (file.lastProgress != progress) {
					this.sendProgressEvent(fileName, progress)
					file.lastProgress = progress;
					//	}

				},
			};
			// if (this._uploadProgress) {
			// 	this.updateProgressEvent({ fileObj, type: "UPLOAD", axiosRequestArgs });
			// }
			axiosRequestArgs.cancelToken = new axios.CancelToken((cancel) => {
				file.cancel = cancel;
			});
			this.sendStatusChange(file.name, file.status, "");
			this.sendProgressEvent(file.name, 0);

			axios(axiosRequestArgs)
				.then(() => {
					if (this.destroyed)
						return;
					this.inProgressCount -= 1;
					console.log("inProgressCount", this.inProgressCount);
					this.inProgress.delete(file.name);
					this.sendStatusChange(file.name, FileStatus.Success, "");
					this.processQueue();
					//this.completedUploads += 1; //.set(fileObj.id, fileObj);
					//this.sendUpdateEvent();
					//this.freeQueue();
				})
				.catch((requestError) => {
					if (this.destroyed)
						return;
					this.inProgressCount -= 1;
					console.log("inProgressCount", this.inProgressCount);
					this.inProgress.delete(file.name);

					let status: FileStatus = FileStatus.Failed
					if (axios.isCancel(requestError))
						status = FileStatus.Cancelled;
					else
						status = FileStatus.Failed;
					this.sendStatusChange(file.name, status, requestError.message);
					this.processQueue();
				});
		}
		catch (e: any) {
			if (this.destroyed)
				return;
			file.status = FileStatus.Failed;
			let message = 'Unknown Error'
			if (e instanceof Error) message = e.message
			if (this.OnStatusChange) {
				this.OnStatusChange(file.name, FileStatus.Failed, message)
			}
		}
	}

}

export default Uploader;