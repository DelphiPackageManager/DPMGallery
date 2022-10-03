import { CompilerVersion, Platform } from "../../types";

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

export type PackageVersionModel = {
    version  : string
    publishedutc : string;
    published : string;
    downloads : number;
}

export type PackageDependencyModel = {
    packageId : string,
    versionRange  : string;
}

export type PackageDetailPlatformModel = {
    platform : Platform,
    dependencies : PackageDependencyModel[],
    downloadUrl : string;
}

export type  PackageDetailCompilerModel = {
    compilerVersion : CompilerVersion,
    platforms : PackageDetailPlatformModel[]
}


export type PackageDetailsModel = {        
    packageId : string,
    packageName : string,
    packageVersion : string,
    prefixReserved : boolean,
    isPrerelease : boolean,
    isLatestVersion : boolean,
    isCommercial : boolean,
    isTrial : boolean,
    icon : string,
    publishedUtc : string,
    published: string,
    versions : PackageVersionModel[],
    compilerPlatforms : PackageDetailCompilerModel[],
    projectUrl : string,
    readMe : string,
    repositoryUrl : string,
    licenses : string[]
    owners : string[],
    ownerMD5s : string[],
    totalDownloads : number,
    currentVersionDownload : number,
    tags : string[], 
}
