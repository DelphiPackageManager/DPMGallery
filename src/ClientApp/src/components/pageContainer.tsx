import React, { ReactNode } from "react";

interface PageContainerProps {
	children: ReactNode;
	className?: string;
}

const PageContainer: React.FC<PageContainerProps> = ({ children, className }) => {
	const classprop = className ? className : "";
	const style = "container mx-auto max-w-7xl px-1 pt-1 flex flex-col  " + `${classprop}`;
	return (
		<div id="pagecontainer" className="flex h-full w-full flex-col bg-white text-base text-gray-900 dark:bg-gray-900 dark:text-gray-50">
			<div id="pagecontainer-inner" className={style}>
				{children}
			</div>
		</div>
	);
};

export default PageContainer;
