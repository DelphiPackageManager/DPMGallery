import {useState} from 'react'

const usePackageDetails = () => { 
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    //const [packages, setPackages] = useState<PackageSearchResult | null>(null);

}

export default usePackageDetails;