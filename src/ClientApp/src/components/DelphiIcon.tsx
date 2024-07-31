/// <reference types="vite-plugin-svgr/client" />
import { CompilerVersion } from "@/types/types";

import D10 from '@/assets/Delphi-10.0.svg?react';
import D10_1 from '@/assets/Delphi-10.1.svg?react';
import D10_2 from '@/assets/Delphi-10.2.svg?react';
import D10_3 from '@/assets/Delphi-10.3.svg?react';
import D10_4 from '@/assets/Delphi-10.4.svg?react';
import D11 from '@/assets/Delphi-11.x.svg?react';
import D12 from '@/assets/Delphi-12.x.svg?react';
import XE2 from '@/assets/Delphi-XE2.svg?react';
import XE3 from '@/assets/Delphi-XE3.svg?react';
import XE4 from '@/assets/Delphi-XE4.svg?react';
import XE5 from '@/assets/Delphi-XE5.svg?react';
import XE6 from '@/assets/Delphi-XE6.svg?react';
import XE7 from '@/assets/Delphi-XE7.svg?react';
import XE8 from '@/assets/Delphi-XE8.svg?react';
import { cn } from "@/lib/utils";

export type DelphiIconProps = {
	compilerVersion: CompilerVersion;
	className?: string;
}

const DelphiIcon = ({ compilerVersion, className }: DelphiIconProps) => {
	let clsName = cn('', className);

	switch (compilerVersion) {
		case CompilerVersion.RSXE2:
			return (<XE2 className={clsName} />)
		case CompilerVersion.RSXE3:
			return (<XE3 className={clsName} />)
		case CompilerVersion.RSXE4:
			return (<XE4 className={clsName} />)
		case CompilerVersion.RSXE5:
			return (<XE5 className={clsName} />)
		case CompilerVersion.RSXE6:
			return (<XE6 className={clsName} />)
		case CompilerVersion.RSXE7:
			return (<XE7 className={clsName} />)
		case CompilerVersion.RSXE8:
			return (<XE8 className={clsName} />)
		case CompilerVersion.RS10_0:
			return (<D10 className={clsName} />)
		case CompilerVersion.RS10_1:
			return (<D10_1 className={clsName} />)
		case CompilerVersion.RS10_2:
			return (<D10_2 className={clsName} />)
		case CompilerVersion.RS10_3:
			return (<D10_3 className={clsName} />)
		case CompilerVersion.RS10_4:
			return (<D10_4 className={clsName} />)
		case CompilerVersion.RS11_0:
			return (<D11 className={clsName} />)
		case CompilerVersion.RS12_0:
			return (<D12 className={clsName} />)
		default:
			throw new Error(`Invalid compilerVerison ${compilerVersion}`);
	}

}

export default DelphiIcon;