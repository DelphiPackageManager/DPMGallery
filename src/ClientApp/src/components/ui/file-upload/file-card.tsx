import DelphiIcon from "@/components/DelphiIcon";
import { cn } from "@/lib/utils";
import { CompilerVersion, Platform, StrToCompilerVersion, StrToPlatform } from "@/types/types";
import { formatBytes } from "@/utils";
import { ArrowUpFromLine, X } from "lucide-react";
import { ReactNode } from "react";
import CircleProgress from "../circle-progress/circleProgress";
import PlatformIcon from "../PlatformIcon";
import { FileInfo, FileStatus } from "./types";

type FileCardProps = {
	fileInfo: FileInfo;
	onRemoveFile: (file: FileInfo) => void;
	onStart?: (file: FileInfo) => void;
	children?: ReactNode;
}

type CancelButtonProps = {
	fileInfo: FileInfo;
	onCancel: (file: FileInfo) => void;
}

const CancelButton = ({ fileInfo, onCancel }: CancelButtonProps) => {

	return (
		<div className="flex aspect-square h-5 cursor-pointer items-center justify-center self-start rounded-full bg-white text-center ring-2 ring-gray-400 transition-all duration-300 hover:bg-destructive hover:text-white hover:ring-destructive dark:bg-gray-900 dark:hover:bg-destructive">
			<button title="Remove" type="button" onClick={(e) => onCancel(fileInfo)} >
				<X size={16} />
			</button>
		</div>
	)
}

type StatusButtonProps = {
	status: FileStatus;
	onClick?: () => void;

}
const StatusButton = ({ status }: StatusButtonProps) => {


	return (
		<div className="flex h-7 w-7 cursor-pointer items-center justify-center rounded-full bg-white text-center ring-2 ring-gray-400 transition-all duration-300 hover:ring-2 hover:ring-primary dark:bg-gray-900">
			<ArrowUpFromLine size={16} className="" />
		</div>
	)
}



const FileCard = ({ fileInfo, onRemoveFile }: FileCardProps) => {

	// export enum FileStatus {
	// 	Unknown = 1,
	// 	Ready = 2,
	// 	Invalid = 3,
	// 	Uploading = 4,
	// 	Cancelled = 5,
	// 	Failed = 6,
	// 	Completed = 7
	// }


	function statusClassNames(status: FileStatus) {
		//console.log(status)
		switch (status) {
			case FileStatus.Invalid:
				return "border-2 border-destructive dark:border-destructive";
			case FileStatus.Unknown:
				return "";
			case FileStatus.Ready:
				return "";

			case FileStatus.Failed:
				return "bg-red-500";

			case FileStatus.Cancelled:
				return "bg-orange-500";

			case FileStatus.InProgress:
				return "bg-blue-500";
			case FileStatus.Queued:
				return "bg-yellow-500";


			case FileStatus.Success:
				return "bg-green-400";
			default:
				return "bg-red-500"

		}

	}

	let compilerVerison: CompilerVersion = CompilerVersion.UnknownVersion;
	let platform: Platform = Platform.UnknownPlatform;
	if (fileInfo.packageInfo) {
		compilerVerison = StrToCompilerVersion(fileInfo.packageInfo.compilerVersion);
		platform = StrToPlatform(fileInfo.packageInfo.platform);
	}

	let className = "";// statusClassNames(fileInfo.status);

	let byteCount = formatBytes(fileInfo.file.size);

	return (
		<div className={cn("dark:border-gray-500 relative h-20 min-w-80 rounded-xl border border-gray-400 bg-gray-200 p-1 text-gray-900 dark:bg-gray-700 dark:text-gray-50", className)}>
			<div className="flex flex-col gap-1 rounded-xl p-2 text-base" >
				<div className="mb-1 flex items-center gap-2">
					<CancelButton fileInfo={fileInfo} onCancel={onRemoveFile} />
					{fileInfo.packageInfo &&
						<>
							<div className="flex grow items-center gap-2 justify-self-start">
								<DelphiIcon compilerVersion={compilerVerison} />
								<PlatformIcon className="h-6 w-6" platform={platform} />
								<span>{fileInfo.packageInfo.packageId}</span>
								<span>v{fileInfo.packageInfo.packageVersion}</span>
							</div>
							<div>
								<CircleProgress size={24} value={fileInfo.progress} />
							</div>
						</>
					}
					{!fileInfo.packageInfo &&
						<span>{fileInfo.file.name}</span>
					}
				</div>
				<div className="flex flex-row items-center gap-2">
					<span className="text-xs">{byteCount}</span>
					<div className="grow">
						{fileInfo.message}
					</div>
				</div>
				<div>{fileInfo.message}</div>
			</div>


		</div>
	)
}

export default FileCard;