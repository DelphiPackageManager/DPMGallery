import { Skeleton } from "@/components/ui/skeleton";

const PackageRowSkeleton = () => {
	return (
		<div className="hover:bg-primary-50 mb-1 flex w-full flex-row flex-nowrap py-2 align-top text-sm dark:hover:bg-gray-700 md:text-lg">
			<div className="mr-1 min-w-fit items-start px-2 md:flex md:px-3">
				<Skeleton className="h-12 w-12" />
			</div>

			<div className="grow">
				<Skeleton className="h-20 w-4/6" />
			</div>
		</div>
	)
}

export default PackageRowSkeleton;