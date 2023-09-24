import { SITE_URL } from "@/constants";
import { firstOrNull } from "@/utils";
import { useQuery } from "@tanstack/react-query";
import { useEffect, useRef, useState } from "react";
import { NavLink, useParams, useSearchParams } from "react-router-dom";
import { getPackages } from "../api/clientApi";
import Meta from "../components/meta";
import PackageItemRow from "../components/packagesPage/packageItemRow";
import { PackageOwnerModel, PackageSearchResult } from "../components/packagesPage/packageTypes";
import PageContainer from "../components/pageContainer";

const ProfilePage = () => {
  let { userName } = useParams();
  let [searchParams, setSearchParams] = useSearchParams();
  const [ownerInfo, setOwnerInfo] = useState<PackageOwnerModel | null>(null);
  const pageStr = searchParams.get("page") || "1";
  const page = Number.parseInt(pageStr);
  const [currentPage, setCurrentPage] = useState(page);

  const query = `owner:${userName}`;

  const controller = new AbortController();

  const packagesQuery = useQuery({
    queryKey: ["packages", page, query],
    queryFn: () => getPackages(page, query, controller.signal),
  });

  const { isError, isLoading } = packagesQuery;

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

  useEffect(() => {
    if (packages) {
      let firstPackage = firstOrNull(packages.packages);
      if (firstPackage) {
        let info = firstPackage.ownerInfos.find((item) => {
          return item.userName === userName;
        });
        if (info) setOwnerInfo(info);
      }
    }
  }, [packages]);

  function getPageLink(page: number): string {
    let result = "";
    if (page > 1) {
      result = `?page=${page}`;
    }
    return result;
  }

  //todo : use same code from packages page.
  return (
    <PageContainer>
      <Meta title={`DPM - Profile - ${userName}`} canonical={`${SITE_URL}/packages`} />

      <div className="flex flex-col-reverse md:flex-row pt-2 grow">
        <div className="px-4 mt-2 md:mt-1 md:ml-2  border-r border-gray-400 dark:border-gray-600 grow">
          {!isError && !isLoading && packages && (
            <div className="container mx-auto max-w-5xl pt-2 px-2 mb-4 text-gray-700 dark:text-gray-500">
              {packages?.packages.map((pkg, index) => {
                return <PackageItemRow key={index} index={index} pkg={pkg} />;
              })}
            </div>
          )}
          {!isError && !isLoading && packages && (
            <div className="flex flex-row justify-center text-xl py-4 ">
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
        </div>
        <div className=" w-1/5 pl-4">
          <div className="py-2  mb-2 ml-2">
            <h1 className="uppercase text-lg md:text-xl text-gray-900 dark:text-gray-100 border-b border-gray-400 dark:border-gray-600">
              {userName}
            </h1>
          </div>
          {ownerInfo && (
            <div className="pl-2 w-24 h-24">
              <img src={`https://www.gravatar.com/avatar/${ownerInfo.emailHash}`} className="" />
            </div>
          )}
          <div className="flex flex-col ">
            <div className="mt-4 text-2xl text-gray-600 dark:text-gray-300">
              <span className="whitespace-nowrap">{packages?.totalPackages} Packages</span>
            </div>
            <div className="flex flex-row md:flex-col mt-1 md:mt-2 items-baseline">
              <div className="text-base md:text-xl text-gray-600 dark:text-gray-400">
                <span>124,687</span>
              </div>
              <div className="ml-2 md:ml-0 mt-1 text-sm md:text-lg text-gray-600 dark:text-gray-400">
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
