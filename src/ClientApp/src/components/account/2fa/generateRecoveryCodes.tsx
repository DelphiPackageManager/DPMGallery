import { useState } from "react";
import { useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../../hooks/useAxiosPrivate";
import PageContainer from "../../pageContainer";

const GenerateRecoveryCodesPage = () => {
  const axiosPrivate = useAxiosPrivate();
  const [errMsg, setErrorMessage] = useState("");

  const navigate = useNavigate();

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      const response = await axiosPrivate.post("/ui/account/2fa-generatecodes");
      if (response.data?.codes) {
        navigate("/account/showrecoverycodes", {
          state: {
            codes: response.data.codes,
            message: "You have generated new recovery codes.",
          },
        });
      } else {
        navigate("/account/twofactorauth");
      }
    } catch (err: any) {
      console.log(err);
      setErrorMessage(err?.response?.data);
    }
  };

  return (
    <PageContainer className="">
      <h3>Generate two-factor authentication (2FA) recovery codes</h3>
      <div className="alert alert-warning" role="alert">
        <p>
          <span className="glyphicon glyphicon-warning-sign"></span>
          <strong>Put these codes in a safe place.</strong>
        </p>
        <p>If you lose your device and don't have the recovery codes you will lose access to your account.</p>
        <p>
          Generating new recovery codes does not change the keys used in authenticator apps. If you wish to change the key used in an authenticator
          app you should <a asp-page="./ResetAuthenticator">reset your authenticator keys.</a>
        </p>
      </div>
      <div>
        <form onSubmit={handleSubmit}>
          <button className="btn btn-danger" type="submit">
            Generate Recovery Codes
          </button>
        </form>
      </div>
      {errMsg !== "" && (
        <div className="text-red-600" role="alert">
          {errMsg}
        </div>
      )}
    </PageContainer>
  );
};

export default GenerateRecoveryCodesPage;
