import { DataTable } from "@/components/ui/data-table/data-table";
import { DataTableColumnHeader } from "@/components/ui/data-table/data-table-column-header";
import { PagedList } from "@/lib/paging";
import { MemberRole, memberRoleToString, UserOrganisation } from "@/types/organisations";
import { ColumnDef } from "@tanstack/table-core";
import { useEffect, useState } from "react";
import { fetchOrganisations } from "./organisationApi";
import OrganisationRowActions from "./organisationRowActions";
import OrganisationsTableActions from "./organisationsTableActions";

export default function OrganisationsTable() {
	const [error, setError] = useState("");
	const [organisations, setOrganisations] = useState<PagedList<UserOrganisation>>();


	function onUpdate(org?: UserOrganisation | null) {
		fetchTableContent();
	}


	function onOrganisationCreate(newOrganisation: UserOrganisation): void {

		if (!organisations)
			return;

		fetchTableContent();
	}




	const columns: ColumnDef<UserOrganisation>[] = [
		{
			accessorKey: "avatarUrl",
			enableSorting: false,
			header: ({ column }) => {
				<DataTableColumnHeader column={column} title="Avatar" allowSorting={false} />
			},
			cell: ({ row }) => {
				const avatarUrl = row.getValue<string>("avatarUrl");
				return (
					<div className="h-6 w-6">
						<img className="rounded-md" src={`${avatarUrl}`} alt="" />
					</div>
				);

			}

		},

		{
			accessorKey: "name",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Name" allowSorting={true} />
			),
			cell: ({ row }) => {
				const name = row.getValue<string>("name");
				return <div >{name}</div>
			},
			meta: {
				headerClassName: "w-4/2"
			}
		},
		{
			accessorKey: "role",
			enableSorting: false,
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Role" />
			),
			cell: ({ row }) => {
				const roleName = row.getValue<string>("role");
				return <div>{roleName}</div>
			}
		},
		{
			accessorKey: "memberCount",
			enableSorting: false,
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Members" />
			),
			accessorFn: (row) => `${row.adminCount + row.collaboratorCount}`,
			cell: ({ row }) => {
				const memberCount = row.getValue<number>("memberCount") | 0;
				return <div>{memberCount}</div>
			}
		},
		{
			accessorKey: "packageCount",
			enableSorting: false,
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Package Count" allowSorting={true} />
			),
			cell: ({ row }) => {
				const packageCount = row.getValue<number>("packageCount");
				return <div >{packageCount}</div>

			},
		},
		{
			id: "actions",
			maxSize: 2,
			cell: ({ row }) => (
				<OrganisationRowActions org={row.original} onUpdate={onUpdate} />
			)

		}

	]



	function fetchTableContent() {
		let fetching = true;

		const fetch = async () => {

			const fetchResponse = await fetchOrganisations();
			if (fetchResponse.error) {
				setError(fetchResponse.error);
				return;
			}

			let result = fetchResponse.result;
			if (!result)
				return;

			if (fetching) {
				setOrganisations(result);
			}
		};

		fetch()
			.catch(console.error);

		return () => { fetching = false; };
	}


	useEffect(() => {
		return fetchTableContent();
	}, []);


	if (!organisations)
		return <div>Loading...</div>;


	return (
		<div className="container mx-auto py-5">
			<DataTable columns={columns} data={organisations} showRowCount={true} showPageCount={false} showPageLinks={false} showFilter={false} allowGoToPage={false} padEndRows={true} renderTableActions={() => <OrganisationsTableActions onCreate={onOrganisationCreate} />} />
			<div>{error}</div>
		</div>

	)

}