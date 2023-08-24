import { ButtonHTMLAttributes, useEffect, useState } from "react";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import PageContainer from "../pageContainer";

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
  }, []);

  const handleRemoveLogin = async (event: React.MouseEvent<HTMLButtonElement>, provider: string, providerKey: string) => {
    event.preventDefault();
    const ok = confirm(`Delete ${provider} external login?`);

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
        <div className="flex flex-row items-center gap-4 w-full my-2" key={index}>
          <div className="flex flex-grow" key={item.providerKey}>
            {item.providerDisplayName}
          </div>
          <div className="flex justify-end">
            {logins.showRemoveButton && (
              <div>
                <input type="hidden" name="LoginProvider" value={item.providerKey} />
                <button
                  type="submit"
                  className="btn btn-danger btn-small"
                  title={`Remove this ${item.providerDisplayName} login from your account`}
                  value={item.loginProvider}
                  onClick={(e) => handleRemoveLogin(e, item.loginProvider, item.providerKey)}
                  disabled={busy}>
                  Remove
                </button>
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
        <form className="flex flex-row items-center gap-4 w-full my-2" method="POST" action="/ui/account/link-login" key={index}>
          <div className="flex flex-grow" key={item.name}>
            {item.displayName}
          </div>
          <div className="flex justify-end">
            <button
              id={item.name}
              type="submit"
              className="btn btn-outline btn-small w-16"
              name="provider"
              value={item.name}
              title={`"Log in using your ${item.displayName} account"`}
              disabled={busy}>
              Add
            </button>
          </div>
        </form>
      );
    });

    return (
      <div className="mt-2">
        <h4>Add another service to log in.</h4>
        <hr className="border-t-gray-200 dark:border-t-gray-600" />
        <div className="flex flex-row">{values}</div>
      </div>
    );
  };

  return (
    <PageContainer>
      <h3>Manage your external logins</h3>
      {errMsg && <p>{errMsg}</p>}
      <CurrentLogins />
      <OtherLogins />
    </PageContainer>
  );
};

export default ExternalLoginsPage;
