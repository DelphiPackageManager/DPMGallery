export type PackageResultItem = {
    published : string,
    packageId: string,
    latestVersion: string
    isCommercial:  boolean,
    isTrial:  boolean,
    isReservedPrefix:  boolean,
    isPrelease : boolean;
    description: string,
    owners: string[],
    icon: string | null,
    hasIcon:  boolean,
    tags: string[] | null,
    totalDownloads: number,
    publishedUtc: string,
    compilerVersions: string[],
    platforms: number[]
};

export type PackageSearchResult = {
    totalPackages : number,
    query : string,
    nextPage : number,
    prevPage : number,
    packages : PackageResultItem[]
};
