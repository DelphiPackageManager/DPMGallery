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

export type PackageDetailsModel = {
    packageId : string
    packageName : string;
    packageVersion : string;
    prefixReserved : boolean;

    isPrerelease : boolean;

    isLatestVersion : boolean;

    isCommercial : boolean;

    isTrial : boolean;

    publishedUtc : string;

    published : string; //prettydate

    platform : Platform[];

    compilerVersions :  CompilerVersion[];

    versions : PackageVersionModel[];

    projectUrl : string 

    readMe : string;
    repositoryUrl : string;

    licenses : string[];

    owners : string[];

    totalDownloads : number;

    currentVersionDownload : number;

    tags : string[] ;

    //Record<T,S>
    //CompilerVersion, List<Platform>> CompilerPlatforms : Dictionary<;

    //CompilerVersion, Dictionary<Platform, List<PackageDependencyModel>>> PackageDependencies : Dictionary;
   
}
