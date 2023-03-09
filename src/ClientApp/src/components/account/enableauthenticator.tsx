import { AxiosError } from "axios";
import { QRCodeSVG } from "qrcode.react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import PageContainer from "../pageContainer";

type AuthenticatorDetailsModel = {
  sharedKey: string;
  authenticatorUri: string;
} | null;

const EnableAuthenticatorPage = () => {
  const axiosPrivate = useAxiosPrivate();
  const [errMsg, setErrorMessage] = useState("");
  const [config, setConFig] = useState<AuthenticatorDetailsModel>(null);
  const [code, setCode] = useState("");
  const navigate = useNavigate();
  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      const response = await axiosPrivate.post("/ui/account/2fa-verify", { code: code });
      if (response.data?.codes) {
        navigate("/account/showrecoverycodes", {
          state: {
            codes: response.data.codes,
            message: "Your authenticator app has been verified.",
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

  const onInput = (event: React.ChangeEvent<HTMLInputElement>) => {
    // event.preventDefault();
    setCode(event.target.value);
  };

  useEffect(() => {
    const fetchConfig = async () => {
      try {
        const response = await axiosPrivate.get("/ui/account/2fa-keyinfo");
        setErrorMessage("");
        console.log(response.data);
        setConFig(response.data);
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

  return (
    <PageContainer className="text-gray-800 dark:text-gray-100">
      <h3>Configure Authenticator app</h3>
      <div className="ml-2 px-2 mt-2 text-base font-light">
        <h3 className="py-2">To use an authenticator app go through the following steps:</h3>
        <ol className="list-disc">
          <li className="mb-2">
            <p>
              Download a two-factor authenticator app like Microsoft Authenticator for{" "}
              <a className="underline" href="https://go.microsoft.com/fwlink/?Linkid=825072">
                Android
              </a>{" "}
              and{" "}
              <a className="underline" href="https://go.microsoft.com/fwlink/?Linkid=825073">
                iOS
              </a>{" "}
              or Google Authenticator for{" "}
              <a className="underline" href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2&amp;hl=en">
                Android
              </a>{" "}
              and{" "}
              <a className="underline" href="https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8">
                iOS
              </a>
              .
            </p>
          </li>
          <li className="mb-2">
            <p>
              Scan the QR Code or enter this key{" "}
              <kbd className="px-2 py-1 rounded-md bg-gray-900 text-white dark:bg-gray-200 dark:text-gray-800 font-mono">{`${config?.sharedKey}`}</kbd>{" "}
              into your two factor authenticator app. Spaces and casing do not matter.
            </p>

            {config?.authenticatorUri && (
              <QRCodeSVG value={`${config?.authenticatorUri}`} level="H" className="w-48 h-48 m-4 p-2 bg-white text-black" />
            )}
          </li>
          <li className="mb-2">
            <p className="py-2 mb-2">
              Once you have scanned the QR code or input the key above, your two factor authentication app will provide you with a unique code. Enter
              the code in the confirmation box below.
            </p>
            <div className="flex flex-row">
              <div className="">
                <form onSubmit={handleSubmit}>
                  <div className="flex flex-col mb-3">
                    <label className="control-label form-label mb-1">Verification Code</label>
                    <input
                      id="code"
                      name="code"
                      onChange={onInput}
                      value={code}
                      className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                      autoComplete="off"
                      placeholder="Please enter the code."
                    />
                    {errMsg !== "" && <span className="text-danger"></span>}
                  </div>

                  <button type="submit" className="w-full btn btn btn-primary" disabled={!code}>
                    Verify Code
                  </button>
                  {errMsg !== "" && (
                    <div className="text-red-600" role="alert">
                      {errMsg}
                    </div>
                  )}
                </form>
              </div>
            </div>
          </li>
        </ol>
      </div>
    </PageContainer>
  );
};

export default EnableAuthenticatorPage;
