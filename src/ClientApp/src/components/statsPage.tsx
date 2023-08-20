import * as React from "react";
import { useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import useStatistics from "../hooks/useStatistics";
import { timeAgo } from "../utils";
import Loader from "./loader";
import PageContainer from "./pageContainer";
import { PackageDownloads, Statistics, VersionDownloads } from "./statistics/statisticsTypes";

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
      <h1>Statistics</h1>
      {!error && loading && <Loader />}
      {!error && !loading && statistics && (
        <React.Fragment>
          <h4 className="mt-4">Statistics Last updated : {lastUpdated}</h4>
          <div className="mt-4 ">
            <div className="">
              <span>Unique Packages : </span>
              <span>{statistics.uniquePackages}</span>
            </div>
            <div className="">
              <span>Package Versions : </span>
              <span>{statistics.packageVersions}</span>
            </div>
          </div>

          <div className="mt4 flex flex-row  flex-1 gap-8 w-full">
            <div className="mt-2 text-left w-1/2">
              <h2>Package downloads</h2>
              <div className="flex flex-row w-full justify-between border-b-2 border-gray-500 dark:border-gray-700 my-4">
                <div className=" w-1/2">
                  <span>Name</span>
                </div>
                <div className="">
                  <span>Downloads</span>
                </div>
              </div>

              {statistics.topPackageDownloads.map((pkg: PackageDownloads, index: number) => (
                <div className="flex flex-row w-full justify-between">
                  <div key={index} className=" w-1/2">
                    <Link to={"/packages/" + pkg.packageId + "/"}>{pkg.packageId}</Link>
                  </div>
                  <div key={index} className="">
                    {pkg.downloads}
                  </div>
                </div>
              ))}
            </div>
            <div className="mt-2 text-left w-1/2">
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
                <div className="flex flex-row w-full justify-between">
                  <div key={index} className=" w-1/2">
                    <Link to={"/packages/" + pkg.packageId + "/" + pkg.version + "/"}>{pkg.packageId}</Link>
                  </div>
                  <div key={index} className="text-left w-1/4">
                    {pkg.version}
                  </div>
                  <div key={index} className="text-right w-1/4 ">
                    {pkg.downloads}
                  </div>
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
