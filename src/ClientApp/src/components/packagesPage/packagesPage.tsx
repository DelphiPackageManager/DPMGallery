import PackageSearchBar from "../packageSearchBar";
import { useEffect, useRef } from "react";
import { useSearchParams } from "react-router-dom";
import {NavLink} from 'react-router-dom'
import PackageItemRow from './packageItemRow'
import usePackages, {SearchParams} from "../../hooks/usePackages";


const PackagesPage = () => {
    const effectRan = useRef(false)
    let [searchParams] = useSearchParams();
    const query = searchParams.get('q') || '';
    const pageStr = searchParams.get('page') || '1';
    const page = Number.parseInt(pageStr);

    console.log(query);
    const sq: SearchParams = {
        query: query,
        page : page
    }
    const controller = new AbortController();

    const [{loading, error, packages}, getPackages] = usePackages();
   
    
     useEffect(() => {
        //dealing with useffect running twice in dev mode due to strict mode
        if (effectRan.current === true ) {
            getPackages(sq,controller);       
        }
        return () => {
            if (effectRan.current === true ) {
                controller.abort()
            }
            effectRan.current = true;
           
        }

    },[query, page, getPackages])




    function getPageLink (query: string, page : number) : string  {
        let result = "";
        if (query !== "") {
            result = `?q=${query}`;
        }
        if (page > 1) {
            if (result != "") {
                result = result + `&page=${page}`;
            } else {
                result = `?page=${page}`;
            }
        }
        return result;
    }


    return (
        <>
            <div className="bg-gray-900 sticky top-[3.5rem]">
                <div className="container mx-auto flex flex-row items-center justify-center py-2 pt-1 mt-0 max-w-6xl ">
                    <PackageSearchBar value={query} />
                </div>
            </div>
            {!error && loading && 
                 <div className="container mx-auto">
                    <div className="text-2xl  ml-24 text-center">
                        <p>Loading....???</p>
                    </div>
                </div>
            }

            {!error && !loading && packages && 
                <div className="container mx-auto max-w-6xl text-gray-500 text-center ">
                    <div className="text-2xl  ml-24  mt-2">
                        {packages.query == '' ?
                            <h1 role="alert" className="">There are {packages?.totalPackages.toString()} packages.</h1>
                            :
                            <h1 role="alert" className="">{packages?.totalPackages.toString()} packages returned for "{packages?.query}"</h1>
                        }
                    </div>
                </div>
            }
            {!error && !loading &&packages &&      
                <div className="container mx-auto max-w-6xl pt-2 px-2 mb-4 text-gray-700 dark:text-gray-500">
                    {packages?.packages.map((pkg, index) => {
                        return (
                            <PackageItemRow key={index} index={index} pkg={pkg} />
                        )
                    })}
                </div>
            }
            {!error && !loading &&packages &&      
               <div className="container mx-auto flex flex-row justify-center text-xl py-4 max-w-6xl ">
                    <div className="mr-3">
                        {packages.prevPage > 0 ? 
                                <div><NavLink to={getPageLink(query, packages.prevPage)} className="text-sky-600">Previous</NavLink></div>
                            :
                                <div><span className="dark:text-gray-600 text-gray-300">Previous</span></div>
                        }
                    </div>
                    <div><span className="px-1 text-gray-400"  >|</span></div>
                    <div className="ml-3">
                        {packages.nextPage > 0 ? 
                                <div><NavLink to={getPageLink(query, packages.nextPage)} className="text-sky-600">Next</NavLink></div>
                                :
                                <div><span className="dark:text-gray-600 text-gray-300">Next</span></div>
                        }
                    </div>

                </div>
            }
        </>
    )


}

export default PackagesPage;