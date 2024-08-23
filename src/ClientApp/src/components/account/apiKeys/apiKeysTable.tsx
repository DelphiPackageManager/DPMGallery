import { Checkbox, CheckedState } from "@/components/ui/checkbox";
import { DataTable } from "@/components/ui/data-table/data-table";
import { DataTableColumnHeader } from "@/components/ui/data-table/data-table-column-header";
import { PagedList } from "@/lib/paging";
import { cn } from "@/lib/utils";
import { ApiKey, ApiKeyScope, apiKeyScopesToString } from "@/types/apiKeys";
import { Constants } from "@/types/constants";
import { expiryDays } from "@/utils/dateUtils";
import { updatePagingUsingSearchParams } from "@/utils/pagingUtils";
import { ColumnDef, Row } from "@tanstack/react-table";
import { format } from 'date-fns';
import { CircleAlert, TriangleAlert } from "lucide-react";
import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { ApiKeyDisplay } from "./apiKeyDisplay";
import ApiKeyRowActions from "./apiKeyRowActions";
import { fetchApiKeys, updateApiKeyEnabled } from "./apiKeysApi";
import ApiKeyTableActions from "./apiKeyTableActions";

export default function ApiKeysTable() {

	const [error, setError] = useState("");
	const [apiKeys, setApiKeys] = useState<PagedList<ApiKey>>()
	const [searchParams, setSearchParams] = useSearchParams();
	const [newApiKey, setNewApiKey] = useState<ApiKey | null>(null);

	function onUpdate(apiKey?: ApiKey | null) {
		if (apiKey)
			setNewApiKey(apiKey);

		fetchTableContent();
	}
	function onRowEnableToggle(checked: string | boolean, row: Row<ApiKey>): void {

		const rowId = row.original.id;
		const apiKey = apiKeys?.items.find(x => x.id === rowId);
		if (!apiKey?.id)
			return;

		const enabled = checked as boolean;
		apiKey.revoked = !enabled;
		updateApiKeyEnabled(enabled, apiKey.id);
		setApiKeys(apiKeys);
	}
	const columns: ColumnDef<ApiKey>[] = [
		{
			accessorKey: "name",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Name" allowSorting={true} />
			),
			cell: ({ row }) => {
				const name = row.getValue<string>("name");
				const expiresUTC = row.getValue<Date>("expiresUTC");
				const expiresInDays = expiryDays(expiresUTC);
				let className = expiresInDays < 0 ? "line-through text-gray-400" : "";
				return <div className={cn(className)}>{name}</div>
			},
			meta: {
				headerClassName: "w-36",

			}
		},
		{
			accessorKey: "globPattern",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Glob Pattern" />
			),
			cell: ({ row }) => {
				const globPattern = row.getValue<string>("globPattern");
				return <div>{globPattern}</div>
			},
			meta: {
				headerClassName: "w-40",

			}
		},
		{
			accessorKey: "packages",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Packages" />
			),
			cell: ({ row }) => {
				let packages = row.getValue<string>("packages").split(",");

				return <div>{packages.map((x: string, index: number) => {
					return (<div key={index}><span >{x}</span><br /></div>)
				})}</div>
			},
			meta: {
				headerClassName: "w-40",
				cellClassName: ""
			}

		},
		{
			accessorKey: "scopes",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Scopes" />
			),
			cell: ({ row }) => {
				const scopesValue = row.getValue<ApiKeyScope>("scopes");
				const scopes = apiKeyScopesToString(scopesValue);
				return <div>{scopes}</div>
			}
		},
		{
			accessorKey: "expiresUTC",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="Expires" allowSorting={true} />
			),
			cell: ({ row }) => {
				const expiresUTC = row.getValue<Date>("expiresUTC");
				const expiresInDays = expiryDays(expiresUTC);
				let expiredClassName = expiresInDays < 0 ? "line-through text-gray-400" : "";
				let titleText;
				let iconType = null;
				if (expiresInDays < 0) {

					titleText = `Expired ${-expiresInDays} days ago`;
					iconType = <CircleAlert size={16} className="text-destructive" />;
				}
				else if (expiresInDays === 0) {
					titleText = "Expires today";
					iconType = <TriangleAlert size={16} className="text-orange-400" />;
				}
				else if (expiresInDays === 1) {
					titleText = "Expires tomorrow";
					iconType = <TriangleAlert size={16} className="text-orange-400" />;
				}
				else
					titleText = `Expires in ${expiresInDays} days`;

				return <div title={format(expiresUTC, "Pp")} className={cn("flex flex-row space-x-1", expiredClassName)}><span>{titleText}</span> {iconType}</div>
			},
			meta: {
				headerClassName: "w-40",
			}
		},
		{
			accessorKey: "enabled",
			header: ({ column }) => (
				<DataTableColumnHeader column={column} title="" allowSorting={false} />
			),
			cell: ({ row }) => {
				const revoked = row.original.revoked;
				const enabled = !revoked;
				const id = (row.original.id ?? "").toString();
				return <div className="flex items-center justify-center"><Checkbox id={id} name="enabled" title="Enable/disable" onCheckedChange={(checked: CheckedState) => onRowEnableToggle(checked, row)} defaultChecked={enabled} /></div>;

			},
			meta: {

			}

		},
		{
			id: "actions",
			maxSize: 2,
			cell: ({ row }) => (
				<ApiKeyRowActions apiKey={row.original} onUpdate={onUpdate} />
			)

		}

	]

	const updatePaging = updatePagingUsingSearchParams(searchParams, setSearchParams, fetchTableContent)

	function onApiKeyCreate(newKey: ApiKey): void {

		if (!apiKeys)
			return;

		//store key for display
		setNewApiKey(newKey);
	}


	function fetchTableContent() {
		let fetching = true;

		const fetch = async () => {

			const page = searchParams.get("page") ?? "1";
			const pagenumber = parseInt(page);
			const sort = searchParams.get("sort") ?? "name";
			const sortDirection = searchParams.get("sortDirection") ?? "asc";
			const filter = searchParams.get("filter") ?? "";
			const paging = { pageSize: Constants.DefaultPageSize, page: pagenumber, sort: sort, sortDirection: sortDirection, filter: filter };
			const fetchResponse = await fetchApiKeys(paging);
			if (fetchResponse.error) {
				setError(fetchResponse.error);
				return;
			}

			let result = fetchResponse.result;
			if (!result)
				return;

			if (fetching) {
				setApiKeys(result);
				if (result.paging) {
					console.log("fetchTableContent: updatePaging", result.paging);
					updatePaging(result.paging, false);
				}
			}
		};

		fetch()
			.catch(console.error);

		return () => { fetching = false; };
	}

	useEffect(() => {
		return fetchTableContent();
	}, []);


	if (!apiKeys)
		return <div>Loading...</div>;

	const openKeyDisplay = newApiKey?.key ? true : false;

	const onKeyDisplayOpenChange = (open: boolean) => {
		if (!open) {
			fetchTableContent();
			setNewApiKey(null)
		}
	};

	return (
		<div className="mt-4">
			<DataTable className="w-full" columns={columns} data={apiKeys} showRowCount={true} showPageCount={false} showPageLinks={false} showFilter={false} allowGoToPage={false} updatePaging={updatePaging} padEndRows={true} renderTableActions={() => <ApiKeyTableActions onCreate={onApiKeyCreate} />} />
			<ApiKeyDisplay apiKey={newApiKey} open={openKeyDisplay} onOpenChange={onKeyDisplayOpenChange} />
			<div>{error}</div>
		</div>

	)


}