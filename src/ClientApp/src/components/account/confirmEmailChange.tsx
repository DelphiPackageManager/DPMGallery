import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import PageContainer from "../pageContainer";

const ConfirmEmailChangePage = () => {
  const axios = useAxiosPrivate();
  let navigate = useNavigate();
  const [searchParams] = useSearchParams();
  let [statusMessage, setStatusMessage] = useState("");
  let [errMsg, SetErrorMessage] = useState<string | null>(null);

  const code = searchParams.get("code");
  const email = searchParams.get("email");
  const userId = searchParams.get("userId");

  const postConfirmation = async () => {
    try {
      const response = await axios.post("/ui/account/confirm-email-change", { userId, code, email });
      if (response?.status == 200) {
        setStatusMessage("Thank you for confirming your email address.");
        setTimeout(() => {
          navigate("/account/email");
        }, 5000);
      }
    } catch (err: any) {
      if (err?.response) {
        if (err.response.statusMessage) {
          SetErrorMessage(err.response.statusMessage);
        } else {
          SetErrorMessage("Error confirming email change  :  " + err.response.status.toString());
        }
      }
    }
  };

  useEffect(() => {
    if (!code || !userId || !email) {
      SetErrorMessage("Invalid confirmation link");
      return;
    }
    postConfirmation();
  }, []);
  return (
    <PageContainer>
      <h1>Confirm Email change.</h1>
      <h2 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {statusMessage}
      </h2>
      <h2 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {errMsg}
      </h2>
    </PageContainer>
  );
};
