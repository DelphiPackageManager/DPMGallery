import React from 'react';
import PageContainer from '../pageContainer'
import { useParams } from 'react-router-dom'

type PackagePageProps = {
    //dasdasd: string;
};

const PackagePage: React.FC<PackagePageProps> = ({  }) => {
    let { packageId, packageVersion } = useParams();


    return (
        <PageContainer>
            <h1>Package {packageId} - {packageVersion} </h1>
        </PageContainer>
    );
};

export default PackagePage;
