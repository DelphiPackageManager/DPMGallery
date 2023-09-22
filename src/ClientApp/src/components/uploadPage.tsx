import { SITE_URL } from "@/constants";
import * as React from "react";
import Meta from "./meta";
import PageContainer from "./pageContainer";

interface IUploadPageProps {}

const UploadPage: React.FunctionComponent<IUploadPageProps> = (props) => {
  return (
    <PageContainer>
      <Meta title="DPM - Upload Packages" canonical={`${SITE_URL}/upload`} />
      <h1>Uploads</h1>
    </PageContainer>
  );
};

export default UploadPage;
