import { PagedList } from "@/lib/paging";

export type ManagePackageInfo = {
	packageId: string;
	owners: string[],
	downloads: number,
	latestVersion: string;
}

export type PagedPackageInfoResult = {
	result?: PagedList<ManagePackageInfo> | null;
	error: string | null;
}