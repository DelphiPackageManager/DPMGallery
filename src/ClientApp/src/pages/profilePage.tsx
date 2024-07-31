import { SITE_URL } from "@/types/constants";
import { firstOrNull } from "@/utils";
import { useQuery } from "@tanstack/react-query";
import { useEffect, useState } from "react";
import { useNavigate, useParams, useSearchParams } from "react-router-dom";
import { getPackages } from "../api/clientApi";
import Meta from "../components/meta";
import PageContainer from "../components/pageContainer";
import PackageItemRow from "./packagePages/packageItemRow";
import { PackageOwnerModel } from "./packagePages/packageTypes";

const ProfilePage = () => {
	const navigate = useNavigate();
	const { userName } = useParams();
	const [searchParams, setSearchParams] = useSearchParams();
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

	const onTagClick = (tag: string) => {
		navigate(`/packages?q=tag:${tag}`);
	};

	return (
		<PageContainer>
			<Meta title={`DPM - Profile - ${userName}`} canonical={`${SITE_URL}/packages`} />

			<div className="flex grow flex-col-reverse pt-2 md:flex-row">
				<div className="mt-2 grow border-r border-gray-400 px-4 dark:border-gray-600 md:ml-2 md:mt-1">
					{!isError && !isLoading && packages && (
						<div className="container mx-auto mb-4 max-w-7xl px-2 pt-2 text-gray-700 dark:text-gray-500">
							{packages?.packages.map((pkg, index) => {
								return <PackageItemRow key={index} index={index} pkg={pkg} onTagClick={onTagClick} />;
							})}
						</div>
					)}
					{!isError && !isLoading && packages && (
						<div className="flex flex-row justify-center py-4 text-xl">
							<div className="mr-3">
								{packages.prevPage > 0 ? (
									<div>
										<button className="text-sky-600 hover:underline" onClick={() => prevPageClick()}>
											Previous
										</button>
									</div>
								) : (
									<div>
										<span className="text-gray-300 dark:text-gray-600">Previous</span>
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
										<span className="text-gray-300 dark:text-gray-600">Next</span>
									</div>
								)}
							</div>
						</div>
					)}
				</div>
				<div className="w-1/5 pl-4">
					<div className="mb-2 ml-2 py-2">
						<h1 className="border-b border-gray-400 text-lg uppercase text-gray-900 dark:border-gray-600 dark:text-gray-100 md:text-xl">
							{userName}
						</h1>
					</div>
					{ownerInfo && (
						<div className="h-24 w-24 pl-2">
							<img src={`https://www.gravatar.com/avatar/${ownerInfo.emailHash}`} className="" />
						</div>
					)}
					<div className="flex flex-col">
						<div className="mt-4 text-2xl text-gray-600 dark:text-gray-300">
							<span className="whitespace-nowrap">{packages?.totalPackages} Packages</span>
						</div>
					</div>
				</div>
			</div>
		</PageContainer>
	);
};

export default ProfilePage;
