import { cn } from "@/lib/utils";
import { ReactNode } from "react";

type PageHeaderProps = {
	title: string,
	children?: ReactNode;
	className?: string;
}

const PageHeader = ({ title, children, className }: PageHeaderProps) => {
	return (
		<div className={cn("mt-1", className)} >
			<h1 className="text-lg font-semibold">{title}</h1>
			{children &&
				<section className="bg-gray-200 p-2 text-base font-normal dark:bg-gray-700">
					{children}
				</section>
			}
		</div>
	);
};
export default PageHeader;