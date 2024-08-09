import { PagedList } from "@/lib/paging";

export enum ApiKeyScope {
	none = 0,
	pushPackageVersion = 1 << 0, //001 = 1
	pushNewPackage = 1 << 1, //010 = 2
	unlistPackage = 1 << 2, //100 = 4
	All = ~(~0 << 3)   // 111 = 7
}

export const apiKeyScopesToString = (value: ApiKeyScope): string => {

	if (value & ApiKeyScope.All) {

		return "New package, new versions, unlist";

	}

	let result = null;
	if (value & ApiKeyScope.pushNewPackage) {
		result = "New package";
	}
	if (value & ApiKeyScope.pushNewPackage) {
		if (result)
			result = result + ", new versions";
	}
	if (value & ApiKeyScope.pushNewPackage) {
		if (result)
			result = result + ", new versions"
		else
			result = "New versions"
	}
	if (value & ApiKeyScope.unlistPackage) {
		if (result)
			result = result + ", unlist"
		else
			result = "Unlist";
	}

	return result ? result : "no scopes set";
}

export type ApiKey = {
	id?: number | null;
	userId?: number | null;
	name: string;
	key?: string | null; //only present in newly created or regenerated api keys
	expiresUTC: string;
	globPattern: string | null;
	packageList: string | null;
	revoked: boolean;
	packageOwner: string;
	scopes: ApiKeyScope;
};

export type PagedApiKeysResult = {
	result?: PagedList<ApiKey> | null;
	error: string | null;
}



export type ApiKeyCreateModel = {
	name: string;
	expiresInDays: number;
	packageOwner: number;
	globPattern: string | null;
	packages: string | null;
	scopes: ApiKeyScope;
};


export type ApiKeyRegenerateModel = {
	id: number;
	expiresInDays: number;
	changeExpiry: boolean;
};