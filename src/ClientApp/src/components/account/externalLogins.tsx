import { ButtonHTMLAttributes, useEffect, useState } from "react";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import PageContainer from "../pageContainer";
import { Button } from "../ui/button";

type ExternalLoginModel = {
  loginProvider: string;
  providerKey: string;
  providerDisplayName: string | null;
};

type AuthentionSchemeModel = {
  name: string;
  displayName: string | null;
};

type LoginsModel = {
  currentLogins: Array<ExternalLoginModel>;
  otherLogins: Array<AuthentionSchemeModel>;
  showRemoveButton: boolean;
} | null;

const ExternalLoginsPage = () => {
  const axiosPrivate = useAxiosPrivate();
  const [logins, setLogins] = useState<LoginsModel>(null);
  const [errMsg, setErrorMessage] = useState("");
  const [busy, setBusy] = useState(false);
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchLogins = async () => {
      setBusy(true);
      try {
        const reponse = await axiosPrivate.get<LoginsModel>("/ui/account/external-logins");
        if (reponse?.data) {
          setLogins(reponse.data);
          setErrorMessage("");
        }
        setBusy(false);
      } catch (err: any) {
        if (err?.reponse) {
          setErrorMessage(err.reponse?.data);
        }
        setBusy(false);
      }
    };
    fetchLogins();
    //not ideal but best way I could find to do this.
    let error = searchParams.get("error");
    if (error) {
      alert(error);
      navigate("/account/externallogins"); //clean up the addressbar
    }
  }, []);

  const handleRemoveLogin = async (event: React.MouseEvent<HTMLButtonElement>, provider: string, providerKey: string) => {
    event.preventDefault();
    const ok = confirm(`Remove ${provider} external login?`);

    if (!ok) {
      return;
    }

    setBusy(true);
    try {
      const reponse = await axiosPrivate.post("/ui/account/remove-login", { loginProvider: provider, providerKey: providerKey });
      if (reponse?.data) {
        setLogins(reponse.data);
        setErrorMessage("");
      }
    } catch (err: any) {
      if (err?.reponse) {
        setErrorMessage(err.reponse?.data);
      }
    }
    setBusy(false);
  };

  const CurrentLogins = () => {
    const currentLogins = logins?.currentLogins;
    if (!currentLogins) {
      return <></>;
    }
    const values = currentLogins.map((item, index) => {
      return (
        <div className="flex flex-row items-center gap-4 my-2" key={index}>
          <div className="grow" key={item.providerKey}>
            {item.providerDisplayName}
          </div>
          <div className="justify-end">
            {logins.showRemoveButton && (
              <div>
                <input type="hidden" name="LoginProvider" value={item.providerKey} />
                <Button
                  type="submit"
                  variant="destructive"
                  size="sm"
                  className="w-20"
                  title={`Remove this ${item.providerDisplayName} login from your account`}
                  value={item.loginProvider}
                  onClick={(e) => handleRemoveLogin(e, item.loginProvider, item.providerKey)}
                  disabled={busy}>
                  Remove
                </Button>
              </div>
            )}
          </div>
        </div>
      );
    });

    return (
      <div className="mt-2 mb-4">
        <h4>Registered Logins</h4>
        <hr />
        <div className="flex flex-col">{values}</div>
      </div>
    );
  };

  const OtherLogins = () => {
    const otherLogins = logins?.otherLogins;

    if (!otherLogins || otherLogins.length == 0) {
      return <></>;
    }

    const values = otherLogins.map((item, index) => {
      return (
        <form className="flex flex-row my-2" method="POST" action="/ui/account/link-login" key={index}>
          <div className="grow" key={item.name}>
            {item.displayName}
          </div>
          <div className="flex justify-end">
            <Button
              id={item.name}
              type="submit"
              className="w-20"
              variant="create"
              size="sm"
              name="provider"
              value={item.name}
              title={`"Log in using your ${item.displayName} account"`}
              disabled={busy}>
              Add
            </Button>
          </div>
        </form>
      );
    });

    return (
      <div className="mt-2">
        <h4>Add another service to log in.</h4>
        <hr className="border-t-gray-200 dark:border-t-gray-600" />
        <div className="flex flex-col">{values}</div>
      </div>
    );
  };

  return (
    <PageContainer>
      <h3>Manage your external logins</h3>
      {errMsg && <p>{errMsg}</p>}
      <div className="flex flex-col w-full mt-2">
        <CurrentLogins />
      </div>
      <div className="flex flex-col w-full mt-2">
        <OtherLogins />
      </div>
    </PageContainer>
  );
};

export default ExternalLoginsPage;
