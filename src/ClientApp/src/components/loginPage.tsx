import * as React from "react";
import { useEffect, useRef, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import axios from "../api/axios";
import { AuthContextInterface, User } from "../context/AuthProvider";
import useAuth from "../hooks/useAuth";
import PageContainer from "./pageContainer";

const LoginPage = () => {
  const { setAuth } = useAuth();

  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from || "/";

  const errRef = useRef<HTMLParagraphElement>(null);

  const [user, setUser] = useState("");
  const [pwd, setPwd] = useState("");
  const [errMsg, setErrMsg] = useState("");

  //clear the error when ever the user or pwd changes
  useEffect(() => {
    setErrMsg("");
  }, [user, pwd]);

  const LOGIN_URL = "/ui/auth/login";

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    try {
      const response = await axios.post(LOGIN_URL, { username: user, password: pwd });
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
      setAuth(currentUser);
      navigate(from, { replace: true });
    } catch (err: any) {
      if (!err?.response) {
        setErrMsg("No Server Response");
      } else if (err.response?.status === 400) {
        setErrMsg("Missing Username or Password");
      } else if (err.response?.status === 401) {
        if (err.response?.data) {
          setErrMsg(err.response?.data);
        } else {
          setErrMsg("Invalid username or password");
        }
      } else {
        setErrMsg("Login Failed : Error code :  " + `${err?.response?.status}`);
      }
      const currentUser: User = {
        user: null,
      };
      setAuth(currentUser);
      errRef.current?.focus();
    }
  };

  return (
    <PageContainer className="text-center">
      <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
        {errMsg}
      </p>
      <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto lg:py-4">
        <a href="#" className="flex items-center mb-6 text-2xl font-semibold text-gray-800 dark:text-white">
          <img className="w-8 h-8 mr-2" src="https://flowbite.s3.amazonaws.com/blocks/marketing-ui/logo.svg" alt="logo"></img>DPM
        </a>
        <div className="w-full bg-white rounded-lg shadow-md dark:shadow-none shadow-gray-200 dark:shadow-gray-800 border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
          <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
            <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">Log In to your account</h1>
            <form className="space-y-4 md:space-y-6" onSubmit={handleSubmit}>
              <div>
                <label htmlFor="userName" className="block mb-2 text-sm font-medium text-gray-800 dark:text-white">
                  UserName
                </label>
                <input
                  type="text"
                  name="userName"
                  id="userName"
                  onChange={(e) => setUser(e.target.value)}
                  value={user}
                  className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  required
                  autoFocus></input>
              </div>
              <div>
                <label htmlFor="password" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                  Password
                </label>
                <input
                  type="password"
                  name="password"
                  id="password"
                  onChange={(e) => setPwd(e.target.value)}
                  value={pwd}
                  className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  required></input>
              </div>
              <div className="flex items-center justify-between">
                <div className="flex items-start">
                  <div className="flex items-center h-5">
                    <input
                      id="remember"
                      aria-describedby="remember"
                      type="checkbox"
                      className="w-4 h-4 border border-gray-300 rounded bg-gray-50 focus:ring-3 focus:ring-primary-300 dark:bg-gray-700 dark:border-gray-600 dark:focus:ring-primary-600 dark:ring-offset-gray-800"></input>
                  </div>
                  <div className="ml-3 text-sm">
                    <label htmlFor="remember" className="text-gray-500 dark:text-gray-300">
                      Remember me
                    </label>
                  </div>
                </div>
                <Link to="/forgotpassword" className="text-sm font-medium text-primary-600 hover:underline dark:text-primary-500">
                  Forgot password?
                </Link>
              </div>
              {/* 
              <button
                type="submit"
                className="w-full text-white bg-primary-600 hover:bg-primary-700 focus:ring-4 focus:outline-none focus:ring-primary-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center dark:bg-primary-600 dark:hover:bg-primary-700 dark:focus:ring-primary-800">
                Log In
              </button>
  */}
              <button type="submit" className="w-full btn btn-primary">
                Log In
              </button>
              <p className="text-sm font-light text-gray-500 dark:text-gray-400">
                Don’t have an account yet?{" "}
                <Link to="/createaccount" replace className="font-medium text-primary-600 hover:underline dark:text-primary-500">
                  Create an Account
                </Link>
              </p>
            </form>
          </div>
        </div>
        <div className="pt-3">
          <div className="flex items-center">
            <div className="w-full h-0.5 bg-gray-200 dark:bg-gray-700"></div>
            <div className="px-5 text-center text-gray-500 dark:text-gray-400">or</div>
            <div className="w-full h-0.5 bg-gray-200 dark:bg-gray-700"></div>
          </div>

          <form action="/ui/auth/external" method="POST">
            <div className="flex items-center justify-center gap-4 py-4">
              <button className="btn btn-outline" name="provider" value="Google">
                <svg className="w-4 h-4" fill="currentColor">
                  <use href="#google" />
                </svg>

                <span className="pl-2">Log In with Google</span>
              </button>
              <button className="btn btn-outline" name="provider" value="GitHub">
                <svg className="w-5 h-5">
                  <use href="#github" />
                </svg>
                <span className="pl-2">Log In with GitHub</span>
              </button>
            </div>
          </form>
        </div>
      </div>
    </PageContainer>
  );
};

export default LoginPage;
