import React, { ReactNode } from 'react';

interface PageContainerProps {
    children : ReactNode;
    className? : string
}

const PageContainer: React.FC<PageContainerProps> = ({children, className} ) => {

    const style = "container text-base text-gray-600 dark:text-gray-300 pb-2 mx-auto max-w-6xl " + `${className}`;

    return (
        <div className="w-full pt-4">
            <div className={style}>
                {children}
            </div>
        </div>
    );
};

export default PageContainer;
