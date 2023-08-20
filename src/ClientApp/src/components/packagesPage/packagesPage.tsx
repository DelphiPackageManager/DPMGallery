import { useEffect, useRef } from "react";
import { NavLink, useSearchParams } from "react-router-dom";
import usePackages, { SearchParams } from "../../hooks/usePackages";
import Loader from "../loader";
import PackageSearchBar from "../packageSearchBar";
import PageContainer from "../pageContainer";
import PackageItemRow from "./packageItemRow";

const PackagesPage = () => {
  let [searchParams] = useSearchParams();
  const query = searchParams.get("q") || "";
  const pageStr = searchParams.get("page") || "1";
  const page = Number.parseInt(pageStr);

  const sq: SearchParams = {
    query: query,
    page: page,
  };
  const controller = new AbortController();

  const [{ loading, error, packages }, getPackages] = usePackages();

  useEffect(() => {
    //dealing with useffect running twice in dev mode due to strict mode
    getPackages(sq, controller);
  }, [query, page]);

  function getPageLink(query: string, page: number): string {
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
      <div className="bg-primary dark:bg-primary-900 sticky top-[3.5rem]">
        <div className="container mx-auto flex flex-row items-center justify-center py-2 pt-1 mt-0 max-w-6xl ">
          <PackageSearchBar value={query} />
        </div>
      </div>
      <PageContainer>
        {!error && loading && <Loader />}

        {!error && !loading && packages && (
          <div className="text-2xl  mt-2 text-center">
            {packages.query == "" ? (
              <h1 role="alert" className="">
                There are {packages?.totalPackages.toString()} packages.
              </h1>
            ) : (
              <h1 role="alert" className="">
                {packages?.totalPackages.toString()} packages found for "{packages?.query}"
              </h1>
            )}
          </div>
        )}
        {!error &&
          !loading &&
          packages &&
          packages?.packages.map((pkg, index) => {
            return <PackageItemRow key={index} index={index} pkg={pkg} />;
          })}
        {!error && !loading && packages && (
          <div className="flex flex-row justify-center text-xl py-4">
            <div className="mr-3">
              {packages.prevPage > 0 ? (
                <div>
                  <NavLink to={getPageLink(query, packages.prevPage)} className="text-sky-600">
                    Previous
                  </NavLink>
                </div>
              ) : (
                <div>
                  <span className="dark:text-gray-600 text-gray-300">Previous</span>
                </div>
              )}
            </div>
            <div>
              <span className="px-1 text-gray-400">|</span>
            </div>
            <div className="ml-3">
              {packages.nextPage > 0 ? (
                <div>
                  <NavLink to={getPageLink(query, packages.nextPage)} className="text-sky-600">
                    Next
                  </NavLink>
                </div>
              ) : (
                <div>
                  <span className="dark:text-gray-600 text-gray-300">Next</span>
                </div>
              )}
            </div>
          </div>
        )}
      </PageContainer>
    </>
  );
};

export default PackagesPage;
