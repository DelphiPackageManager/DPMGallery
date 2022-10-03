import React from 'react';
import {PackageResultItem} from './packageTypes'
import {NavLink} from 'react-router-dom'
import {ClockIcon,ArrowDownTrayIcon, AtSymbolIcon ,TagIcon} from '@heroicons/react/24/outline'


type PackageItemRowProps = {
    pkg : PackageResultItem;
    index : number;
};

const PackageItemRow: React.FC<PackageItemRowProps> = ({ pkg, index  }) => {
    return (
            <div key={index} className="flex flex-row flex-nowrap mb-1 py-2 align-top text-sm md:text-lg hover:bg-primary-50 dark:hover:bg-gray-700 w-full">
                <div className="md:flex items-start px-2 mr-1 md:px-3 min-w-fit">
                    <img className="w-8 h-8 md:w-12 md:h-12" src="/img/dpm64light.png" />
                </div>

                <div className="flex-grow text-left text-base text-gray-900 dark:text-gray-100">
                    <div className="flex flex-col">
                        <div className="flex flex-row w-full items-baseline mb-1">
                            <NavLink to={`/packages/${pkg.packageId}/${pkg.latestVersion}/`} className="text-base md:text-lg text-sky-600 dark:text-sky-400">{pkg.packageId}</NavLink>
                            <div className="ml-3 text-sm md:text-base text-gray-400">
                                <span>by :</span>
                                {pkg.owners.map((owner, index) => {
                                    return <NavLink className="ml-1 text-sky-600 dark:text-sky-600" key={index} to={`/profiles/${owner}`}>{owner}</NavLink>
                                })
                                }
                            </div>
                        </div>
                        <div className="flex flex-row flex-wrap text-gray-400 dark:text-gray-500 mb-2 items-start text-sm">
                            <div className='flex flex-row flex-nowrap items-center'>
                                <AtSymbolIcon className='w-4 h4 mr-1' />
                                <span className="mr-1">latest: {pkg.latestVersion} {pkg.isPrelease ? "(prerelease)" : ""} </span>
                            </div>
                            <div className='flex flex-row flex-nowrap items-center'>
                                <ClockIcon className='w-4 h4 mr-1' />
                                <span className="tooltip mr-1" data-tooltip={pkg.publishedUtc}> last updated {pkg.published}</span>
                            </div>
                            <div className='flex flex-row flex-nowrap items-center'>
                                <ArrowDownTrayIcon className='w-4 h4 mr-1'/>
                                <span className="mr-2">{pkg.totalDownloads} total {pkg.totalDownloads == 1 ? "download" : "downloads"}</span>
                            </div>
                            <div  className='flex flex-row items-center'>
                                {pkg.tags && <TagIcon className="w-4 h4 mr-1" />}
                                {pkg.tags?.map((tag, index) => {
                                    return (
                                            <NavLink className="mr-1 text-sky-700 dark:text-sky-700" key={index} to={`/packages?q=tag%3A${tag}`}>{tag}</NavLink>
                                    )
                                })}
                            </div>
                        </div>
                        <div className="text-sm md:text-base">
                            <span className="word-break">
                                {pkg.description}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
     )
}

export default PackageItemRow;
