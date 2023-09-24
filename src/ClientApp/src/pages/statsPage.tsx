import { SITE_URL } from "@/constants";
import * as React from "react";
import { useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import Loader from "../components/loader";
import Meta from "../components/meta";
import PageContainer from "../components/pageContainer";
import { PackageDownloads, Statistics, VersionDownloads } from "../components/statistics/statisticsTypes";
import useStatistics from "../hooks/useStatistics";
import { timeAgo } from "../utils";

interface IStatsPageProps {}

const StatsPage: React.FunctionComponent<IStatsPageProps> = (props) => {
  const controller = new AbortController();

  const [{ loading, error, statistics }, getStatistics] = useStatistics();
  let lastUpdated: string = "";
  if (statistics) {
    var date = new Date(statistics.lastUpdated);
    lastUpdated = timeAgo(date);
  }

  useEffect(() => {
    //dealing with useffect running twice in dev mode due to strict mode
    getStatistics(controller);
  }, []);

  return (
    <PageContainer>
      <Meta title="DPM - Statistics" canonical={`${SITE_URL}/statistics`} />

      <h1>Statistics</h1>
      {!error && loading && <Loader />}
      {!error && !loading && statistics && (
        <React.Fragment>
          <h4 className="mt-4">Statistics Last updated : {lastUpdated}</h4>
          <div className="mt-4 px-2">
            <div className="">
              <span>Unique Packages : </span>
              <span>{statistics.uniquePackages}</span>
            </div>
            <div className="">
              <span>Package Versions : </span>
              <span>{statistics.packageVersions}</span>
            </div>
          </div>

          <div className="px-2 mt-2 flex flex-col md:flex-row gap-4 md:gap-8 w-full">
            <div className="mt-2 text-left basis-full">
              <h2>Package downloads</h2>
              <div className="flex flex-row w-full justify-between border-b-2 border-gray-500 dark:border-gray-700 my-4">
                <div className="">
                  <span>Name</span>
                </div>
                <div className="">
                  <span>Downloads</span>
                </div>
              </div>

              {statistics.topPackageDownloads.map((pkg: PackageDownloads, index: number) => (
                <div key={index} className="flex flex-row w-full justify-between">
                  <div className=" w-1/2">
                    <Link to={"/packages/" + pkg.packageId + "/"} className=" hover:underline">
                      {pkg.packageId}
                    </Link>
                  </div>
                  <div className="">{pkg.downloads}</div>
                </div>
              ))}
            </div>
            <div className="mt-2 text-left basis-full">
              <h2>Package Version downloads</h2>
              <div className="flex flex-row w-full justify-between border-b-2 border-gray-500 dark:border-gray-700 my-4">
                <div className=" w-1/2 ">
                  <span>Name</span>
                </div>
                <div className="text-left w-1/4">
                  <span>Version</span>
                </div>
                <div className="text-right w-1/4 ">
                  <span>Downloads</span>
                </div>
              </div>

              {statistics.topVersionDownloads.map((pkg: VersionDownloads, index: number) => (
                <div key={index} className="flex flex-row w-full justify-between">
                  <div className=" w-1/2">
                    <Link to={"/packages/" + pkg.packageId + "/" + pkg.version + "/"} className="hover:underline">
                      {pkg.packageId}
                    </Link>
                  </div>
                  <div className="text-left w-1/4">{pkg.version}</div>
                  <div className="text-right w-1/4 ">{pkg.downloads}</div>
                </div>
              ))}
            </div>
          </div>
        </React.Fragment>
      )}
    </PageContainer>
  );
};

export default StatsPage;
