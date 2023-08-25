import { useEffect, useState } from "react";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
import useAuth from "../../hooks/useAuth";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import PageContainer from "../pageContainer";

const ConfirmEmailChangePage = () => {
  const location = useLocation();
  let navigate = useNavigate();
  const axios = useAxiosPrivate();
  const { currentUser, login } = useAuth();
  const [searchParams] = useSearchParams();
  let [statusMessage, setStatusMessage] = useState("");
  let [errMsg, SetErrorMessage] = useState<string | null>(null);

  const code = searchParams.get("code");
  const email = searchParams.get("email") || "";
  const userId = searchParams.get("userId");

  const postConfirmation = async () => {
    try {
      const response = await axios.post("/ui/account/confirm-email-change", { userId, code, email });
      if (response?.status == 200) {
        setStatusMessage("Thank you for confirming your email change.");
        //update the auth context
        let user = currentUser;
        if (user) {
          user.email = email;
          login(user);
        }

        setTimeout(() => {
          navigate("/account/email");
        }, 5000);
      }
    } catch (err: any) {
      if (err?.response) {
        if (err?.response?.status == 401) {
          navigate("/login", { state: location.pathname });
        }

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
      <h1 className="mb-4">Confirm Email change.</h1>
      <h2 className="mb-4" aria-live="assertive">
        {statusMessage}
      </h2>
      <h2 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {errMsg}
      </h2>
    </PageContainer>
  );
};

export default ConfirmEmailChangePage;
