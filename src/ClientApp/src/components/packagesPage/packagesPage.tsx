import { SITE_URL } from "@/constants";
import { useQuery } from "@tanstack/react-query";
import { useEffect, useRef, useState } from "react";
import { NavLink, useSearchParams } from "react-router-dom";
import { getPackages } from "../../api/clientApi";
import usePackages, { SearchParams } from "../../hooks/usePackages";
import Loader from "../loader";
import Meta from "../meta";
import PackageSearchBar from "../packageSearchBar";
import PageContainer from "../pageContainer";
import PackageItemRow from "./packageItemRow";

const PackagesPage = () => {
  let [searchParams, setSearchParams] = useSearchParams();
  const query = searchParams.get("q") || "";
  const pageStr = searchParams.get("page") || "1";
  const page = Number.parseInt(pageStr);
  const [currentPage, setCurrentPage] = useState(page);

  const controller = new AbortController();

  const packagesQuery = useQuery({
    queryKey: ["packages", currentPage, query],
    queryFn: () => getPackages(currentPage, query, controller.signal),
    staleTime: 10000,
    keepPreviousData: true,
  });

  const { isLoading } = packagesQuery;

  const packages = packagesQuery.isSuccess ? packagesQuery.data : null;
  const totalPages = packages ? packages.totalPackages / packages.pageSize : 0;

  const doSetCurrentPage = (pg: number) => {
    if (pg > 1) {
      searchParams.set("page", pg.toString());
    } else {
      searchParams.delete("page");
    }
    setSearchParams(searchParams);

    setCurrentPage(pg);
  };

  const prevPageClick = () => {
    if (currentPage > 1) {
      doSetCurrentPage(currentPage - 1);
    }
  };

  const nextPageClick = () => {
    if (currentPage < totalPages) {
      doSetCurrentPage(currentPage + 1);
    }
  };

  return (
    <>
      <Meta title="DPM - Packages" description="DPM Package List" canonical={`${SITE_URL}/packages`} />
      <div className="bg-brand sticky top-[3.5rem]">
        <div className="container mx-auto flex flex-row items-center justify-center py-2 pt-1 mt-0 max-w-6xl ">
          <PackageSearchBar value={query} />
        </div>
      </div>
      <PageContainer>
        {isLoading && <Loader />}

        {packages && (
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
        {packages &&
          packages?.packages.map((pkg, index) => {
            return <PackageItemRow key={index} index={index} pkg={pkg} />;
          })}
        {packages && (
          <div className="flex flex-row justify-center text-xl py-4">
            <div className="mr-3">
              {packages.prevPage > 0 ? (
                <div>
                  <button className="text-sky-600 hover:underline" onClick={() => prevPageClick()}>
                    Previous
                  </button>

                  {/* <NavLink to={getPageLink(query, packages.prevPage)} className="text-sky-600  hover:underline">
                      Previous
                    </NavLink> */}
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
                  <button className="text-sky-600 hover:underline" onClick={() => nextPageClick()}>
                    Next
                  </button>

                  {/* <NavLink to={getPageLink(query, packages.nextPage)} className="text-sky-600 hover:underline">
                      Next
                    </NavLink> */}
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
