import { DragEvent, useState } from "react";
import { Button } from "../button";
import FileCard from "./file-card";
import { FileInfo, fileRegex, FileStatus, PackageInfo } from "./types";


const FileUploader = () => {
	const [packageFiles, setPackageFiles] = useState<FileInfo[]>([]);
	const [dragIsOver, setDragIsOver] = useState(false);

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
		if (fileInfo.status == FileStatus.Uploading) {
			//abort the upload.
		}
		var updatedPackageFiles = packageFiles.filter(x => x.file.name != fileInfo.file.name)
		setPackageFiles(updatedPackageFiles);
	}

	return (
		<div className="h-fit w-full">
			<div className="my-2">
				<Button disabled={packageFiles.length == 0}>Start Upload</Button>
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