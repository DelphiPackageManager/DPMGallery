import { ArrowDownIcon, ArrowUpIcon, CaretSortIcon } from "@radix-ui/react-icons"
import { Column, SortDirection } from "@tanstack/react-table"

import { cn } from "@/lib/utils"
import { Button } from "../button"

interface DataTableColumnHeaderProps<TData, TValue>
	extends React.HTMLAttributes<HTMLDivElement> {
	column: Column<TData, TValue>
	title: string
	allowSorting?: boolean
}

export function DataTableColumnHeader<TData, TValue>({ column, title, allowSorting, className }: DataTableColumnHeaderProps<TData, TValue>) {

	if (!column.getCanSort())
		return <div className={cn(className)}>{title}</div>


	const sortDirection = column.getIsSorted();

	const columnHeaderOnClick = (sortDirection: false | SortDirection) => {

		//console.log("columnHeaderOnClick - sortDirection=", sortDirection);
		if (sortDirection === 'asc' || sortDirection === false)
			column.toggleSorting(true);
		else
			column.toggleSorting(false);
	};

	return (
		<div className={cn("flex items-center space-x-2", className)}>
			{allowSorting ? (
				<Button variant="ghost" size="sm" className="-ml-3 h-4 data-[state=open]:bg-accent" onClick={() => columnHeaderOnClick(sortDirection)}>
					<span className="font-semibold">{title}</span>
					{sortDirection === "desc" ? (
						<ArrowDownIcon className="ml-2 h-3 w-3" />
					) : sortDirection === "asc" ? (
						<ArrowUpIcon className="ml-2 h-3 w-3" />
					) : (
						<CaretSortIcon className="ml-2 h-3 w-3" />
					)}
				</Button>)
				: (
					<span>{title}</span>
				)}
		</div>
	);
}
