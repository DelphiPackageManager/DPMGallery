import { PagedList, Paging } from "@/lib/paging"
import { cn } from "@/lib/utils"
import { ColumnDef, Row, RowData, RowSelectionState, SortingState, Updater, flexRender, getCoreRowModel, getPaginationRowModel, getSortedRowModel, useReactTable } from "@tanstack/react-table"
import { ChangeEvent, ReactElement, useEffect, useState } from "react"
import { Input } from "../input"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "../table"
import { DataTableFooter } from "./data-table-footer"

declare module '@tanstack/react-table' {
	interface ColumnMeta<TData extends RowData, TValue> {
		headerClassName?: string;
		cellClassName?: string;
	}
}

interface DataTableProps<TData, TValue> {
	columns: ColumnDef<TData, TValue>[]
	data: PagedList<TData>,
	className?: string,
	showRowCount?: boolean,
	showPageCount?: boolean,
	showPageLinks?: boolean,
	showFilter?: boolean,
	padEndRows?: boolean,
	allowGoToPage?: boolean,
	urlFilter?: string | null,
	tableBodyClassName?: string,
	tableHeadClassName?: string,
	tableRowClassName?: string,
	updatePaging?: (paging: Paging) => void,
	renderTableActions?: () => ReactElement;
	setRowSelection?: (updater: Updater<RowSelectionState>) => void;
	onRowSelect?: (row: TData, selected: boolean) => void;
}

export function DataTable<TData, TValue>({ columns, data, className, tableBodyClassName, tableHeadClassName, tableRowClassName, showRowCount, showPageCount, showPageLinks, allowGoToPage, updatePaging, urlFilter, renderTableActions, padEndRows, showFilter, onRowSelect }: DataTableProps<TData, TValue>) {

	const paging = data.paging || {};

	const [sorting, setSorting] = useState<SortingState>([])
	const [typedFilter, setTypedFilter] = useState(null as string | null);
	const [rowSelection, setRowSelection] = useState<RowSelectionState>({});
	const pageNo = paging.page || 1;
	const pageIndex = pageNo - 1;
	const pageSize = paging.pageSize || 10;
	const items = data.items || [];
	const rowCount = data.totalCount || items.length;

	//debounce filter
	useEffect(() => {
		if (typedFilter === null || !updatePaging)
			return;
		const timer = setTimeout(() => { updatePaging({ filter: typedFilter, page: 1 }) }, 500);
		return () => clearTimeout(timer);
	}, [typedFilter])

	function onFilterChange(event: ChangeEvent<HTMLInputElement>): void {
		setTypedFilter(event.target.value ?? "");
	}
	function onSortingChange(updaterOrValue: Updater<SortingState>): void {

		if (updatePaging) {
			const newSortingValue = updaterOrValue instanceof Function ? updaterOrValue(sorting) : updaterOrValue;
			if (newSortingValue.length > 0)
				updatePaging({ sort: newSortingValue[0].id, sortDirection: newSortingValue[0].desc ? "desc" : "asc" });
		}
		setSorting(updaterOrValue);
	}

	const table = useReactTable({
		data: items,
		columns,
		getCoreRowModel: getCoreRowModel(),
		manualPagination: updatePaging ? true : false,
		getPaginationRowModel: updatePaging ? undefined : getPaginationRowModel(),
		autoResetPageIndex: false,
		enableRowSelection: true,
		enableMultiRowSelection: false,
		onRowSelectionChange: setRowSelection,
		//manualSorting: true,
		rowCount: rowCount,
		initialState: {
			pagination: {
				pageSize: pageSize,
				pageIndex: pageIndex
			}
		},
		onSortingChange: onSortingChange,
		getSortedRowModel: getSortedRowModel(),
		state: {
			sorting,
			rowSelection
		}
	})

	const tableActions = renderTableActions ? renderTableActions() : null;
	const currentFilter = typedFilter ?? urlFilter ?? "";
	let rows = table.getRowModel().rows;

	const emptyRows = [];
	if (padEndRows && pageIndex > 0 && rows.length && rows.length > 0) {
		const numOfEmptyRows = pageSize - rows.length;
		for (let i = 0; i < numOfEmptyRows; i++) {
			const rowKey = `empty-${i}`;
			emptyRows.push(<TableRow key={rowKey}><TableCell className="h-4 text-center" colSpan={columns.length} >&nbsp;</TableCell></TableRow>);
		}
	}

	function OnRowClick(event: React.MouseEvent<HTMLTableRowElement, MouseEvent>, row: Row<TData>) {
		const selected = !rowSelection[row.id];
		setRowSelection({ [row.id]: selected });
		if (onRowSelect)
			onRowSelect(row.original, selected);
	}

	const showActionsPanel = tableActions || showFilter;
	return (
		<>
			{showActionsPanel &&
				<div className="mb-1 flex items-center justify-end">
					{showFilter &&
						<Input type="search" value={currentFilter} placeholder="Filter..." className="h-8 w-52" onChange={onFilterChange} />
					}
					{tableActions}
				</div>
			}
			<div className={className}>
				<Table>
					<TableHeader className={tableHeadClassName}>
						{table.getHeaderGroups().map((headerGroup) => (
							<TableRow key={headerGroup.id}>
								{headerGroup.headers.map((header) => {

									let colDef = header.column.columnDef;
									let meta = colDef.meta;
									let className = meta?.headerClassName || "";

									if (colDef.maxSize && colDef.maxSize !== Number.MAX_SAFE_INTEGER)
										className += ` w-${header.getSize()} max-w-${colDef.maxSize}`;

									return (
										<TableHead key={header.id} className={cn(className)} >
											{header.isPlaceholder
												? null
												: flexRender(header.column.columnDef.header, header.getContext())}
										</TableHead>
									)
								})}
							</TableRow>
						))}
					</TableHeader>
					<TableBody className={tableBodyClassName} >
						{rows?.length ? (
							rows.map((row) => (
								<TableRow onClick={(event) => OnRowClick(event, row)} key={row.id} data-state={row.getIsSelected() && "selected"} className={tableRowClassName}>
									{row.getVisibleCells().map((cell) => {
										let className = cell.column.columnDef.meta?.cellClassName || "";
										return (
											<TableCell key={cell.id} className={cn(className)}>
												{flexRender(cell.column.columnDef.cell, cell.getContext())}
											</TableCell>
										);
									})}
								</TableRow>
							))

						) : (
							<TableRow>
								<TableCell colSpan={columns.length} className="h-24 text-center">
									No results.
								</TableCell>
							</TableRow>

						)}
						{emptyRows}
					</TableBody>
				</Table>
			</div>
			<DataTableFooter table={table} showRowCount={showRowCount} showPageCount={showPageCount} showPageLinks={showPageLinks} allowGoToPage={allowGoToPage} updatePaging={updatePaging} />
		</>
	)
}

