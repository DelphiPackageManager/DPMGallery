import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { axiosPrivate } from "../../../api/axios";
import useAuth from "../../../hooks/useAuth";
import PageContainer from "../../pageContainer";

const DisableAuthenticatorPage = () => {
  const { currentUser } = useAuth();
  const [twoFaEnabled, setTwoFAEnabled] = useState(false);
  const navigate = useNavigate();
  const [errMsg, setErrorMessage] = useState("");
  useEffect(() => {
    //console.log(currentUser);
    setTwoFAEnabled(currentUser?.twoFactorEnabled || false);
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const ok = confirm(`Disable Two-factor Authentication?`);
    if (!ok) {
      return;
    }

    try {
      const response = await axiosPrivate.post("/ui/account/2fa-disable", {});
      if (response.status == 200) {
        navigate("/account/twofactorauth");
        //navigate("/account/showrecoverycodes", {
        //   state: {
        //     codes: response.data.codes,
        //     message: "Your authenticator app has been verified.",
        //   },
        // });
      } else {
        setErrorMessage(response?.statusText);
      }
    } catch (err: any) {
      //   console.log(err);
      setErrorMessage(err?.statusText);
    }
  };

  return (
    <PageContainer>
      <h3>Disable Two-factor Authentication</h3>
      {errMsg && <p>{errMsg}</p>}
      <div className="alert alert-warning" role="alert">
        <p className="mb-2">
          <span className="glyphicon glyphicon-warning-sign"></span>
          <strong>This action only disables 2FA.</strong>
        </p>
        <p className="font-light">
          Disabling 2FA does not change the keys used in authenticator apps. If you wish to change the key used in an authenticator app you should{" "}
          <Link to="/account/resetauthenticator">reset your authenticator keys.</Link>
        </p>
      </div>
      {twoFaEnabled && (
        <form onSubmit={handleSubmit}>
          <button className="btn btn-danger" type="submit">
            Disable 2FA
          </button>
        </form>
      )}
    </PageContainer>
  );
};

export default DisableAuthenticatorPage;
