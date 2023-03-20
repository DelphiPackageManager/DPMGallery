import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import PageContainer from "./pageContainer";

type ConfirmEmailModel = {
  userId: string | undefined;
  code: string | undefined;
  returnUrl: string | undefined;
};

const ConfirmEmailPage = () => {
  const [errMsg, setErrorMessage] = useState("");
  const axios = useAxiosPrivate();
  let emailConfirmed = false;

  let { returnUrl, userId, code } = useParams();
  const model: ConfirmEmailModel = {
    userId,
    code,
    returnUrl,
  };

  const postConfirmation = async () => {
    try {
      const response = await axios.post("/ui/auth/confirmemail", model);
      if (response?.status == 200) {
        emailConfirmed = true;
      }
    } catch (err: any) {
      if (err?.response) {
        if (err.response.statusMessage) {
          setErrorMessage(err.response.statusMessage);
        } else {
          setErrorMessage("Error fetching external login details - Error :  " + err.response.status.toString());
        }
      }
    }
  };

  useEffect(() => {
    if (!model.code || !model.userId) {
      setErrorMessage("invalid userid or code");
      return;
    }
    postConfirmation();
  }, []);

  return (
    <PageContainer className="text-center">
      <h1>Confirm Email</h1>
      <p className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
        {errMsg}
      </p>
      {emailConfirmed && <p>Email confirmed.</p>}
    </PageContainer>
  );
};

export default ConfirmEmailPage;
