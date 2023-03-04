import { useEffect } from "react";
import { Outlet } from "react-router-dom";
import { User } from "../context/AuthProvider";
import useAsync from "../hooks/useAsync";
import useAuth from "../hooks/useAuth";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
//import useAxiosPrivate from "../hooks/useAxiosPrivate";
import Footer from "./footer";
import NavBar from "./navbar";

export const LayoutLoader = async () => {
  //const axiosPrivate = useAxiosPrivate();
  return null;
};

const Layout = () => {
  const { auth, setAuth } = useAuth();
  const axiosPrivate = useAxiosPrivate();

  const PROFILE_URL = "/ui/auth/profile";

  const fetchData = async () => {
    console.log("fetching Profile");
    try {
      if (auth) return;

      //using axios private so we get the new refresh token.
      const response = await axiosPrivate.post(
        PROFILE_URL,
        {},
        {
          withCredentials: true,
        }
      );
      if (!response.data) {
        console.log("no data");
        return;
      }

      const username = response?.data?.userName;
      const email = response?.data?.email;
      const emailConfirmed = response?.data?.emailConfirmed;
      const roles = response?.data?.roles;
      const avatarUrl = response?.data?.avatarUrl;

      const currentUser: User = {
        user: {
          userName: username,
          email: email,
          emailConfirmed: emailConfirmed,
          roles: roles,
          avatarUrl: avatarUrl,
        },
      };
      console.log(currentUser);
      setAuth(currentUser);
      console.log("profile is set");
    } catch (err) {}
  };

  return (
    <>
      <div className="flex flex-col m-0 h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100">
        <NavBar />
        <div className="flex-grow mt-[3.5rem]">
          <Outlet />
        </div>
        <Footer />
      </div>
    </>
  );
};

export default Layout;
