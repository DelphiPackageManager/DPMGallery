import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import PageContainer from "./pageContainer";

const ConfirmEmailPage = () => {
  const axios = useAxiosPrivate();
  let navigate = useNavigate();
  let [searchParams] = useSearchParams();
  let userId = searchParams.get("userId");
  let code = searchParams.get("code");
  let returnUrl = searchParams.get("returnUrl") || "/";
  let [statusMessage, setStatusMessage] = useState("");
  let [errMsg, SetErrorMessage] = useState<string | null>(null);
  const postConfirmation = async () => {
    try {
      const response = await axios.post("/ui/auth/confirm-email", { userId, code, returnUrl });
      if (response?.status == 200) {
        setStatusMessage("Thank you for confirming your email address.");
        setTimeout(() => {
          navigate(returnUrl);
        }, 5000);
      }
    } catch (err: any) {
      if (err?.response) {
        if (err.response.statusMessage) {
          SetErrorMessage(err.response.statusMessage);
        } else {
          SetErrorMessage("Error confirming email  :  " + err.response.status.toString());
        }
      }
    }
  };

  useEffect(() => {
    if (!code || !userId) {
      SetErrorMessage("missing userid or code");
      return;
    }
    postConfirmation();
  }, []);

  return (
    <PageContainer className="text-center">
      <h1>Confirm Email</h1>
      <h2 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {statusMessage}
      </h2>
      <h2 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {errMsg}
      </h2>
    </PageContainer>
  );
};

export default ConfirmEmailPage;
