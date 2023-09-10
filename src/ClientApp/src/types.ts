//NOTE : Must be kept in sync with server and delphi client enums!

export enum CompilerVersion {
  UnknownVersion,
  RSXE2,
  RSXE3,
  RSXE4,
  RSXE5,
  RSXE6,
  RSXE7,
  RSXE8,
  RS10_0,
  RS10_1,
  RS10_2,
  RS10_3,
  RS10_4,
  RS11_0,
}

export function CompilerVersionString(version: CompilerVersion): string {
  if (version == CompilerVersion.UnknownVersion) return "Unknown";

  let nameOf = CompilerVersion[version];
  nameOf = nameOf.slice(2);
  nameOf = nameOf.replace("_", ".");
  return nameOf;
}

//NOTE : Must be kept in sync with server and delphi client enums!
export enum Platform {
  UnknownPlatform,
  Win32,
  Win64,
  WinArm32, //reserved for future use
  WinArm64, //reserved for future use
  OSX32,
  OSX64,
  OSXARM64,
  AndroidArm32,
  AndroidArm64,
  AndroidIntel32, //reserved for future use
  AndroidIntel64, //reserved for future use
  iOS32,
  iOS64, //reserved for future use
  LinuxIntel32, //reserved for future use
  LinuxIntel64,
  LinuxArm32, //reserved for future use
  LinuxArm64, //reserved for future use
}

export function PlatformString(platform: Platform): string {
  switch (platform) {
    case Platform.UnknownPlatform:
      return "Unknown";

    case Platform.Win32:
      return "Windows 32-bit";
    case Platform.Win64:
      return "Windows 64-bit";
    case Platform.WinArm32:
      return "Windows 32-bit ARM";
    case Platform.WinArm64:
      return "Windows 64-bit ARM";

    case Platform.OSX32:
      return "macOS 32-bit";
    case Platform.OSX64:
      return "macOS 64-bit";
    case Platform.OSXARM64:
      return "macOS ARM 64-bit";

    case Platform.AndroidArm32:
      return "Andriod 32-bit ARM";
    case Platform.AndroidArm64:
      return "Andriod 64-bit ARM";
    case Platform.AndroidIntel32:
      return "Andriod 32-bit Intel";
    case Platform.AndroidIntel64:
      return "Andriod 64-bit Intel";

    case Platform.iOS32:
      return "iOS 32-bit";
    case Platform.iOS64:
      return "iOS 64-bit";

    case Platform.LinuxIntel32:
      return "Linux 32-bit";
    case Platform.LinuxIntel64:
      return "Linux 64-bit";
    case Platform.LinuxArm32:
      return "Linux 32-bit ARM";
    case Platform.LinuxArm64:
      return "Linux 64-bit ARM";

    default:
      throw new Error("PlatformString function is out of date with Platform Type");
  }
}

export type ApiKey = {
  id?: number | null;
  userId?: number | null;
  name: string;
  keyValue?: string | null; //only present in newly created or regenerated api keys
  expiresUTC: string;
  globPattern: string | null;
  packageList: string | null;
  revoked: boolean;
  scopes: ApiKeyScopes;
};

export enum ApiKeyScopes {
  none = 0,
  pushNewPackage = 1,
  pushPackageVersion = 2,
  unlistPackage = 4,
}

export enum MemberRole {
  Collaborator = 0,
  Administrator = 1,
}

export type OrganisationMember = {
  id: number;
  userName: string;
  role: MemberRole;
  avatarUrl: string;
};

export type UserOrganisation = {
  id: number;
  name: string;
  email: string;
  userId: number;
  role: MemberRole;
  adminCount: number;
  collaboratorCount: number;
  packageCount: number;
  allowContact: boolean;
  notifyOnPublish: boolean;
  members: Array<OrganisationMember>;
};

export type OrganisationSettings = {
  allowContact: boolean;
  notifyOnPublish: boolean;
};

export type EditableOrganisation = {
  id: number;
  name: string;
  emailAddress: string;
  settings: OrganisationSettings;
  members: OrganisationMember[] | null;
};

export const memberRoleToString = (role: MemberRole): string => {
  switch (role) {
    case MemberRole.Administrator:
      return "Administrator";
    case MemberRole.Collaborator:
      return "Collaborator";
  }
};
