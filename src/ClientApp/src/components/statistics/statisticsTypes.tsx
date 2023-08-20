export type PackageDownloads = {
  packageId: string;
  downloads: number;
};

export type VersionDownloads = {
  packageId: string;
  version: string;
  downloads: number;
};

export type Statistics = {
  totalDownloads: number;
  uniquePackages: number;
  packageVersions: number;
  lastUpdated: string;
  topPackageDownloads: PackageDownloads[];
  topVersionDownloads: VersionDownloads[];
};
