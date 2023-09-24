import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import PageContainer from "../components/pageContainer";
import useAuth from "../hooks/useAuth";
import useAxiosPrivate from "../hooks/useAxiosPrivate";

const VerifyEmailPage = () => {
  const axios = useAxiosPrivate();
  const { currentUser, login } = useAuth();
  let navigate = useNavigate();
  let [searchParams] = useSearchParams();
  let userName = searchParams.get("userName");
  let code = searchParams.get("code");
  let returnUrl = searchParams.get("returnUrl") || "/";
  let [statusMessage, setStatusMessage] = useState("");
  let [errMsg, SetErrorMessage] = useState<string | null>(null);

  const postConfirmation = async () => {
    try {
      const response = await axios.post("/ui/auth/confirm-email", { userName, code, returnUrl });
      if (response?.status == 200) {
        setStatusMessage("Thank you for verifying your email address.");
        if (currentUser && currentUser.userName.toLowerCase() === userName?.toLowerCase()) {
          currentUser.emailConfirmed = true;
        }

        setTimeout(() => {
          navigate(returnUrl);
        }, 3000);
      }
    } catch (err: any) {
      if (err?.response) {
        if (err.response.statusMessage) {
          SetErrorMessage(err.response.statusMessage);
        } else {
          SetErrorMessage("Error verifying email  :  " + err.response.status.toString());
        }
      }
    }
  };

  useEffect(() => {
    if (!code || !userName) {
      SetErrorMessage("missing userid or code");
      return;
    }
    postConfirmation();
  }, []);

  return (
    <PageContainer className="text-center">
      <h1>Verify Email</h1>
      <h2 className={statusMessage ? "" : "offscreen"} aria-live="assertive">
        {statusMessage}
      </h2>
      <h2 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {errMsg}
      </h2>
    </PageContainer>
  );
};

export default VerifyEmailPage;
