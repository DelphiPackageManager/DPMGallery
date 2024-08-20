import { futimes } from "fs";

export enum SortDirection {
	Default,
	Asc,
	Desc
}


//NOTE : Must be kept in sync with server and delphi client enums!

export enum CompilerVersion {
	UnknownVersion,
	RSXE2 = "RSXE2",
	RSXE3 = "RSXE3",
	RSXE4 = "RSXE4",
	RSXE5 = "RSXE5",
	RSXE6 = "RSXE6",
	RSXE7 = "RSXE7",
	RSXE8 = "RSXE8",
	RS10_0 = "RS10_0",
	RS10_1 = "RS10_1",
	RS10_2 = "RS10_2",
	RS10_3 = "RS10_3",
	RS10_4 = "RS10_4",
	RS11_0 = "RS11_0",
	RS12_0 = "RS12_0"
}

export function StrToCompilerVersion(value: string): CompilerVersion {
	let lversion = "RS" + value.replace('.', '_');
	return lversion as CompilerVersion;
	//return CompilerVersion.UnknownVersion;
}



export function CompilerVersionTitle(version: CompilerVersion): string {
	if (version == CompilerVersion.UnknownVersion) return "Unknown";

	let nameOf = CompilerVersion[version];
	let result = nameOf.slice(2);
	result = "RAD Studio " + result.replace("_", ".");
	return result;
}


export function CompilerVersionString(version: CompilerVersion): string {
	if (version == CompilerVersion.UnknownVersion) return "Unknown";

	let nameOf = CompilerVersion[version];
	let result = nameOf.slice(2);
	result = result.replace("_", ".");
	return result;
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
			console.log("unknown platform")
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

export function StrToPlatform(value: string): Platform {
	switch (value) {
		case "UnknownPlatform":
			return Platform.UnknownPlatform;
		case "Win32":
			return Platform.Win32;
		case "Win64":
			return Platform.Win64;
		case "WinArm32":
			return Platform.WinArm32;
		case "WinArm64":
			return Platform.WinArm64;

		case "OSX32":
			return Platform.OSX32;
		case "OSX64":
			return Platform.OSX64;
		case "OSXARM64":
			return Platform.OSXARM64

		case "Android":
		case "AndroidArm32":
			return Platform.AndroidArm32;
		case "Android64":
		case "AndroidArm64":
			return Platform.AndroidArm64;
		case "AndroidIntel32":
			return Platform.AndroidIntel32;
		case "AndroidIntel64":
			return Platform.AndroidIntel64;

		case "iOS32":
			return Platform.iOS32;
		case "iOS64":
			return Platform.iOS64;

		case "LinuxIntel32":
			return Platform.LinuxIntel32;
		case "LinuxIntel64":
			return Platform.LinuxIntel64;
		case "LinuxArm32":
			return Platform.LinuxArm32;
		case "LinuxArm64":
			return Platform.LinuxArm64;

		default:
			return Platform.UnknownPlatform;

	}
}