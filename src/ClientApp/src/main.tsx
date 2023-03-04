import React from "react";
import ReactDOM from "react-dom/client";
import { createAxiosInitial } from "./api/axios";
import App from "./components/App";
import { AuthProvider, User } from "./context/AuthProvider";

import useRefreshToken from "./hooks/useRefreshToken";
import "./index.css";

const refresh = useRefreshToken();

const fetchData = async () => {
  const PROFILE_URL = "/ui/auth/profile";
  const axiosInitial = createAxiosInitial();
  try {
    //using axios private so we get the new refresh token.
    const response = await axiosInitial.post(
      PROFILE_URL,
      {},
      {
        withCredentials: true,
      }
    );
    if (!response.data) {
      return {
        user: null,
      };
    }

    const username = response?.data?.userName;
    const email = response?.data?.email;
    const emailConfirmed = response?.data?.emailConfirmed;
    const roles = response?.data?.roles;
    const avatarUrl = response?.data?.avatarUrl;

    const result = {
      user: {
        userName: username,
        email: email,
        emailConfirmed: emailConfirmed,
        roles: roles,
        avatarUrl: avatarUrl,
      },
    };
    return result;
  } catch (err) {
    return {
      user: null,
    };
  }
};

const user = await fetchData();

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <AuthProvider user={user}>
      <App />
    </AuthProvider>
  </React.StrictMode>
);
