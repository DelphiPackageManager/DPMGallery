import * as React from 'react';

interface IDownloadsPageProps {
}

const DownloadsPage: React.FunctionComponent<IDownloadsPageProps> = (props) => {
  return (
    <div className="w-full pt-4">
        <div className="container text-base text-gray-600 dark:text-gray-300 pb-2 mx-auto max-w-7xl">
        <h1>Downloads</h1>
        </div>    
    </div>
  )
};

export default DownloadsPage;
