import * as React from 'react';
import PageContainer from './pageContainer';

interface IDownloadsPageProps {
}

const DownloadsPage: React.FunctionComponent<IDownloadsPageProps> = (props) => {
  return (
    <PageContainer>
       <h1>Downloads</h1>
    </PageContainer>
  )
};

export default DownloadsPage;
