import React from 'react';
import { useParams } from 'react-router-dom'
import { useEffect, useRef } from "react";
import { useSearchParams } from "react-router-dom";
import { NavLink } from 'react-router-dom'
import PackageItemRow from './packagesPage/packageItemRow'
import usePackages, { SearchParams } from "../hooks/usePackages";
import PackageSearchBar from './packageSearchBar';
import PageContainer from './pageContainer';


const ProfilePage = () => {
    const effectRan = useRef(false)
    let { userName } = useParams();
    let [searchParams] = useSearchParams();
    const pageStr = searchParams.get('page') || '1';
    const page = Number.parseInt(pageStr);

    const query = `owner:${userName}`;

    const sq: SearchParams = {
        query: query,
        page: page
    }
    const controller = new AbortController();

    const [{ loading, error, packages }, getPackages] = usePackages();

    useEffect(() => {
        //dealing with useffect running twice in dev mode due to strict mode
        if (effectRan.current === true) {
            getPackages(sq, controller);
        }
        return () => {
            if (effectRan.current === true) {
                controller.abort()
            }
            effectRan.current = true;
        }
    }, [query, page])


    function getPageLink ( page : number) : string  {
        let result = ""
        if (page > 1) {
             result = `?page=${page}`;
        }
        return result;
    }



    //todo : use same code from packages page.
    return (
        <PageContainer>
            <div className='flex flex-col-reverse md:flex-row pt-2 flex-grow'>
                <div className='px-4 mt-2 md:mt-1 md:ml-2  border-r border-gray-400 dark:border-gray-600 flex-grow'>
                    
                {!error && !loading &&packages &&      
                    <div className="container mx-auto max-w-5xl pt-2 px-2 mb-4 text-gray-700 dark:text-gray-500">
                        {packages?.packages.map((pkg, index) => {
                            return (
                                <PackageItemRow key={index} index={index} pkg={pkg} />
                            )
                        })}
                    </div>
                }
                {!error && !loading &&packages &&      
                <div className="flex flex-row justify-center text-xl py-4 ">
                        <div className="mr-3">
                            {packages.prevPage > 0 ? 
                                    <div><NavLink to={getPageLink(packages.prevPage)} className="text-sky-500">Previous</NavLink></div>
                                :
                                    <div><span className="dark:text-gray-600 text-gray-300">Previous</span></div>
                            }
                        </div>
                        <div><span className="px-1 text-gray-400"  >|</span></div>
                        <div className="ml-3">
                            {packages.nextPage > 0 ? 
                                    <div><NavLink to={getPageLink(packages.nextPage)} className="text-sky-500">Next</NavLink></div>
                                    :
                                    <div><span className="dark:text-gray-600 text-gray-300">Next</span></div>
                            }
                        </div>

                    </div>
                }
                    
                </div>
                <div className=' w-1/5 pl-4'>
                    <div className='py-2  mb-2 ml-2'>
                        <h1 className='uppercase text-lg md:text-xl text-gray-900 dark:text-gray-100 border-b border-gray-400 dark:border-gray-600'>{userName}</h1>
                    </div>
                    <div className='pl-2 w-24 h-24'>
                        <img src='https://www.gravatar.com/avatar/537d208f522e2d50f727370037b3a75f' className='' />
                    </div>
                    <div className='flex flex-col '>
                        <div className='mt-4 text-2xl text-gray-600 dark:text-gray-300'>
                            <span className='whitespace-nowrap'>{packages?.totalPackages}  Packages</span>
                        </div>
                        <div className="flex flex-row md:flex-col mt-1 md:mt-2 items-baseline">
                            <div className='text-base md:text-xl text-gray-600 dark:text-gray-400'>
                                <span>124,687</span>
                            </div>
                            <div className='ml-2 md:ml-0 mt-1 text-sm md:text-lg text-gray-600 dark:text-gray-400'>
                                <span>Downloads</span>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

        </PageContainer>
    );
};

export default ProfilePage;


