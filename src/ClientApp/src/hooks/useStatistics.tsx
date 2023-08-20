import axios, { AxiosError } from "axios";
import { useCallback, useState } from "react";
import { Statistics } from "../components/statistics/statisticsTypes";

//copy code from PackagesPage so we can reuse on profile page
const useStatistics = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [statistics, setStatistics] = useState<Statistics | null>(null);

  const getStatistics = async (controller: AbortController) => {
    setError(null);
    setLoading(true);
    try {
      const response = await axios.get("/ui/stats", {
        signal: controller.signal,
      });
      setStatistics(response.data);
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

  //note 'as const' is required, otherwise you will see getStatistics is not callable
  return [{ loading, error, statistics }, useCallback(getStatistics, [])] as const;
};

export default useStatistics;
