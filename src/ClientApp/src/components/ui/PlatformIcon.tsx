import { cn } from "@/lib/utils";
import { Platform } from "@/types/types";


import WindowsLogo from '@/assets/Windows.svg?react';

export type PlatformIconProps = {
	platform: Platform;
	className?: string;
}

const PlatformIcon = ({ platform, className }: PlatformIconProps) => {
	let clsName = cn('', className);



	return (<div>platform</div>)

}

export default PlatformIcon;