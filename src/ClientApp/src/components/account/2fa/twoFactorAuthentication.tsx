import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
//import useAuth from "../../hooks/useAuth";
import useAxiosPrivate from "../../../hooks/useAxiosPrivate";
import PageContainer from "../../pageContainer";

type TwoFactorAuthModel = {
  twoFactorEnabled: boolean;
  hasAuthenticator: boolean;
  recoveryCodesLeft: number;
  isMachineRemembered: boolean;
} | null;

const TwoFactorAuthenticationPage = () => {
  //const { auth } = useAuth();
  const axiosPrivate = useAxiosPrivate();
  const [twofaConfig, setTwoFaConfig] = useState<TwoFactorAuthModel>(null);
  const [errMsg, setErrorMessage] = useState("");

  useEffect(() => {
    const fetchConfig = async () => {
      try {
        const response = await axiosPrivate.get("/ui/account/2fa-config");
        setErrorMessage("");
        setTwoFaConfig(response.data);
        //  console.log(response.data);
      } catch (err: any) {
        if (err?.data) {
          setErrorMessage(err?.data);
        } else {
          setErrorMessage("An unkown error occured while fetching 2fa config : " + err?.status?.toString());
        }
      }
    };
    fetchConfig();
  }, []);

  const handleForgetBrowser = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    //console.log("forgetting machine");
    try {
      const response = await axiosPrivate.post("/ui/account/2fa-forget");
      setErrorMessage("");
      setTwoFaConfig(response.data);
      //console.log(response.data);
    } catch (err: any) {
      if (err?.data) {
        setErrorMessage(err?.data);
      } else {
        setErrorMessage("An unkown error occured while fetching 2fa config : " + err?.status.toString());
      }
    }
  };

  return (
    <PageContainer>
      <h3>Two-factor authentication (2FA)</h3>
      {errMsg !== "" && <h2>{errMsg}</h2>}
      {twofaConfig?.twoFactorEnabled && (
        <div>
          {twofaConfig.recoveryCodesLeft == 0 && (
            <div>
              <strong>You have no recovery codes left.</strong>
              <p>
                You must <Link to="/account/generaterecoverycodes">generate a new set of recovery codes</Link> before you can log in with a recovery
                code.
              </p>
            </div>
          )}
          {twofaConfig?.recoveryCodesLeft < 3 && (
            <div>
              <strong>You have `${twofaConfig.recoveryCodesLeft}` recovery code left.</strong>
              <p>
                You should <Link to="/account/generateRecoveryCodes">generate a new set of recovery codes</Link>.
              </p>
            </div>
          )}
          {twofaConfig?.isMachineRemembered && (
            <form className="inline-block" onSubmit={handleForgetBrowser}>
              <button type="submit" className="btn btn-outline">
                Forget this browser
              </button>
            </form>
          )}
          <div className="flex flex-row gap-3 pt-2">
            <Link to="/account/disable2fa" className="btn btn-outline">
              Disable 2FA
            </Link>
            <Link to="/account/generaterecoverycodes" className="btn btn-outline">
              Reset recovery codes
            </Link>
          </div>
        </div>
      )}
      <h4 className="mt-2">Authenticator app</h4>
      <div className="pt-2">
        {!twofaConfig?.hasAuthenticator && (
          <Link to="/account/enableauthenticator" className="btn btn-outline">
            Add Authenticator app
          </Link>
        )}
        {twofaConfig?.hasAuthenticator && (
          <div className="flex flex-row gap-3">
            <Link to="/account/enableauthenticator" className="btn btn-primary">
              Set up Authenticator app
            </Link>
            <Link to="/account/resetauthenticator" className="btn btn-danger">
              Reset Authenticator app
            </Link>
          </div>
        )}
      </div>
    </PageContainer>
  );
};

export default TwoFactorAuthenticationPage;
