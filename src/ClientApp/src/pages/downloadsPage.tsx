import { SITE_URL } from "@/constants";
import * as React from "react";
import Meta from "../components/meta";
import PageContainer from "../components/pageContainer";

interface IDownloadsPageProps {}

const DownloadsPage: React.FunctionComponent<IDownloadsPageProps> = (props) => {
  return (
    <PageContainer>
      <Meta title="DPM - Downloads" canonical={`${SITE_URL}/downloads`} description="Download the DPM Package Manager client" />
      <h1>Downloads</h1>
      <p className="mt-4">
        Download the latest DPM installer for Delphi XE2 - 11.x from{" "}
        <a href="https://github.com/DelphiPackageManager/DPM/releases" title="Download DPM" className="text-sky-800 dark:text-sky-600">
          GitHub
        </a>
      </p>
      <p className="mt-4">
        Note that the IDE plugins were compiled with the latest patch version available for each compiler version. Whilst in theory they should be
        compatible with all major release and patch versions, we have only tested with the latest versions.
      </p>
    </PageContainer>
  );
};

export default DownloadsPage;
