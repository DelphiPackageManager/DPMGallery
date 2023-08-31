//import { useEffect } from "react";
import { useEffect, useState } from "react";
import { Outlet, useNavigate } from "react-router-dom";
import { User } from "../context/AuthProvider";
import fetchIdentity from "../fechIdentity";
import useAuth from "../hooks/useAuth";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import { useDidMount } from "../hooks/useDidMount";
import usePageVisibility from "../hooks/usePageVisibility";
import Footer from "./footer";
import NavBar from "./navbar";

export const LayoutLoader = async () => {
  //const axiosPrivate = useAxiosPrivate();
  return null;
};

const Layout = () => {
  const [wasLoggedIn, setWasLoggedIn] = useState(false);
  const { currentUser, login, logout } = useAuth();
  const axiosPrivate = useAxiosPrivate();
  const PROFILE_URL = "/ui/auth/identity";
  const navigate = useNavigate();
  const isPageVisible = usePageVisibility();
  const didMount = useDidMount();
  useEffect(() => {
    if (didMount) return;

    setWasLoggedIn(currentUser !== null);
  }, []);

  //trigger when isPageVisible changes
  useEffect(() => {
    //stop this running twice as it is causing issues
    if (didMount) return;

    //fancy shite to call async from useeffect
    (async () => {
      // if isPageVisible, then the user activated the browser tab
      // we don't know how long they were away so we need to check
      // if they are still logged in.
      // called twice in dev mode due to useffect strict mode design
      if (isPageVisible) {
        //update the identity
        let user = await fetchIdentity();
        login(user);
        //if we were logged in but no longer are, then navigate to home
        //just in case we were on an authenticated page.
        if (!currentUser && wasLoggedIn) {
          setWasLoggedIn(false);
          navigate("/");
        }
      } else {
        setWasLoggedIn(currentUser !== null);
      }
    })();
  }, [isPageVisible]);

  const fetchData = async () => {
    try {
      if (currentUser !== null) return;

      //using axios private so we get the new refresh token.
      const response = await axiosPrivate.post(
        PROFILE_URL,
        {},
        {
          withCredentials: true,
        }
      );
      if (!response.data) {
        return;
      }

      const username = response?.data?.userName;
      const email = response?.data?.email;
      const emailConfirmed = response?.data?.emailConfirmed;
      const roles = response?.data?.roles;
      const avatarUrl = response?.data?.avatarUrl;
      const twoFactorEnabled = response?.data?.twoFactorEnabled;

      const user: User = {
        userName: username,
        email: email,
        emailConfirmed: emailConfirmed,
        roles: roles,
        avatarUrl: avatarUrl,
        twoFactorEnabled: twoFactorEnabled,
      };
      login(user);
    } catch (err) {
      logout();
    }
  };

  return (
    <>
      <div className="flex flex-col m-0 h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100">
        <NavBar />
        <div className="flex flex-col flex-grow mt-[3.5rem]">
          <Outlet />
        </div>
        <Footer />
      </div>
    </>
  );
};

export default Layout;
