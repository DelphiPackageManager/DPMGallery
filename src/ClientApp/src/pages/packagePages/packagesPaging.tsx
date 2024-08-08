import { PackageSearchResult } from "./packageTypes";

type PackagesPageProps = {
	packages: PackageSearchResult | null;
	prevPageClick: () => void;
	nextPageClick: () => void;
}

const PackagesPaging = ({ packages, prevPageClick, nextPageClick }: PackagesPageProps) => {

	return (
		<>
			{packages && packages.totalPackages && (packages.totalPackages > packages.pageSize) && (
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
		</>
	)

}

export default PackagesPaging;
