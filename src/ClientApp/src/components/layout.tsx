//import { useEffect } from "react";
import { Outlet, useNavigate } from "react-router-dom";
import { User } from "../context/AuthProvider";
import useAuth from "../hooks/useAuth";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
//import useAxiosPrivate from "../hooks/useAxiosPrivate";
import { useEffect, useState } from "react";
import fetchIdentity from "../fechIdentity";
import usePageVisibility from "../hooks/usePageVisibility";
import Footer from "./footer";
import NavBar from "./navbar";

export const LayoutLoader = async () => {
  //const axiosPrivate = useAxiosPrivate();
  return null;
};

const Layout = () => {
  const [wasLoggedIn, setWasLoggedIn] = useState(false);
  const { auth, setAuth } = useAuth();
  const axiosPrivate = useAxiosPrivate();
  const PROFILE_URL = "/ui/auth/identity";
  const navigate = useNavigate();
  const isPageVisible = usePageVisibility();

  useEffect(() => {
    setWasLoggedIn(auth?.user !== null);
  }, []);

  //trigger when isPageVisible changes
  useEffect(() => {
    //fancy shite to call async from useeffect
    (async () => {
      // if isPageVisible, then the user activated the browser tab
      // we don't know how long they were away so we need to check
      // if they are still logged in.
      // called twice in dev mode due to useffect strict mode design
      if (isPageVisible) {
        //updste the identity
        let user = await fetchIdentity();
        setAuth(user);
        //if we were logged in but no longer are, then navigate to home
        //just in case we were on an authenticated page.
        if (!user.user && wasLoggedIn) {
          setWasLoggedIn(false);
          navigate("/");
        }
      } else {
        setWasLoggedIn(auth?.user !== null);
      }
    })();
  }, [isPageVisible]);

  const fetchData = async () => {
    try {
      if (auth?.user) return;

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

      const currentUser: User = {
        user: {
          userName: username,
          email: email,
          emailConfirmed: emailConfirmed,
          roles: roles,
          avatarUrl: avatarUrl,
          twoFactorEnabled: twoFactorEnabled,
        },
      };
      setAuth(currentUser);
    } catch (err) {
      //console.log(err);
      const currentUser: User = {
        user: null,
      };
      setAuth(currentUser);
    }
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
