import axios from "axios";
import { PackageSearchResult } from "../pages/packagePages/packageTypes";

export type SearchParams = {
  query?: string;
  page?: number;
};

export const getPackages = async (page: number, query: string, signal: AbortSignal): Promise<PackageSearchResult> => {
  const queryParams = new URLSearchParams();
  if (query) {
    queryParams.append("q", query);
  }

  if (page && page > 1) {
    queryParams.append("page", page.toString());
  }
  return await axios
    .get<PackageSearchResult>("/ui/packages", {
      signal: signal,
      params: queryParams,
    })
    .then((response) => response.data);
};

//export default getPackages;
