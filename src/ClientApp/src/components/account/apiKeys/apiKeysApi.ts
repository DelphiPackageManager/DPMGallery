import { createAxiosInitial, errorToResult } from "@/api/axios";
import { PagedList, Paging } from "@/lib/paging";
import { ApiResult } from "@/types/api";
import { ApiKey, ApiKeyCreateModel, ApiKeyRegenerateModel, PagedApiKeysResult } from "@/types/apiKeys";
import { SortDirection } from "@/types/types";

import { AxiosError, isAxiosError } from "axios";


export async function fetchApiKeys(paging: Paging): Promise<PagedApiKeysResult> {

	try {

		const pageSize = paging.pageSize ?? 3;
		const page = paging.page ?? 1;
		const sort = paging.sort ?? "name";
		const sortDirection = paging.sortDirection ?? SortDirection.Asc;
		const filter = paging.filter ?? "";

		const axiosInitial = createAxiosInitial();

		const url = "/ui/account/apikeys";
		const response = await axiosInitial.get<PagedList<ApiKey>>(url, { params: { pageSize: pageSize, page: page, sort: sort, sortDirection: sortDirection, filter: filter } });
		let apiKeys = response?.data;
		if (!apiKeys)
			return {
				result: apiKeys,
				error: "No data returned from server"
			};

		return {
			result: apiKeys,
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

		return { error: message };

	} finally {
		//setLoading(false);
	}
}

export async function createApiKey(user: ApiKeyCreateModel): Promise<ApiResult> {

	try {

		const axiosInitial = createAxiosInitial();

		const url = `/ui/account/apikeys`;
		const response = await axiosInitial.post<ApiResult>(url, user);
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

	} finally {
		//setLoading(false);
	}
}

export async function deleteApiKey(id: number): Promise<ApiResult> {

	try {


		const axiosInitial = createAxiosInitial();
		const url = `/ui/account/apikeys/${id}`;
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

	} finally {
		//setLoading(false);
	}
}

export async function updateApiKeyEnabled(enabled: boolean, apiKeyId: number): Promise<ApiResult> {

	try {


		const axiosInitial = createAxiosInitial();
		const url = `/ui/account/apikeys/${apiKeyId}`;
		const response = await axiosInitial.put<ApiResult>(url, enabled);
		let result = response?.data;
		if (!result)
			result = {
				succeeded: false,
				errors: ["No data returned from server"]
			};

		return result;

	} catch (error: any) {
		return errorToResult(error);

	} finally {
		//setLoading(false);
	}
}


export async function regenerateApiKey(model: ApiKeyRegenerateModel): Promise<ApiResult> {
	try {
		const axiosInitial = createAxiosInitial();
		const url = `/ui/account/apikeys/${model.id}/regenerate`;
		const expiresInDays = model.changeExpiry ? model.expiresInDays : null;

		const response = await axiosInitial.put<ApiResult>(url, expiresInDays);
		let result = response?.data;
		if (!result)
			result = {
				succeeded: false,
				errors: ["No data returned from server"]
			};

		return result;

	} catch (error: any) {
		return errorToResult(error);

	} finally {
		//setLoading(false);
	}
}