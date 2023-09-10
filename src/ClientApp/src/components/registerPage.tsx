import { useEffect, useRef, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import axios from "../api/axios";
import PageContainer from "./pageContainer";

import { NavLink } from "react-router-dom";
import { User } from "../context/AuthProvider";
import useAuth from "../hooks/useAuth";
import { validateEmail } from "../utils";
import { Button } from "./ui/button";

const RegisterPage = () => {
  const errRef = useRef<HTMLParagraphElement>(null);
  const passwordRef = useRef<HTMLInputElement>(null);
  const [errMsg, setErrorMessage] = useState("");

  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from || "/";

  const [user, setUser] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [email, setEmail] = useState("");
  const [disabled, setDisabled] = useState(true);
  const [emailValid, setEmailValid] = useState(false);
  const { login, logout } = useAuth();

  //clear the error when ever the user or pwd changes
  useEffect(() => {
    setErrorMessage("");
  }, [user, password, confirmPassword, email]);

  const handleEmailChanged = async (event: React.ChangeEvent<HTMLInputElement>) => {
    event.preventDefault();
    setEmail(event.target.value);
    const isValid = validateEmail(event.target.value) !== null;
    setEmailValid(isValid);
  };

  const REGISTER_URL = "/ui/auth/register";

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (password !== confirmPassword) {
      setErrorMessage("Passwords do not match");
      passwordRef?.current?.focus();
      return;
    }
    if (password.length < 8 || password.length > 256) {
      setErrorMessage("Passwords must be at least 8 characters (max 256)");
      passwordRef?.current?.focus();
      return;
    }

    try {
      const response = await axios.post(REGISTER_URL, { username: user, email: email, password: password });
      const username = response?.data?.userName;
      const emailAddress = response?.data?.email;
      const emailConfirmed = response?.data?.emailConfirmed;
      const roles = response?.data?.roles;
      const avatarUrl = response?.data?.avatarUrl;
      const twoFactorEnabled = response?.data?.twoFactorEnabled;

      const newUser: User = {
        userName: username,
        email: emailAddress,
        emailConfirmed: emailConfirmed,
        roles: roles,
        avatarUrl: avatarUrl,
        twoFactorEnabled: twoFactorEnabled,
      };
      login(newUser);
      navigate(from, { replace: true });
    } catch (err: any) {
      console.log("error during login");
      console.log(err);
      if (!err?.response) {
        setErrorMessage("No Server Response");
      } else if (err.response?.status === 400) {
        setErrorMessage("Missing Username or Password");
      } else if (err.response?.status === 401) {
        if (err.response?.data) {
          setErrorMessage(err.response?.data);
        } else {
          setErrorMessage("Invalid username or password");
        }
      } else if (err.response?.status === 409) {
        setErrorMessage("An account with either the username or email already exists. Use the forgot password link on the login page.");
      } else {
        setErrorMessage("Create Account Failed : Error code :  " + `${err?.response?.status}`);
      }
      logout();
      errRef.current?.focus();
    }
  };

  return (
    <PageContainer className="">
      <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto lg:py-4">
        <a href="#" className="flex items-center mb-6 text-2xl font-semibold text-gray-800 dark:text-white">
          <img className="w-8 h-8 mr-2" src="/img/dpm32.png" alt="logo"></img>DPM
        </a>
        <h2 className="text-center mb-4">Note : you only need an account if you wish to publish packages!</h2>
        <p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
          {errMsg}
        </p>

        <div className="w-full bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
          <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
            <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">Create an account</h1>
            <form className="space-y-4 md:space-y-6" onSubmit={handleSubmit}>
              <div>
                <label htmlFor="userName" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                  UserName
                </label>
                <input
                  type="text"
                  name="userName"
                  id="userName"
                  className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  onChange={(e) => setUser(e.target.value)}
                  required
                  autoFocus></input>
              </div>
              <div>
                <label htmlFor="email" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                  Your email
                </label>
                <input
                  type="email"
                  name="email"
                  id="email"
                  className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  onChange={(e) => handleEmailChanged(e)}
                  placeholder="name@company.com"
                  required></input>
              </div>
              <div>
                <label htmlFor="password" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                  Password
                </label>
                <input
                  type="password"
                  name="password"
                  id="password"
                  minLength={8}
                  maxLength={256}
                  ref={passwordRef}
                  className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  onChange={(e) => setPassword(e.target.value)}
                  required></input>
              </div>
              <div>
                <label htmlFor="confirm-password" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                  Confirm password
                </label>
                <input
                  type="password"
                  name="confirm-password"
                  id="confirm-password"
                  minLength={8}
                  maxLength={256}
                  className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required></input>
              </div>
              <div className="flex items-start">
                <div className="flex items-center h-5">
                  <input
                    id="terms"
                    aria-describedby="terms"
                    type="checkbox"
                    className="w-4 h-4 border border-gray-300 rounded focus:ring-3 focus:ring-primary-300 dark:bg-gray-700 dark:border-gray-600 dark:focus:ring-primary-600 dark:ring-offset-gray-800"
                    onChange={(e) => setDisabled(!e.target.checked)}
                    required></input>
                </div>
                <div className="ml-3 text-sm">
                  <label htmlFor="terms" className="font-light text-gray-500 dark:text-gray-300">
                    I accept the{" "}
                    <NavLink className="font-medium text-primary-600 hover:underline dark:text-primary-500" to="/policies/terms">
                      Terms and Conditions
                    </NavLink>
                  </label>
                </div>
              </div>
              <Button type="submit" disabled={!emailValid || disabled} className="w-full">
                Create an account
              </Button>
              <p className="text-sm font-light text-gray-500 dark:text-gray-400">
                Already have an account?{" "}
                <NavLink to="/login" replace className="font-medium text-primary-600 hover:underline dark:text-primary-500">
                  Log In here
                </NavLink>
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
              <Button variant="secondary" name="provider" value="Google">
                <svg className="w-4 h-4" fill="currentColor">
                  <use href="#google" />
                </svg>

                <span className="pl-2">Log In with Google</span>
              </Button>
              <Button variant="secondary" name="provider" value="GitHub">
                <svg className="w-5 h-5">
                  <use href="#github" />
                </svg>
                <span className="pl-2">Log In with GitHub</span>
              </Button>
            </div>
          </form>
        </div>
      </div>
    </PageContainer>
  );
};

export default RegisterPage;
