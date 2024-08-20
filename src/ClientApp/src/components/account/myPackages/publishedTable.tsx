import { DataTable } from "@/components/ui/data-table/data-table";
import { DataTableColumnHeader } from "@/components/ui/data-table/data-table-column-header";
import { PagedList } from "@/lib/paging";
import { Constants } from "@/types/constants";
import { ManagePackageInfo } from "@/types/managePackages";
import { updatePagingUsingSearchParams } from "@/utils/pagingUtils";
import { ColumnDef } from "@tanstack/react-table";
import { Users } from "lucide-react";
import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchPackageInfos } from "./myPackagesApi";
import PackageRowActions from "./packageRowActions";

export type PublishedPackagesTableProps = {
	currentUser: string;
	filter: string;
}

const PublishedPackagesTable = ({ currentUser, filter }: PublishedPackagesTableProps) => {

	const [packageInfos, setPackageInfos] = useState<PagedList<ManagePackageInfo>>()
	const [searchParams, setSearchParams] = useSearchParams();
	const [error, setError] = useState("");

	const columns: ColumnDef<ManagePackageInfo>[] = [
		{
			accessorKey: "packageId",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="packageId" allowSorting={true} />
			),
			cell: ({ row }) => {
				const packageId = row.getValue<string>("packageId");
				return <div >{packageId}</div>
			},
			meta: {
				headerClassName: "w-36",
			}
		},
		{
			accessorKey: "owners",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Owners" allowSorting={false} />
			),
			cell: ({ row }) => {
				const owners = row.getValue<string[]>("owners");

				return <div>
					{owners.map((value: string, index: number) => {
						if (value == currentUser) {
							return (<Link key={index} to={`/profiles/${value}`}>{value}</Link>)
						} else {
							//org
							return (<Link key={index} to={`/profiles/${value}`}><Users size={16} strokeWidth={1.25} absoluteStrokeWidth />{value}</Link>)
						}
					})}

				</div>

			},
			meta: {
				headerClassName: "w-40",
				cellClassName: ""
			}
		},
		{
			accessorKey: "downloads",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Downloads" allowSorting={false} />
			),
			cell: ({ row }) => {
				const downloads = row.getValue<number>("downloads");
				return <div >{downloads.toString()}</div>
			}
		},
		{
			accessorKey: "latestVersion",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Latest Version" allowSorting={true} />
			),
			cell: ({ row }) => {
				const version = row.getValue<string>("latestVersion");
				return <div >{version}</div>
			},
			meta: {
				headerClassName: "w-36",
			}
		},
		{
			id: "actions",
			maxSize: 1,
			cell: ({ row }) => {
				const packageId = row.getValue<string>("packageId");
				const version = row.getValue<string>("latestVersion");
				return <PackageRowActions packageId={packageId} version={version} />
			}
		}
	]

	function fetchTableContent() {
		let fetching = true;

		const fetch = async () => {
			const page = searchParams.get("page") ?? "1";
			const pagenumber = parseInt(page);
			const sort = searchParams.get("sort") ?? "name";
			const sortDirection = searchParams.get("sortDirection") ?? "asc";
			const filter = searchParams.get("filter") ?? "";
			const paging = { pageSize: Constants.DefaultPageSize, page: pagenumber, sort: sort, sortDirection: sortDirection, filter: filter };
			const fetchResponse = await fetchPackageInfos(paging, false);
			if (fetchResponse.error) {
				setError(fetchResponse.error);
				return;
			}

			let result = fetchResponse.result;
			if (!result)
				return;

			if (fetching) {
				setPackageInfos(result);
			}
		};

		fetch()
			.catch(console.error);

		return () => { fetching = false; };
	}

	useEffect(() => {
		return fetchTableContent();
	}, []);


	if (!packageInfos)
		return <div>Loading...</div>;

	const updatePaging = updatePagingUsingSearchParams(searchParams, setSearchParams, fetchTableContent)


	return (
		<div className="mt-4">
			<DataTable className="w-full" columns={columns} data={packageInfos} showRowCount={true} showPageCount={false} showPageLinks={false} showFilter={false} allowGoToPage={false} updatePaging={updatePaging} padEndRows={true} />
			<div>{error}</div>
		</div>

	)
}


export default PublishedPackagesTable;