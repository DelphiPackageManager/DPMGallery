import axios from "@/api/axios";
import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import PageContainer from "../components/pageContainer";
import { Button } from "../components/ui/button";
import { User } from "../context/AuthProvider";
import useAuth from "../hooks/useAuth";

const LoginWith2faPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const from = location?.state?.from || "/";
  const rememberMe = location?.state?.rememberMe || false;
  const [errMsg, setErrMsg] = useState("");
  const [code, setCode] = useState("");
  const [rememberMachine, setRememberMachine] = useState(true);
  const { login, logout } = useAuth();

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      const state = {
        code: code,
        rememberMe: rememberMe,
        rememberMachine: rememberMachine,
      };
      console.log(state);

      const response = await axios.post("/ui/auth/login-2fa", state);
      if (response?.data?.lockedOut) {
        navigate("/lockedout");
        return;
      }
      const username = response?.data?.userName;
      const email = response?.data?.email;
      const emailConfirmed = response?.data?.emailConfirmed;
      const roles = response?.data?.roles;
      const avatarUrl = response?.data?.avatarUrl;
      const twoFactorEnabled = response?.data?.twoFactorEnabled;

      const loginUser: User = {
        userName: username,
        email: email,
        emailConfirmed: emailConfirmed,
        roles: roles,
        avatarUrl: avatarUrl,
        twoFactorEnabled: twoFactorEnabled,
      };
      login(loginUser);
      navigate(from, { replace: true });
      //
    } catch (err: any) {
      if (!err?.response) {
        setErrMsg("No Server Response");
      } else if (err.response?.status === 400) {
        if (err.response?.data) {
          setErrMsg(err.response?.data);
        } else {
          setErrMsg("Login Failed : Error code :  " + `${err?.response?.status}`);
        }
      } else if (err.response?.status === 401) {
        if (err.response?.data) {
          setErrMsg(err.response?.data);
        } else {
          setErrMsg("Login failed, access denied");
        }
      } else {
        setErrMsg("Login Failed : Error code :  " + `${err?.response?.status}`);
      }
      logout();
      // errRef.current?.focus();
    }
  };

  const onInput = (event: React.ChangeEvent<HTMLInputElement>) => {
    // event.preventDefault();
    setCode(event.target.value);
  };

  return (
    <PageContainer className="">
      <h1>Two-factor authentication</h1>
      <hr className="my-2" />
      <form onSubmit={handleSubmit}>
        <div className="flex flex-col justify-items-start">
          <p>Your login is protected with an authenticator app. Enter your authenticator code below.</p>
          <div className="form-floating my-3">
            <input
              id="code"
              name="code"
              onChange={onInput}
              value={code}
              className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
              autoComplete="off"
              placeholder="Please enter the code."
            />
            <label htmlFor="twofactorcode" className="form-label"></label>
          </div>
          <input id="rememberMe" type="hidden" value={rememberMe} />
          <div className="checkbox my-3">
            <input
              type="checkbox"
              id="rememberMachine"
              name="rememberMachine"
              checked={rememberMachine}
              onChange={(e) => setRememberMachine(e.target.checked)}
            />
            <label className="form-label" htmlFor="remembermachine">
              <span>&nbsp; Remember this Machine</span>
            </label>
          </div>
          <div>
            <Button type="submit" variant="default" className="w-100" disabled={!code}>
              Log in
            </Button>
          </div>
          {errMsg !== "" && (
            <div className="mt-2 errmsg" role="alert">
              {errMsg}
            </div>
          )}
        </div>
      </form>
    </PageContainer>
  );
};

export default LoginWith2faPage;
