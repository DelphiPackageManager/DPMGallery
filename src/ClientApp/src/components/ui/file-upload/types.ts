import { Canceler } from "axios";

export enum FileStatus {
	Unknown = 1,
	Ready = 2,
	Invalid = 3,
	InProgress = 4,
	Queued = 5,
	Cancelled = 6,
	Failed = 7,
	Success = 8
}

export type PackageInfo = {
	packageId: string;
	compilerVersion: string;
	platform: string;
	packageVersion: string;
}


export type FileInfo = {
	file: File;
	status: FileStatus;
	progress: number;
	packageInfo?: PackageInfo;
	message?: string;
	cancel?: Canceler;
}

export type FileObj = {
	file: File;
	name: string;
	status: FileStatus;
	cancel?: Canceler;
	lastProgress: number;
}



const fileRegex = /^((?:\w+)(?:\.\w+)+)\-([^\-]+)\-([^\-]+)\-([^\-]+)\.dpkg$/

const checkFileName = (fileName: string): boolean => fileRegex.test(fileName);

class Queue<T> {
	private storage: T[] = [];

	constructor(private capacity: number = Infinity) { }

	enqueue(item: T): void {
		if (this.size() === this.capacity) {
			throw Error("Queue has reached max capacity, you cannot add more items");
		}
		this.storage.push(item);
	}
	dequeue(): T | undefined {
		return this.storage.shift();
	}
	size(): number {
		return this.storage.length;
	}

	remove(predicate: (value: T) => boolean, thisArg?: any) {
		let idx = -1;
		for (let index = 0; index < this.storage.length; index++) {
			const element = this.storage[index];
			if (predicate(element)) {
				idx = index;
				break;
			}

		}
		if (idx > -1) {
			this.storage.splice(idx, 1);
		}
	}

	clear() {
		this.storage = [];
	}

}


export { checkFileName, fileRegex, Queue };

