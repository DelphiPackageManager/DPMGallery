import { useEffect, useState } from "react";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import PageContainer from "../pageContainer";
const EnableAuthenticatorPage = () => {
  const axiosPrivate = useAxiosPrivate();
  const [errMsg, setErrMsg] = useState("");

  useEffect(() => {
    //
  }, []);

  return (
    <PageContainer>
      <h1>Configure Authenticator app</h1>
      <div className="ml-2 px-2 mt-2 text-gray-200 text-base font-light">
        <h3 className="py-2">To use an authenticator app go through the following steps:</h3>
        <ol className="list-disc">
          <li className="mb-2">
            <p>
              Download a two-factor authenticator app like Microsoft Authenticator for{" "}
              <a href="https://go.microsoft.com/fwlink/?Linkid=825072">Android</a> and{" "}
              <a href="https://go.microsoft.com/fwlink/?Linkid=825073">iOS</a> or Google Authenticator for{" "}
              <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2&amp;hl=en">Android</a> and{" "}
              <a href="https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8">iOS</a>.
            </p>
          </li>
          <li className="mb-2">
            <p>
              Scan the QR Code or enter this key <kbd>@Model.SharedKey</kbd> into your two factor authenticator app. Spaces and casing do not matter.
            </p>
            <div id="qrCode"></div>
            <div id="qrCodeData" data-url="@Model.AuthenticatorUri"></div>
          </li>
          <li className="mb-2">
            <p className="py-2 mb-2">
              Once you have scanned the QR code or input the key above, your two factor authentication app will provide you with a unique code. Enter
              the code in the confirmation box below.
            </p>
            <div className="flex flex-row">
              <div className="">
                <form id="send-code" method="post">
                  <div className="flex flex-col mb-3">
                    <label className="control-label form-label mb-1">Verification Code</label>
                    <input
                      className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                      autoComplete="off"
                      placeholder="Please enter the code."
                    />
                    {errMsg !== "" && <span className="text-danger"></span>}
                  </div>

                  <button type="submit" className="w-100 btn btn btn-primary">
                    Verify
                  </button>
                  <div className="text-danger" role="alert"></div>
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
