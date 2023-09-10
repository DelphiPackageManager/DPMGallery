import { useState } from "react";
import axios from "../api/axios";

import { useNavigate } from "react-router-dom";
import PageContainer from "./pageContainer";
import { Button } from "./ui/button";

const FORGOTPWD_URL = "/ui/auth/forgotpassword";

const ForgotPasswordPage = () => {
  const [email, setEmail] = useState("");
  const [submitted, setSubmitted] = useState(false);
  const [errMsg, setErrorMessage] = useState("");
  const navigate = useNavigate();
  ///const [disabled, setDisabled] = useState(true);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      const response = await axios.post(FORGOTPWD_URL, { email: email });
      if (response?.status == 200) {
        setSubmitted(true);
        setTimeout(() => {
          navigate("/");
        }, 10000);
      }
    } catch (err: any) {
      if (!err?.response) {
        if (err.response?.data) {
          setErrorMessage(err.response?.data);
        } else {
          setErrorMessage("Something went wrong on the server. Our admins have been alerted.");
        }
      }
    }
  };

  return (
    <PageContainer>
      <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto lg:py-4">
        <a href="#" className="flex items-center mb-6 text-2xl font-semibold text-gray-800 dark:text-white">
          <img className="w-8 h-8 mr-2" src="/img/dpm32.png" alt="logo"></img>DPM
        </a>

        <div className="w-full bg-white rounded-lg shadow dark:border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
          <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
            <h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 md:text-2xl dark:text-white">Forgot Password</h1>
            <p className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
              {errMsg}
            </p>

            {!submitted && (
              <form className="space-y-4 md:space-y-6" onSubmit={handleSubmit}>
                <div>
                  <label htmlFor="email" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
                    Your email
                  </label>
                  <input
                    type="email"
                    name="email"
                    id="email"
                    className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="name@company.com"
                    required></input>
                </div>
                <Button type="submit">Forgot Password</Button>
              </form>
            )}
            {submitted && (
              <div>
                <p>If an account exists with that email, we sent you an email. If not, well then we didn't</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </PageContainer>
  );
};

export default ForgotPasswordPage;
