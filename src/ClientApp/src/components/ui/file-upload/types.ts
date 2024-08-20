
export enum FileStatus {
	Unknown = 1,
	Ready = 2,
	Invalid = 3,
	Uploading = 4,
	Cancelled = 5,
	Failed = 6,
	Completed = 7
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
}




const fileRegex = /^((?:\w+)(?:\.\w+)+)\-([^\-]+)\-([^\-]+)\-([^\-]+)\.dpkg$/

const checkFileName = (fileName: string): boolean => fileRegex.test(fileName);

export { checkFileName, fileRegex };

