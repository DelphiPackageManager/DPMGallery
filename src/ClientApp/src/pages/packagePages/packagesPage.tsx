import { SITE_URL } from "@/constants";
import { useQuery } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { getPackages } from "../../api/clientApi";
import Loader from "../../components/loader";
import Meta from "../../components/meta";
import PackageSearchBar from "../../components/packageSearchBar";
import PageContainer from "../../components/pageContainer";
import PackageItemRow from "./packageItemRow";

const PackagesPage = () => {
  const navigate = useNavigate();

  const [searchParams, setSearchParams] = useSearchParams();
  const [query, setQuery] = useState(searchParams.get("q") || "");
  const pageStr = searchParams.get("page") || "1";
  const page = Number.parseInt(pageStr);
  const [currentPage, setCurrentPage] = useState(page);

  const controller = new AbortController();

  const packagesQuery = useQuery({
    queryKey: ["packages", currentPage, query],
    queryFn: () => getPackages(currentPage, query, controller.signal),
    staleTime: 10000,
  });

  const { isLoading } = packagesQuery;

  const packages = packagesQuery.isSuccess ? packagesQuery.data : null;
  const totalPages = packages ? packages.totalPackages / packages.pageSize : 0;

  const doSetCurrentPage = (pg: number) => {
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

  useEffect(() => {
    if (query) {
      searchParams.set("q", query);
    } else {
      searchParams.delete("q");
    }
    if (currentPage > 1) {
      searchParams.set("page", currentPage.toString());
    } else {
      searchParams.delete("page");
    }

    setSearchParams(searchParams);
  }, [query, currentPage]);

  const onTagClick = (tag: string) => {
    setCurrentPage(1);
    setQuery(`tag:${tag}`);
  };

  const onSetQuery = (value: string) => {
    setCurrentPage(1);
    setQuery(value);
  };

  return (
    <>
      <Meta title="DPM - Packages" description="DPM Package List" canonical={`${SITE_URL}/packages`} />
      <div className="bg-brand sticky top-[3.5rem]">
        <div className="container mx-auto flex flex-row items-center justify-center py-2 pt-1 mt-0 max-w-6xl ">
          <PackageSearchBar value={query} onChange={onSetQuery} />
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
            return <PackageItemRow key={index} index={index} pkg={pkg} onTagClick={onTagClick} />;
          })}
        {packages && (
          <div className="flex flex-row justify-center text-xl py-4">
            <div className="mr-3">
              {packages.prevPage > 0 ? (
                <div>
                  <button className="text-sky-600 hover:underline" onClick={() => prevPageClick()}>
                    Previous
                  </button>
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
