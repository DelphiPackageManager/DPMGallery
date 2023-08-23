import { useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "../../../api/axios";
import useAxiosPrivate from "../../../hooks/useAxiosPrivate";
import PageContainer from "../../pageContainer";

const ResetAuthenticatorAppPage = () => {
  const navigate = useNavigate();
  const axiosPrivate = useAxiosPrivate();
  const [errMsg, setErrMsg] = useState("");

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    console.log("resetting...");
    axiosPrivate
      .post("/ui/account/2fa-reset")
      .then((response) => {
        if (response?.status == 200) {
          navigate("/account/twofactorauth");
        } else {
          setErrMsg("reset failed : " + response.status.toString());
        }
      })
      .catch((err) => {
        setErrMsg(err);
      });
  };

  return (
    <PageContainer className="">
      <h3>Reset authenticator key</h3>
      <div className="alert alert-warning" role="alert">
        <p className="mb-2">
          <span className="glyphicon glyphicon-warning-sign"></span>
          <strong>If you reset your authenticator key your authenticator app will not work until you reconfigure it.</strong>
        </p>
        <p className="font-light">
          This process disables 2FA until you verify your authenticator app. If you do not complete your authenticator app configuration you may lose
          access to your account.
        </p>
      </div>
      <div className="mt-2">
        {errMsg !== "" && (
          <div>
            <span>{errMsg}</span>
          </div>
        )}
        <form onSubmit={handleSubmit}>
          <button id="reset-authenticator-button" className="btn btn-danger" type="submit">
            Reset authenticator key
          </button>
        </form>
      </div>
    </PageContainer>
  );
};

export default ResetAuthenticatorAppPage;
