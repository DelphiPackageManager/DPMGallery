import { createAxiosInitial } from "@/api/axios";
import { PagedList, Paging } from "@/lib/paging";
import { ManagePackageInfo, PagedPackageInfoResult } from "@/types/managePackages";
import { AxiosError, isAxiosError } from "axios";



export async function fetchPackageInfos(paging: Paging, unlisted: boolean): Promise<PagedPackageInfoResult> {

	const axiosInitial = createAxiosInitial();
	const url = unlisted ? '/ui/account/packages/unlisted' : '/ui/account/packages/published'
	try {
		const response = await axiosInitial.get<PagedList<ManagePackageInfo>>(url);
		let packages = response?.data;
		if (!packages)
			return {
				result: packages,
				error: "No data returned from server"
			};

		return {
			result: packages,
			error: null
		};

	} catch (error: any) {
		let message;
		if (isAxiosError(error) && error.response) {
			let axiosErr = error as AxiosError;
			message = axiosErr.message;
		}
		else if (error instanceof Error)
			message = error.message;
		else
			message = String(error);

		return {
			error: message
		};
	}
};