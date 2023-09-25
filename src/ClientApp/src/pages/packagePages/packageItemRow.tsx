import { ArrowDownTrayIcon, AtSymbolIcon, ClockIcon, TagIcon } from "@heroicons/react/24/outline";
import React from "react";
import { Link } from "react-router-dom";
import { PackageResultItem } from "./packageTypes";

type PackageItemRowProps = {
  pkg: PackageResultItem;
  index: number;
  onTagClick?: (value: string) => void;
};

const PackageItemRow: React.FC<PackageItemRowProps> = ({ pkg, index, onTagClick }) => {
  const doOnTagClick = (value: string) => {
    if (onTagClick) {
      onTagClick(value);
    }
  };

  return (
    <div key={index} className="flex flex-row flex-nowrap mb-1 py-2 align-top text-sm md:text-lg hover:bg-primary-50 dark:hover:bg-gray-700 w-full">
      <div className="md:flex items-start px-2 mr-1 md:px-3 min-w-fit">
        <img
          alt="package icon"
          className="w-8 h-8 md:w-12 md:h-12"
          src={
            pkg.icon
              ? `/api/v1/package/${pkg.packageId}/${pkg.compilerVersions[0]}/${pkg.platforms[0]}/${pkg.latestVersion}/icon`
              : "/img/dpm64light.png"
          }
        />
      </div>

      <div className="grow text-left text-base text-gray-900 dark:text-gray-100">
        <div className="flex flex-col">
          <div className="flex flex-row w-full items-baseline mb-1">
            <Link
              to={`/packages/${pkg.packageId}/${pkg.latestVersion}/`}
              className="text-base md:text-lg text-sky-600 dark:text-sky-400  hover:underline">
              {pkg.packageId}
            </Link>
            <div className="ml-3 text-sm md:text-base text-gray-400">
              <span>by :</span>
              {pkg.owners.map((owner, index) => {
                return (
                  <Link className="ml-1 text-sky-600 dark:text-sky-600 hover:underline" key={index} to={`/profiles/${owner}`}>
                    {owner}
                  </Link>
                );
              })}
            </div>
          </div>
          <div className="flex flex-row flex-wrap text-gray-400 dark:text-gray-500 mb-2 items-start text-sm">
            <div className="flex flex-row flex-nowrap items-center">
              <AtSymbolIcon className="w-4 h4 mr-1" />
              <span className="mr-1">
                latest: {pkg.latestVersion} {pkg.isPrelease ? "(prerelease)" : ""}{" "}
              </span>
            </div>
            <div className="flex flex-row flex-nowrap items-center">
              <ClockIcon className="w-4 h4 mr-1" />
              <span className="tooltip mr-1" data-tooltip={pkg.publishedUtc}>
                {" "}
                last updated {pkg.published}
              </span>
            </div>
            <div className="flex flex-row flex-nowrap items-center">
              <ArrowDownTrayIcon className="w-4 h4 mr-1" />
              <span className="mr-2">
                {pkg.totalDownloads} total {pkg.totalDownloads == 1 ? "download" : "downloads"}
              </span>
            </div>
            <div className="flex flex-row items-center">
              {pkg.tags && <TagIcon className="w-4 h4 mr-1" />}
              {pkg.tags?.map((tag, index) => {
                return (
                  <div className="mr-1 text-sky-700 dark:text-sky-700 hover:underline cursor-pointer" key={index} onClick={() => doOnTagClick(tag)}>
                    {tag}
                  </div>
                );
              })}
            </div>
          </div>
          <div className="text-sm md:text-base">
            <span className="word-break">{pkg.description}</span>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PackageItemRow;
