//TODO : This page is a bit of a mess - need to clean up loading and state management.

import { ArrowDownTrayIcon, AtSymbolIcon, InformationCircleIcon, TagIcon } from "@heroicons/react/24/outline";
import { AxiosError, isAxiosError } from "axios";

import { useEffect, useState } from "react";
import { Link, NavLink, useParams } from "react-router-dom";

import PageContainer from "../../components/pageContainer";

import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";

import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";

import axios from "@/api/axios";
import { SITE_URL } from "@/constants";
import { firstOrNull } from "@/utils";
import { Crosshair2Icon, InfoCircledIcon, ReaderIcon, StopwatchIcon } from "@radix-ui/react-icons";
import Meta from "../../components/meta";
import { Tooltip, TooltipContent, TooltipTrigger } from "../../components/ui/tooltip";
import PackageSideBar from "./packageSideBar";
import { PackageDetailsModel, PackageVersionModel } from "./packageTypes";

//TODO : find a cleaner way to do this.
// import DelphiXE2 from '../../assets/Delphi-XE2.svg';
// import DelphiXE3 from '../../assets/Delphi-XE3.svg';
// import DelphiXE4 from '../../assets/Delphi-XE4.svg';
// import DelphiXE5 from '../../assets/Delphi-XE5.svg';
// import DelphiXE6 from '../../assets/Delphi-XE6.svg';
// import DelphiXE7 from '../../assets/Delphi-XE7.svg';
// import DelphiXE8 from '../../assets/Delphi-XE8.svg';
// import Delphi10 from '../../assets/Delphi-10.0.svg';
// import Delphi10_1 from '../../assets/Delphi-10.1.svg';
// import Delphi10_2 from '../../assets/Delphi-10.2.svg';
// import Delphi10_3 from '../../assets/Delphi-10.3.svg';
// import Delphi10_4 from '../../assets/Delphi-10.4.svg';
// import Delphi11 from '../../assets/Delphi-11.x.svg';
// import Delphi12 from '../../assets/Delphi-12.x.svg';

const PackagePage = () => {
  let { packageId, packageVersion } = useParams();
  const [reactContent, setMarkdownSource] = useState("");
  const [error, setError] = useState("");
  const [packageDetails, setPackageDetails] = useState<PackageDetailsModel | null>(null);
  const [currentVersion, setCurrentVersion] = useState<PackageVersionModel | null>(null);
  const [latestVersion, setLatestVersion] = useState<PackageVersionModel | null>(null);
  const [currentTab, setCurrentTab] = useState("readme");
  const controller = new AbortController();

  let owner = "vincent"; //test
  let totalDownloads = "1,234,343";

  function GetThisVersion(versions: PackageVersionModel[], currentVersion: string): PackageVersionModel | null {
    let result = versions.find(function (item) {
      return item.version === currentVersion;
    });
    return result || null;
  }

  const fetchPackageDetails = async () => {
    try {
      let url;
      if (packageVersion) {
        url = `/ui/packagedetails/${packageId}/${packageVersion}/`;
      } else {
        url = `/ui/packagedetails/${packageId}`; ///get the latest version
      }
      const response = await axios.get<PackageDetailsModel>(url, {
        signal: controller.signal,
        baseURL: "/",
      });
      // await sleep(500); //so we can see loading
      setPackageDetails(response.data);
      let model = response.data;
      if (packageVersion) {
        let ver = GetThisVersion(model?.versions, packageVersion);
        setCurrentVersion(ver);
        ver = firstOrNull(model?.versions);
        setLatestVersion(ver);
      } else {
        let ver = firstOrNull(model?.versions);
        setLatestVersion(ver);
        setCurrentVersion(ver);
        if (ver) {
          packageId = ver.version;
        }
      }
      if (model?.description) {
        setMarkdownSource(response.data.description);
      }
      if (model?.readMe) {
        try {
          //withcredentials false is important.
          const readMeResponse = await axios.get<string>(model.readMe, {
            signal: controller.signal,
            baseURL: "/",
            headers: { "Content-Type": "text/plain" },
            withCredentials: false,
          });
          if (readMeResponse.data) {
            setMarkdownSource(readMeResponse.data);
          }
        } catch (err: any) {
          if (isAxiosError(err) && err.response) {
            let axiosErr = err as AxiosError;
            //lots of older packages do not actually have the readme even though the say they do.
            if (axiosErr.status !== 404) throw err;
          }
        }
      }
    } catch (err: any) {
      let message;
      if (isAxiosError(err) && err.response) {
        let axiosErr = err as AxiosError;
        message = axiosErr.message;
      } else message = String(error);
      setError(message);
    } finally {
      //setLoading(false);
    }
  };

  useEffect(() => {
    const fetchData = async () => {
      await fetchPackageDetails();
    };
    setCurrentTab("readme");
    fetchData();
  }, [packageId, packageVersion]);

  type PackageVersionsProps = {
    versions: PackageVersionModel[];
    currentVersion: string | null;
  };

  const VersionRows = ({ versions, currentVersion }: PackageVersionsProps) => {
    return (
      <table className="w-full">
        <thead className="">
          <tr className="w-full mb-2 border-b border-gray-600">
            <th className="text-left font-medium sm:w-48 md:w-72">Version</th>
            <th className="text-left font-medium">Downloads</th>
            <th className="text-right font-medium">Published</th>
            <th className="w-12"></th>
          </tr>
        </thead>
        <tbody>
          {versions.map((version, index) => {
            if (!version._published) {
              let date = new Date(version.publishedUtc);
              version._published = date.toDateString();
            }

            return (
              <tr className={`${version.version == currentVersion ? "bg-gray-100 dark:bg-gray-700" : "font-normal"}`} key={index}>
                <td className={`${version.version == currentVersion ? "" : ""}`}>
                  <Link className="text-sky-600 dark:text-sky-400 hover:underline" to={`/packages/${packageId}/${version.version}/`}>
                    {version.version}
                  </Link>
                </td>
                <td className="text-left">{version.downloads}</td>
                <td className="text-right">
                  <Tooltip>
                    <TooltipTrigger>{version.published}</TooltipTrigger>
                    <TooltipContent>
                      <p>{version._published}</p>
                    </TooltipContent>
                  </Tooltip>
                </td>
                <td className="w-12"></td>
              </tr>
            );
          })}
        </tbody>
      </table>
    );
  };

  return (
    <PageContainer>
      <Meta title={`DPM - ${packageId} ${packageVersion}`} canonical={`${SITE_URL}/packages`} />

      {error && <span className="text-red">{error}</span>}
      <div className="flex flex-col-reverse md:flex-row justify-between w-full text-gray-600 dark:text-gray-300">
        <PackageSideBar currentVersion={currentVersion} packageDetails={packageDetails} />
        <div className="grow">
          <div className="flex flex-row mt-4 text-gray-700 dark:text-gray-100 items-baseline">
            <h1 className="text-2xl mr-3">{packageId}</h1>
            <span className="text-lg ">{packageVersion}</span>
          </div>
          <div className="flex flex-row items-start space-x-2 mb-2">
            {/* <img src={DelphiXE2} className="w-6 h-6"  title='Delphi XE2' />
                        <img src={DelphiXE3} className="w-6 h-6"  title='Delphi XE3' />
                        <img src={DelphiXE4} className="w-6 h-6"  title='Delphi XE4' />
                        <img src={DelphiXE5} className="w-6 h-6"  title='Delphi XE5' />
                        <img src={DelphiXE6} className="w-6 h-6"  title='Delphi XE6' />
                        <img src={DelphiXE7} className="w-6 h-6"  title='Delphi XE7' />
                        <img src={DelphiXE8} className="w-6 h-6"  title='Delphi XE8' />
                        <img src={Delphi10} className="w-6 h-6"  title='Delphi 10.0' />
                        <img src={Delphi10_1} className="w-6 h-6"  title='Delphi 10.1' />
                        <img src={Delphi10_2} className="w-6 h-6"  title='Delphi 10.2' />
                        <img src={Delphi10_3} className="w-6 h-6"  title='Delphi 10.3' />
                        <img src={Delphi10_4} className="w-6 h-6"  title='Delphi 10.4' />
                        <img src={Delphi11} className="w-6 h-6"  title='Delphi 11.x' />
                        <img src={Delphi12} className="w-6 h-6"  title='Delphi 12.x' />
    */}
          </div>
          {packageDetails && packageDetails.isPrerelease && (
            <div
              className="flex items-center mt-2 bg-primary/80 dark:bg-primary/60 text-gray-100 dark:text-gray-100 text-sm px-4 py-2 rounded-md "
              role="alert">
              <InfoCircledIcon className="w-6 h-6 mr-2" />
              <p>This is a prerelease version of {packageId}</p>
            </div>
          )}
          {latestVersion && latestVersion.version !== packageVersion && (
            <div
              className="flex items-center mt-2 bg-primary/80 dark:bg-primary/60 text-gray-100 dark:text-gray-100 text-sm px-4 py-2 rounded-md"
              role="alert">
              <InfoCircledIcon className="w-6 h-6 mr-2" />
              <p>There is a newer version of this package available.</p>
            </div>
          )}

          {packageDetails && packageDetails.prefixReserved && (
            <div className="flex items-center py-2 mt-2">
              <Tooltip>
                <TooltipTrigger>
                  <div className="flex items-center justify-center bg-primary rounded-full h-5 w-5">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="currentColor" className="text-white rounded-full w-4 h-4">
                      <path d="M13.78 4.22a.75.75 0 0 1 0 1.06l-7.25 7.25a.75.75 0 0 1-1.06 0L2.22 9.28a.751.751 0 0 1 .018-1.042.751.751 0 0 1 1.042-.018L6 10.94l6.72-6.72a.75.75 0 0 1 1.06 0Z"></path>
                    </svg>
                  </div>
                </TooltipTrigger>
                <TooltipContent>
                  <p>The ID prefix of this package has been reserved for one of the owners of this package by DPM</p>
                </TooltipContent>
              </Tooltip>
              <span className="ml-2 text-sm">Prefix Reserved</span>
            </div>
          )}

          <div className="my-2">
            <Tabs value={currentTab} onValueChange={(e) => setCurrentTab(e)} className="">
              <TabsList className="w-full border-b border-primary text-base bg-white dark:bg-gray-800 items-start">
                <TabsTrigger value="readme">
                  <span className="flex">
                    <ReaderIcon className="mr-2 h-4 w-4" />
                    Readme
                  </span>
                </TabsTrigger>
                <TabsTrigger value="compilers">
                  <span className="flex">
                    <Crosshair2Icon className="mr-2 h-4 w-4" />
                    Compilers/Platforms
                  </span>
                </TabsTrigger>
                <TabsTrigger value="versions">
                  <span className="flex">
                    <StopwatchIcon className="mr-2 h-4 w-4" />
                    Versions
                  </span>
                </TabsTrigger>
              </TabsList>
              <TabsContent value="readme">
                <div className="remark">
                  <ReactMarkdown remarkPlugins={[remarkGfm]}>{reactContent}</ReactMarkdown>
                </div>
              </TabsContent>
              <TabsContent value="compilers">
                <p>compilers</p>
              </TabsContent>
              <TabsContent value="versions">
                <div className="flex flex-row">
                  {packageDetails && packageDetails.versions && (
                    <VersionRows versions={packageDetails.versions} currentVersion={currentVersion?.version || null} />
                  )}
                </div>
              </TabsContent>
            </Tabs>
          </div>
        </div>
      </div>
    </PageContainer>
  );
};

export default PackagePage;
