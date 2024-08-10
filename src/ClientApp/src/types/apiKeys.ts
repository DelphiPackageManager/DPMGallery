import { PagedList } from "@/lib/paging";

export enum ApiKeyScope {
	none = 0,
	pushPackageVersion = 1,
	pushNewPackage = 3,
	unlistPackage = 4,
}

export const apiKeyScopesToString = (value: ApiKeyScope): string => {

	let result = null;
	if ((value & ApiKeyScope.pushNewPackage) == ApiKeyScope.pushNewPackage) {
		result = "Push new packages and package version";
	}
	if ((value & ApiKeyScope.pushPackageVersion) == ApiKeyScope.pushPackageVersion) {
		if (!result)
			result = "Push only new package versions"
		// else
		// 	result = "Push only new package versions";
	}
	if ((value & ApiKeyScope.unlistPackage) == ApiKeyScope.unlistPackage) {
		if (result)
			result += ", Unlist or relist package versions"
		else
			result = "Unlist or relist package versions";
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
	packages: string | null;
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