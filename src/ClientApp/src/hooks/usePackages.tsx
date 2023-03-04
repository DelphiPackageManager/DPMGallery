import axios, { AxiosError } from "axios";
import { useCallback, useState } from "react";
import { PackageSearchResult } from "../components/packagesPage/packageTypes";

export type SearchParams = {
  query?: string;
  page?: number;
};

const sleep = (ms: number) => new Promise((resolve) => setTimeout(resolve, ms));

//copy code from PackagesPage so we can reuse on profile page
const usePackages = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [packages, setPackages] = useState<PackageSearchResult | null>(null);

  const getPackages = async (params: SearchParams, controller: AbortController) => {
    try {
      setError(null);
      setLoading(true);

      const queryParams = new URLSearchParams();
      if (params.query) {
        queryParams.append("q", params.query);
      }

      if (params.page && params.page > 1) {
        queryParams.append("page", params.page.toString());
      }
      const response = await axios.get("/ui/packages", {
        signal: controller.signal,
        params: queryParams,
      });
      // await sleep(500); //so we can see loading
      setPackages(response.data);
    } catch (err: any) {
      let message;
      if (axios.isAxiosError(error) && error.response) {
        let axiosErr = err as AxiosError;
        message = axiosErr.message;
      } else message = String(error);
      setError(message);
      //console.error(err)
    } finally {
      setLoading(false);
    }
  };
  //note 'as const' is required, otherwise you will see getPackages is not callable
  return [{ loading, error, packages }, useCallback(getPackages, [])] as const;
};

export default usePackages;
