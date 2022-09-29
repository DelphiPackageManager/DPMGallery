import * as React from 'react';

interface IUploadPageProps {
}

const UploadPage: React.FunctionComponent<IUploadPageProps> = (props) => {
  return (
      <div className="w-full pt-4">
          <div className="container text-base text-gray-600 dark:text-gray-300 pb-2 mx-auto max-w-7xl">
          <h1>Uploads</h1>
          </div>    
      </div>
  );
};

export default UploadPage;
  