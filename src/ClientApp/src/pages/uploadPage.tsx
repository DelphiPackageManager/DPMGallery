import { SITE_URL } from "@/types/constants";
import * as React from "react";
import Meta from "../components/meta";
import PageContainer from "../components/pageContainer";

interface IUploadPageProps { }

const UploadPage: React.FunctionComponent<IUploadPageProps> = (props) => {
	return (
		<PageContainer>
			<Meta title="DPM - Upload Packages" canonical={`${SITE_URL}/upload`} />
			<h1>Uploads</h1>
		</PageContainer>
	);
};

export default UploadPage;
