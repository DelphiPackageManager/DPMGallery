import PackageSearchBar from "./packageSearchBar";
import { useEffect } from "react";
import { useSearchParams } from "react-router-dom";

const PackagesPage = () => {
    let [searchParams] = useSearchParams();
    const query  = searchParams.get('q') ;
    


    useEffect(() => {
        const controller = new AbortController();
        //todo : get package list from server


    },[query])
    
    let value = query ? query : "";

    return (
        <div className="w-full">
            <div className="bg-gray-200 dark:bg-gray-900">
                <div className="container mx-auto flex flex-row items-center justify-center py-2">
                    <PackageSearchBar value={value} />
                </div>
            </div>
            <div className="container mx-auto flex flex-row items-center justify-center mt-2 py-2">
                <h1>{query}</h1>
            </div>
        </div>
    )

}

export default PackagesPage;