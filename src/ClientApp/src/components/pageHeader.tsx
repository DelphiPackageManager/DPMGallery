import { ReactNode } from "react";

type PageHeaderProps = {
	title: string,
	children?: ReactNode;
}

const PageHeader = ({ title, children }: PageHeaderProps) => {



	return (
		<div className="mx-2 mt-1">
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