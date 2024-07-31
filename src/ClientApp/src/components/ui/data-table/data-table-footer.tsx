import { Paging } from "@/lib/paging"
import { Table } from "@tanstack/react-table"
import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from "lucide-react"
import { Button } from "../button"
import { Input } from "../input"

interface DataTableFooter<TData> {
    table: Table<TData>,
    showRowCount?: boolean,
    showPageCount?: boolean,
    showPageLinks?: boolean,
    allowGoToPage?: boolean,
    updatePaging?: (paging: Paging) => void
}

export function DataTableFooter<TData>({ table, showRowCount, showPageCount, showPageLinks, allowGoToPage, updatePaging }: DataTableFooter<TData>) {

    // console.log("showRowCount=", showRowCount);
    // console.log("showPageCount=", showPageCount);
    // console.log("showPageLinks=", showPageLinks);
    // console.log("allowGoToPage=", allowGoToPage);
    // console.log("fetchPage=", updatePaging);

    const pageCount = table.getPageCount();
    //const pageCount = Math.ceil(table.getRowCount() / table.getState().pagination.pageSize)
    // console.log("pageCount=", pageCount);

    // console.log("table.getPageCount()=", table.getPageCount());
    // console.log("table.options.pageCount=",  table.options.pageCount);
    // console.log("table.getRowCount()=",  table.getRowCount());
    // console.log("table.getState().pagination.pageSize=",  table.getState().pagination.pageSize);
    // console.log("table.getState().pagination.pageIndex=",  table.getState().pagination.pageIndex );
    const showPaging = pageCount > 1;

    if (!showPaging)
        return null;

    const showFirstAndLastNavigation = showPageLinks && pageCount > 2;
    allowGoToPage = allowGoToPage && pageCount > 2;

    function goToPage(pageNo: number) {
        // console.log("goToPage pageNo=", pageNo);
        table.setPageIndex(pageNo - 1);
        updatePaging && updatePaging({ page: pageNo });
    }

    const currentPage = table.getState().pagination.pageIndex + 1;
    const goToFirstPage = () => goToPage(1);
    const goToLastPage = () => goToPage(pageCount);
    const goToPreviousPage = () => goToPage(currentPage - 1);
    const goToNextPage = () => goToPage(currentPage + 1);
    const canPreviousPage = table.getCanPreviousPage();
    const canNextPage = table.getCanNextPage();
    const numberOfRows = table.getRowModel().rows.length;
    const totalNumberOfRows = table.getRowCount();
    // console.log("numberOfRows=", numberOfRows);
    // console.log("totalNumberOfRows=", totalNumberOfRows);
    // console.log("currentPage=", currentPage);
    // console.log("pageCount=", pageCount);
    // console.log("canPreviousPage=", canPreviousPage);
    // console.log("canNextPage=", canNextPage);

    return (

        <div className="flex items-center justify-end space-x-1 py-4">
            {showRowCount &&
                <div>
                    Showing {numberOfRows.toLocaleString()} of {totalNumberOfRows.toLocaleString()} rows
                </div>
            }
            <div className="flex grow items-center justify-center gap-5">
                {showPageCount &&
                    <div>Page {currentPage.toLocaleString()} of {pageCount.toLocaleString()}</div>
                }
                {showPageCount && allowGoToPage &&
                    <div> | </div>
                }
                {allowGoToPage &&
                    <div className="flex items-center justify-center gap-1 whitespace-nowrap"> Go to page:
                        <Input type="number" value={currentPage} min={1} max={pageCount}
                            onChange={e => {
                                // console.log("page number input on change. e.target.value=", e.target.value);
                                const page = e.target.value ? Number(e.target.value) : 1
                                goToPage(page);
                            }}
                            className="h-8 w-16 rounded border p-1" /></div>
                }
            </div>
            {showPageLinks &&
                <>
                    {showFirstAndLastNavigation &&
                        <Button variant="outline" size="icon" onClick={() => goToFirstPage()} disabled={!canPreviousPage}><ChevronsLeft /></Button>
                    }
                    <Button variant="outline" size="icon" onClick={() => goToPreviousPage()} disabled={!canPreviousPage}><ChevronLeft /></Button>
                    <Button variant="outline" size="icon" onClick={() => goToNextPage()} disabled={!canNextPage}><ChevronRight /></Button>
                    {showFirstAndLastNavigation &&
                        <Button variant="outline" size="icon" onClick={() => goToLastPage()} disabled={!canNextPage}><ChevronsRight /></Button>
                    }
                </>
            }
        </div>

    )
}