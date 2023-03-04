import axios from "../api/axios";
import { User } from "../context/AuthProvider";
import useAuth from "./useAuth";

const useRefreshToken = () => {
  const refresh = async () => {
    console.log("refreshing token");
    const response = await axios.post("/ui/auth/refresh-token", {
      withCredentials: true, //send cookies
    });
    return response;
  };
  return refresh;
};

export default useRefreshToken;
