import { GlobeAltIcon } from "@heroicons/react/24/outline";
import { StopwatchIcon } from "@radix-ui/react-icons";
import { NavLink } from "react-router-dom";
import { PackageDetailsModel, PackageVersionModel } from "./packageTypes";

export type PackageSideBarProps = {
  packageDetails: PackageDetailsModel | null;
  currentVersion: PackageVersionModel | null;
};

const PackageSideBar = ({ packageDetails, currentVersion }: PackageSideBarProps) => {
  const PackageOwners = () => {
    if (!packageDetails) return <></>;

    return (
      <div className="flex flex-col mb-2">
        {packageDetails.owners.map((owner, index) => {
          return (
            <div key={index} className="flex flex-row mt-2 ">
              <div className="w-10 h-10 mr-2">
                <img src={`https://www.gravatar.com/avatar/${owner.emailHash}`} className="rounded-lg object-cover" />
              </div>
              <div className="py-2">
                <NavLink to={`/profiles/${owner.userName}`} className="hover:underline">
                  {owner.userName}
                </NavLink>
              </div>
            </div>
          );
        })}
      </div>
    );
  };

  return (
    <div className="md:min-w-[16rem] md:mb-2 md:mt-6 px-2">
      <div>
        <h2 className="text-base font-medium">Owners</h2>
      </div>
      <PackageOwners />
      <div>
        <h2 className="mt-2 text-base font-medium">Downloads</h2>
      </div>
      <div className="">
        <span className="text-sm">Total : </span>
        <span className="text-xs">{packageDetails?.totalDownloads}</span>
      </div>
      <div className="">
        <span className="text-sm">This version : </span>
        <span className="text-xs">{currentVersion?.downloads}</span>
      </div>
      <div>
        <h2 className="mt-4 text-base font-medium">About</h2>
      </div>
      <div className="mt-2 flex items-center">
        <StopwatchIcon className="mr-2 h-4 w-4" />
        <span className="text-sm">Last Updated {currentVersion?.published}</span>
      </div>
      {packageDetails?.projectUrl && (
        <div className="mt-2 flex items-center">
          <GlobeAltIcon className="mr-2 h-4 w-4" />
          <span className="text-sm">
            <a href={packageDetails?.projectUrl} className="hover:underline">
              Project Website
            </a>
          </span>
        </div>
      )}
      {packageDetails?.repositoryUrl && (
        <div className="mt-2 flex items-center">
          <GlobeAltIcon className="mr-2 h-4 w-4" />
          <span className="text-sm">
            <a href={packageDetails?.repositoryUrl} className="hover:underline">
              Source Respository
            </a>
          </span>
        </div>
      )}
    </div>
  );
};

export default PackageSideBar;
