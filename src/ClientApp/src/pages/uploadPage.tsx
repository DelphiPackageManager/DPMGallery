import { Button } from "@/components/ui/button";
import FileUploader, { FileUploader2 } from "@/components/ui/file-upload";
import { Input } from "@/components/ui/input";
import { SITE_URL } from "@/types/constants";
import { DragEvent, useState } from "react";
import PageContainer from "../components/pageContainer";

interface IUploadPageProps { }



const UploadPage: React.FunctionComponent<IUploadPageProps> = (props) => {

	return (
		<PageContainer>
			<h1 className="mb-2 text-xl">Uploads</h1>
			<FileUploader2 />
		</PageContainer>
	);
};

export default UploadPage;
