import React, { ReactNode } from 'react';

interface PageContainerProps {
    children : ReactNode;
    className? : string
}

const PageContainer: React.FC<PageContainerProps> = ({children, className} ) => {

    const style = "container pb-2 mx-auto max-w-5xl " + `${className}`;
    return (
        <div className="w-full pt-6 px-2 md:px-1 bg-white dark:bg-gray-900 text-base text-gray-900 dark:text-gray-50">
            <div className={style}>
                {children}
            </div>
        </div>
    );
};

export default PageContainer;
