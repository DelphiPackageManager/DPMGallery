/// <reference types="vite-plugin-svgr/client" />
import { cn } from "@/lib/utils";
import { CompilerVersion, CompilerVersionString, CompilerVersionTitle } from "@/types/types";

export type DelphiIconProps = {
	compilerVersion: CompilerVersion;
	className?: string;

}


const DIcon = ({ version, title, className }: { title: string, version: string, className?: string }) => {
	let clsName = cn("flex h-7 w-7 items-center text-white  justify-center rounded-full bg-[#f32735]", className)
	return (
		<div className={clsName} title={title}>
			<span className="p-1 text-xs">{version}</span>
		</div>
	)
	// return (
	// 	<svg
	// 		xmlns="http://www.w3.org/2000/svg"
	// 		fillRule="evenodd"
	// 		strokeLinejoin="round"
	// 		strokeMiterlimit="2"
	// 		clipRule="evenodd"
	// 		className={className}
	// 		viewBox="0 0 100 100"
	// 	>
	// 		<title>{title}</title>
	// 		<circle
	// 			cx="7099"
	// 			cy="798"
	// 			r="80"
	// 			fill="#f32735"
	// 			transform="matrix(.625 0 0 .625 -4386.88 -448.75)"
	// 		></circle>
	// 		<text
	// 			x="122.13"
	// 			y="101.042"
	// 			fill="#fff"
	// 			fontFamily="'ArialNarrow-Bold', 'Arial', sans-serif"
	// 			fontSize="75"
	// 			fontStretch="condensed"
	// 			fontWeight="700"
	// 			transform="translate(-100 -50) matrix(.63756 0 0 .6473 34.137 52.03)"
	// 		>
	// 			{version}
	// 		</text>
	// 	</svg>
	//);

}

const DelphiIcon = ({ compilerVersion, className }: DelphiIconProps) => {
	let clsName = cn('', className);
	let compilerTitle = CompilerVersionTitle(compilerVersion);
	let compilerName = CompilerVersionString(compilerVersion);

	return (<DIcon className={className} version={compilerName} title={compilerTitle} />)
}

export default DelphiIcon;