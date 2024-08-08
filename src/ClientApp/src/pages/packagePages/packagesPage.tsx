import { SITE_URL } from "@/types/constants";
import { useQuery } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { getPackages } from "../../api/clientApi";
import Meta from "../../components/meta";
import PackageSearchBar from "../../components/packageSearchBar";
import PageContainer from "../../components/pageContainer";
import PackageItemRow from "./packageItemRow";
import PackageRowSkeleton from "./packageRowSkeleton";
import PackagesPaging from "./packagesPaging";

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

	const skeletonRows = Array(12).fill(0);

	return (
		<>
			<Meta title="DPM - Packages" description="DPM Package List" canonical={`${SITE_URL}/packages`} />
			<div className="sticky top-[3.5rem] bg-brand">
				<div className="container mx-auto mt-0 flex max-w-6xl flex-row items-center justify-center py-2 pt-1">
					<PackageSearchBar value={query} onChange={onSetQuery} />
				</div>
			</div>
			<PageContainer>
				{isLoading &&
					<div className="mt-2 text-center text-2xl">
						<h1 role="alert" className="">
							Loading...
						</h1>
					</div>

				}
				{isLoading &&
					<div className="z-0">
						{skeletonRows.map(() =>
							<PackageRowSkeleton />
						)}

					</div>

				}

				{packages && (
					<div className="mt-2 text-center text-2xl">
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
				<PackagesPaging packages={packages} nextPageClick={nextPageClick} prevPageClick={prevPageClick} />

				{packages &&
					packages?.packages.map((pkg, index) => {
						return (
							<PackageItemRow key={index} index={index} pkg={pkg} onTagClick={onTagClick} />
						)
					})}
				<PackagesPaging packages={packages} nextPageClick={nextPageClick} prevPageClick={prevPageClick} />

			</PageContainer>
		</>
	);
};

export default PackagesPage;
