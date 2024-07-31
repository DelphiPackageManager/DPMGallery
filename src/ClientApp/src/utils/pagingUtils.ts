import { Paging } from "@/lib/paging";
import { SetURLSearchParams } from "react-router-dom";

export function updatePagingUsingSearchParams(searchParams: URLSearchParams, setSearchParams: SetURLSearchParams, fetchTableContent: () => () => void) {
	return (paging: Paging, fetchContent?: boolean) => {

		let changed = false;
		if (paging.page) {
			const prevPage = searchParams.get("page");
			if (prevPage !== paging.page.toString()) {
				searchParams.set("page", paging.page.toString());
				changed = true;
			}
		}
		if (paging.sort) {
			const prevSort = searchParams.get("sort");
			const lcSort = paging.sort.toLowerCase();
			if (prevSort !== lcSort) {
				searchParams.set("sort", lcSort);
				changed = true;
			}

			//sortDirection and sort are always updated at the same time
			if (paging.sortDirection) {
				const prevSortDirection = searchParams.get("sortDirection");
				const lcSortDirection = paging.sortDirection.toLowerCase();
				if (prevSortDirection !== lcSortDirection) {
					searchParams.set("sortDirection", lcSortDirection);
					changed = true;
				}
			}
		}
		if (paging.filter) {
			const prevFilter = searchParams.get("filter");
			const lcFilter = paging.filter.toLowerCase();
			if (prevFilter !== lcFilter) {
				searchParams.set("filter", lcFilter);
				changed = true;
			}
		}
		if (paging.filter == "" && searchParams.has("filter")) {
			searchParams.delete("filter");
			changed = true;
		}

		if (!changed)
			return;

		setSearchParams(searchParams);

		if (fetchContent === undefined || fetchContent)
			fetchTableContent();

	};
}
