import axios, { AxiosError } from "axios";
import { useCallback, useState } from "react";
import { PackageDetailsModel } from "../components/packagesPage/packageTypes";

export type InfoParams = {
  packageId: string | undefined;
  packageVersion: string | undefined;
};

const usePackageDetails = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [packageInfo, setPackageInfo] = useState<PackageDetailsModel | null>(null);

  const getPackageInfo = async ({ packageId, packageVersion }: InfoParams, controller: AbortController) => {
    try {
      setError(null);
      setLoading(true);
      const url = `/ui/packagedetails/${packageId}/${packageVersion}/`;
      const response = await axios.get(url, {
        signal: controller.signal,
        baseURL: "/",
      });
      // await sleep(500); //so we can see loading
      setPackageInfo(response.data);
    } catch (err: any) {
      let message;
      if (axios.isAxiosError(err) && err.response) {
        let axiosErr = err as AxiosError;
        message = axiosErr.message;
      } else message = String(error);
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  return [{ loading, error, packageInfo }, useCallback(getPackageInfo, [])] as const;
};

export default usePackageDetails;
