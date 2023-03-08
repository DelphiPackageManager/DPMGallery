import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import useAuth from "../../hooks/useAuth";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import PageContainer from "../pageContainer";

type TwoFactorAuthModel = {
  twoFactorEnabled: boolean;
  hasAuthenticator: boolean;
  recoveryCodesLeft: number;
  isMachineRemembered: boolean;
} | null;

const TwoFactorAuthenticationPage = () => {
  const { auth } = useAuth();
  const axiosPrivate = useAxiosPrivate();
  const [twofaConfig, setTwoFaConfig] = useState<TwoFactorAuthModel>(null);
  const [errMsg, setErrorMessage] = useState("");

  useEffect(() => {
    //
    const fetchConfig = async () => {
      const response = await axiosPrivate.get("/ui/account/2fa-config");
      if (response?.status == 200) {
        setErrorMessage("");
        setTwoFaConfig(response.data);
      } else {
        if (response?.data) {
          setErrorMessage(response?.data);
        } else {
          setErrorMessage("An unkown error occured while fetching 2fa config : " + response?.status.toString());
        }
      }
    };
    fetchConfig();
  }, []);

  return (
    <PageContainer>
      <h1>Two-factor authentication (2FA)</h1>
      {errMsg !== "" && <h2>{errMsg}</h2>}
      {twofaConfig?.twoFactorEnabled && (
        <div>
          {twofaConfig.recoveryCodesLeft == 0 && (
            <div>
              <strong>You have no recovery codes left.</strong>
              <p>
                You must <Link to="/account/generateRecoveryCodes">generate a new set of recovery codes</Link> before you can log in with a recovery
                code.
              </p>
            </div>
          )}
          {twofaConfig.recoveryCodesLeft < 3 && (
            <div>
              <strong>You have `${twofaConfig.recoveryCodesLeft}` recovery code left.</strong>
              <p>
                You should <Link to="/account/generateRecoveryCodes">generate a new set of recovery codes</Link>.
              </p>
            </div>
          )}
          {twofaConfig.isMachineRemembered && (
            <form method="post" className="inline-block">
              <button type="submit" className="btn btn-primary">
                Forget this browser
              </button>
            </form>
          )}
          <Link to="/account/disable2fa" className="btn btn-primary">
            Disable 2FA
          </Link>
          <Link to="/account/generateRecoveryCodes" className="btn btn-primary">
            Reset recovery codes
          </Link>
        </div>
      )}

      <h4>Authenticator app</h4>
      <div className="pt-2">
        {!twofaConfig?.hasAuthenticator && (
          <Link to="/account/enableauthenticator" className="btn btn-primary">
            Add Authenticator app
          </Link>
        )}
        {twofaConfig?.hasAuthenticator && (
          <div className="flex flex-row gap-3">
            <Link to="/account/enableauthenticator" className="btn btn-primary">
              Set up Authenticator app
            </Link>
            <Link to="/account/resetauthenticator" className="btn btn-primary">
              Reset Authenticator app
            </Link>
          </div>
        )}
      </div>
    </PageContainer>
  );
};

export default TwoFactorAuthenticationPage;
