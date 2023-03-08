import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { User } from "../context/AuthProvider";
import useAuth from "../hooks/useAuth";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import PageContainer from "./pageContainer";

const LoginWith2faPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from || "/";
  const rememberMe = location.state?.rememberMe || false;
  const [errMsg, setErrMsg] = useState("");
  const [code, setCode] = useState("");
  const { setAuth } = useAuth();
  const axiosPrivate = useAxiosPrivate();

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      const response = await axiosPrivate.post("/ui/auth/login-2fa", { code: code });

      //
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
      // errRef.current?.focus();
    }
  };

  return (
    <PageContainer className="text-center">
      <h1>Two-factor authentication</h1>
      <hr />
      <p>Your login is protected with an authenticator app. Enter your authenticator code below.</p>
      <form onSubmit={handleSubmit}>
        <div className="form-floating mb-3">
          <input
            id="twofactorcode"
            name="twofactorcode"
            className="form-control"
            autoComplete="off"
            value={code}
            onChange={(e) => setCode(e.target.value)}
          />
          <label htmlFor="twofactorcode" className="form-label"></label>
          <span asp-validation-for="Input.TwoFactorCode" className="text-danger"></span>
        </div>
        <div className="checkbox mb-3">
          <label className="form-label" htmlFor="remembermachine">
            <input type="checkbox" id="remembermachine" name="remembermachine" />
            <span>Remember this Machine</span>
          </label>
        </div>
        <div>
          <button type="submit" className="w-100 btn btn-primary" disabled={!code}>
            Log in
          </button>
        </div>
        {errMsg !== "" && (
          <div className="text-danger" role="alert">
            {errMsg}
          </div>
        )}
      </form>
    </PageContainer>
  );
};

export default LoginWith2faPage;
