import RepositoryLogo from '@/assets/git.svg?react';
import { FlagIcon, GlobeAltIcon, ScaleIcon } from "@heroicons/react/24/outline";

import { StopwatchIcon } from "@radix-ui/react-icons";
import { ReactNode } from "react";
import { Link, NavLink } from "react-router-dom";
import { PackageDetailsModel, PackageVersionModel } from "./packageTypes";

export type PackageSideBarProps = {
	packageDetails: PackageDetailsModel | null;
	currentVersion: PackageVersionModel | null;
};

const PackageSideBar = ({ packageDetails, currentVersion }: PackageSideBarProps) => {
	const PackageOwners = () => {
		if (!packageDetails) return <></>;

		return (
			<div className="mb-2 flex flex-col">
				{packageDetails.owners.map((owner, index) => {
					return (
						<div key={index} className="mt-2 flex flex-row">
							<div className="mr-2 h-10 w-10">
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
		<div className="px-2 md:mb-2 md:mt-6 md:min-w-[16rem]">
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
						<a href={packageDetails?.projectUrl} target="_blank" className="hover:underline">
							Project Website
						</a>
					</span>
				</div>
			)}
			{packageDetails?.repositoryUrl && (
				<div className="mt-2 flex items-center">
					<RepositoryLogo className="mr-2 h-4 w-4" />
					<span className="text-sm">
						<a href={packageDetails?.repositoryUrl} target="_blank" className="hover:underline">
							Source Repository
						</a>
					</span>
				</div>
			)}
			{packageDetails?.licenses &&
				<div className="mt-2 flex items-center">
					{packageDetails?.licenses?.map((value: string, index: number): ReactNode => {
						return (
							<div key={index} className="flex items-center">
								<ScaleIcon className="mr-2 h-4 w-4" />
								<span className="text-sm">
									<a href={`https://spdx.org/licenses/${value}.html`} target="_blank" className="hover:underline">
										{value} License
									</a>
								</span>
							</div>
						)
					})}
				</div>

			}
			<div className="mt-2 flex items-center">
				<FlagIcon className="mr-2 h-4 w-4" />
				<span className="text-sm">
					<Link to={`/packages/${packageDetails?.packageId}/${packageDetails?.packageVersion}/report`} className="hover:underline" >
						Report Package
					</Link>
				</span>
			</div>
		</div>
	);
};

export default PackageSideBar;
