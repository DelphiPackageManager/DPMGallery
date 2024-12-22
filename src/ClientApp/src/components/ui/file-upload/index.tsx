import useConstructor from "@/hooks/useConstructor";
import { useMap, useQueue } from "@uidotdev/usehooks";
import React, { DragEvent, useEffect, useState } from "react";
import { Button } from "../button";
import FileCard from "./file-card";
import { FileInfo, FileObj, fileRegex, FileStatus, PackageInfo } from "./types";
import Uploader from "./uploader";

type FileUploadProps = {

}
type FileUploadState = {
	packageFiles: FileInfo[],
	uploading: boolean

}

export class FileUploader2 extends React.Component<FileUploadProps, FileUploadState> {

	private uploader: Uploader;

	constructor(props: FileUploadProps) {
		super(props);
		this.uploader = new Uploader("/api/v1/package");
		this.uploader.OnStatusChange = this.onStatusChanged;
		this.uploader.OnProgress = this.onProgress;
		this.uploader.OnStarted = this.onStarted;
		this.uploader.OnCompleted = this.onCompleted;

	}

	state: FileUploadState = {
		packageFiles: [],
		uploading: false
	}

	private onStarted = () => {
		this.setState({ ...this.state, uploading: true })
	}

	private onCompleted = () => {
		console.log("completed");
		this.setState({ ...this.state, uploading: false })
	}


	private onStatusChanged = (fileName: string, status: FileStatus, message: string) => {
		//		console.log("onStatusChanged", { filename: fileName, status: status });
		let updatedPackages = [...this.state.packageFiles];

		let item = updatedPackages.find(x => x.file.name == fileName);
		if (item) {
			item.status = status;
			item.message = message;
		}

		this.setState({ ...this.state, packageFiles: updatedPackages })
	}

	private onProgress = (fileName: string, progress: number) => {
		let updatedPackages = [...this.state.packageFiles];

		let item = updatedPackages.find(x => x.file.name == fileName);
		if (item) {
			item.progress = progress;
		}
		this.setState({ ...this.state, packageFiles: updatedPackages })
	}


	private updateFiles = (files: FileList | null) => {
		if (files && files.length > 0) {
			let lFiles: FileInfo[] = [];

			for (let index = 0; index < files.length; index++) {
				const element = files[index];
				let fileInfo: FileInfo = {
					file: element,
					status: FileStatus.Unknown,
					progress: 0
				};
				var parts = fileRegex.exec(fileInfo.file.name);
				//console.log("parts", parts);
				if (parts) {
					let packgeInfo: PackageInfo = {
						packageId: parts[1],
						compilerVersion: parts[2],
						platform: parts[3],
						packageVersion: parts[4],
					}
					fileInfo.packageInfo = packgeInfo;
					fileInfo.status = FileStatus.Ready
				} else {
					fileInfo.status = FileStatus.Invalid
				}
				lFiles.push(fileInfo);
			}
			let newPackageFiles = [...this.state.packageFiles];
			lFiles.forEach(item => {
				newPackageFiles.findIndex(x => x.file.name == item.file.name) == -1 ? newPackageFiles.push(item) : false;
			});
			this.setState({
				packageFiles: newPackageFiles
			})

		}
		else {
			this.setState({
				packageFiles: []
			})
		}
	}


	private handleDragOver = (event: DragEvent<HTMLInputElement>) => {
		event.preventDefault();
		//setDragIsOver(true);
	};

	private handleDragLeave = (event: DragEvent<HTMLInputElement>) => {
		event.preventDefault();
		//setDragIsOver(false);
	};

	private handleDrop = (event: DragEvent<HTMLInputElement>) => {
		event.preventDefault();
		//setDragIsOver(false);
		this.updateFiles(event.dataTransfer.files);
	};



	private handleChangeFile = (e: React.ChangeEvent<HTMLInputElement>) => {
		const targetFiles = e.currentTarget.files
		this.updateFiles(targetFiles);
		e.currentTarget.value = '';
	}

	private onRemoveFile = (fileInfo: FileInfo) => {
		if (fileInfo.status == FileStatus.InProgress) {
			//abort the upload.
			fileInfo.cancel?.();
		}
		var updatedPackageFiles = this.state.packageFiles.filter(x => x.file.name != fileInfo.file.name)
		this.setState({
			packageFiles: updatedPackageFiles
		})
	}

	private onStartUpload = () => {
		console.log("onStartUpload")
		this.uploader?.Start(this.state.packageFiles, 5);
	}

	private onClear = () => {
		this.setState({
			packageFiles: []
		})
		this.uploader.Clear();
	}


	render() {
		console.log("render");

		return (
			<div className="h-full w-full">
				<div className="my-2 flex gap-4">
					<Button disabled={this.state.uploading || this.state.packageFiles.length == 0} onClick={(e) => this.onStartUpload()} >Start Upload</Button>
					<Button disabled={this.state.uploading || this.state.packageFiles.length == 0} onClick={(e) => this.onClear()} >Clear</Button>
				</div>
				<div className="relative min-h-full w-full border-2 border-dashed border-gray-400 p-4 hover:border-blue-400">
					<input
						type="file"
						multiple={true}
						accept=".dpkg"
						onChange={this.handleChangeFile}
						className="absolute left-0 top-0 h-full w-full opacity-0"
						onDragOver={this.handleDragOver}
						onDragLeave={this.handleDragLeave}
						onDrop={this.handleDrop}
						title=""
					/>
					<div className="relative flex flex-wrap gap-2">
						{this.state.packageFiles && this.state.packageFiles.length > 0 && this.state.packageFiles.map((x, i) =>
							<FileCard key={x.file.name} fileInfo={x} onRemoveFile={this.onRemoveFile} />
						)}
						{!this.state.packageFiles || this.state.packageFiles.length == 0 &&
							<p>Drag &amp; Drop files or Click to browse for package files</p>
						}
					</div>
				</div>
			</div>)

	}
}


const FileUploader = () => {
	const [uploader, setUploader] = useState<Uploader>()
	const [packageFiles, setPackageFiles] = useState<FileInfo[]>([]);
	const [dragIsOver, setDragIsOver] = useState(false);

	const { queue: pandingQueue, add: enqueue, remove: dequeue, clear: clearQueue, size: queueLength } = useQueue<FileObj>([])
	const [inProgress] = useMap<FileObj>();

	// private inQueue = new Queue<FileObj>;
	// private inProgress = new Map<string, FileObj>();
	// private uploadUrl: string;

	useEffect(() => {
		let newUploader = new Uploader("/api/v1/package");
		newUploader.OnProgress = onProgress;
		newUploader.OnStatusChange = onStatusChanged;
		setUploader(newUploader);
	}, [])


	// useConstructor(() => {
	// 	let newUploader = new Uploader("/api/v1/package");
	// 	newUploader.OnProgress = onProgress;
	// 	newUploader.OnStatusChange = onStatusChanged;
	// 	setUploader(newUploader);
	// })


	const onStatusChanged = (fileName: string, status: FileStatus, message: string) => {
		alert(packageFiles?.length);
		let updatedPackages = packageFiles.map(item => {
			if (item.file.name == fileName) {
				return { ...item, status: status, message: message };
			}
			return item; // else return unmodified item 
		});
		setPackageFiles(updatedPackages)
	}

	const onProgress = (fileName: string, progress: number) => {
		let updatedPackages = packageFiles.map(item => {
			if (item.file.name == fileName) {
				return { ...item, progress: progress };
			}
			return item; // else return unmodified item 
		});
		setPackageFiles(updatedPackages)
	}



	const updateFiles = (files: FileList | null) => {
		if (files && files.length > 0) {
			let lFiles: FileInfo[] = [];

			for (let index = 0; index < files.length; index++) {
				const element = files[index];
				let fileInfo: FileInfo = {
					file: element,
					status: FileStatus.Unknown,
					progress: 33
				};
				var parts = fileRegex.exec(fileInfo.file.name);
				//console.log("parts", parts);
				if (parts) {
					let packgeInfo: PackageInfo = {
						packageId: parts[1],
						compilerVersion: parts[2],
						platform: parts[3],
						packageVersion: parts[4],
					}
					fileInfo.packageInfo = packgeInfo;
					fileInfo.status = FileStatus.Ready
				} else {
					fileInfo.status = FileStatus.Invalid
				}
				lFiles.push(fileInfo);
			}
			let newPackageFiles = [...packageFiles];
			lFiles.forEach(item => {
				newPackageFiles.findIndex(x => x.file.name == item.file.name) == -1 ? newPackageFiles.push(item) : false;
			});

			setPackageFiles(newPackageFiles);
		}
		else {
			setPackageFiles([]);
		}
	}


	const handleDragOver = (event: DragEvent<HTMLInputElement>) => {
		event.preventDefault();
		setDragIsOver(true);
	};

	const handleDragLeave = (event: DragEvent<HTMLInputElement>) => {
		event.preventDefault();
		setDragIsOver(false);
	};

	const handleDrop = (event: DragEvent<HTMLInputElement>) => {
		event.preventDefault();
		setDragIsOver(false);
		updateFiles(event.dataTransfer.files);
	};



	const handleChangeFile = (e: React.ChangeEvent<HTMLInputElement>) => {
		const targetFiles = e.currentTarget.files
		updateFiles(targetFiles);
	}

	const onRemoveFile = (fileInfo: FileInfo) => {
		if (fileInfo.status == FileStatus.InProgress) {
			//abort the upload.
			fileInfo.cancel?.();
		}
		var updatedPackageFiles = packageFiles.filter(x => x.file.name != fileInfo.file.name)
		setPackageFiles(updatedPackageFiles);
	}
	const onStartUpload = () => {
		console.log("onStartUpload")
		uploader?.Start(packageFiles, 4);
	}

	return (
		<div className="h-fit w-full">
			<div className="my-2">
				<Button disabled={packageFiles.length == 0} onClick={(e) => onStartUpload()} >Start Upload</Button>
			</div>
			<div className="relative h-fit min-h-48 w-full border-2 border-dashed border-gray-400 p-4 hover:border-blue-400">
				<input
					type="file"
					multiple={true}
					accept=".dpkg"
					onChange={handleChangeFile}
					className="absolute left-0 top-0 h-full w-full opacity-0"
					onDragOver={handleDragOver}
					onDragLeave={handleDragLeave}
					onDrop={handleDrop}
					title=""
				/>
				<div className="relative flex flex-wrap gap-2">
					{packageFiles && packageFiles.length > 0 && packageFiles.map((x, i) =>
						<FileCard key={i} fileInfo={x} onRemoveFile={onRemoveFile} />
					)}
					{!packageFiles || packageFiles.length == 0 &&
						<p>Drag &amp; Drop files or Click to browse for package files</p>
					}
				</div>
			</div>
		</div>
	)
}

export default FileUploader;