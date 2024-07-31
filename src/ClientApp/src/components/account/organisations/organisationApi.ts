import { createAxiosInitial, errorToResult } from "@/api/axios";
import { PagedList } from "@/lib/paging";
import { ApiResult } from "@/types/api";
import { UserOrganisation, UserOrganisationCreateModel, UserOrganisationsResult } from "@/types/organisations";
import { AxiosError, isAxiosError } from "axios";


export async function fetchOrganisations(): Promise<UserOrganisationsResult> {

	const axiosInitial = createAxiosInitial();

	try {
		const response = await axiosInitial.get<PagedList<UserOrganisation>>("/ui/account/organisations");
		let orgs = response?.data;
		if (!orgs)
			return {
				result: orgs,
				error: "No data returned from server"
			};

		return {
			result: orgs,
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


export async function createOrganisation(org: UserOrganisationCreateModel): Promise<ApiResult> {

	try {

		const axiosInitial = createAxiosInitial();

		const url = `/ui/account/organisations`;
		const response = await axiosInitial.post<ApiResult>(url, org);
		let data = response?.data;
		let result: ApiResult;
		if (!data) {
			result = {
				succeeded: false,
				errors: ["No data returned from server"]
			};
		}
		else {
			result = {
				succeeded: true,
				data: data,
				errors: []
			};
		}

		return result;

	} catch (error: any) {
		return errorToResult(error);
	}
}


export async function checkOrgNameUnique(orgName: string): Promise<ApiResult> {
	try {
		const axiosInitial = createAxiosInitial();
		const url = `/ui/account/check-unique/${orgName}`;
		const response = await axiosInitial.get<ApiResult>(url);
		let result = response?.data;
		if (!result)
			result = {
				succeeded: false,
				errors: ["No result returned from server"]
			};

		return result;

	} catch (error: any) {
		return errorToResult(error);
	}
}

export async function deleteOrganisation(id: number): Promise<ApiResult> {

	try {
		const axiosInitial = createAxiosInitial();
		const url = `/ui/account/organisations/${id}`;
		const response = await axiosInitial.delete<ApiResult>(url);
		let result = response?.data;
		if (!result)
			result = {
				succeeded: false,
				errors: ["No result returned from server"]
			};

		return result;

	} catch (error: any) {
		return errorToResult(error);

	}
}