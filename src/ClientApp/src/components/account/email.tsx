import { useEffect, useRef, useState } from "react";
import axios from "../../api/axios";
import useAuth from "../../hooks/useAuth";
import PageContainer from "../pageContainer";

const SENDVERIFYEMAIL_URL = "/ui/account/send-verify-email";

const EmailSettingsPage = () => {
  const [statusMessage, setStatusMessage] = useState("");
  const [email, setEmail] = useState("");
  const [newEmail, setNewEmail] = useState("");
  const [emailConfirmed, setEmailConfirmed] = useState(false);
  const { auth } = useAuth();
  const newEmailRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    setEmail(auth?.user?.email || "");
    setEmailConfirmed(auth?.user?.emailConfirmed || false);
  }, []);

  const handleSendVerificationEmail = async (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    try {
      const response = await axios.post(SENDVERIFYEMAIL_URL, { email });
      if (response?.status == 200) {
        setStatusMessage(response?.data);
      }
    } catch (err: any) {
      setStatusMessage(err?.statusText);
    }
  };

  return (
    <PageContainer>
      <h3>Manage Email</h3>

      <div className="mt-4">
        <div className="w-1/2">
          <label htmlFor="email" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
            Your current email address
          </label>

          <div className="flex flex-row items-center">
            <input
              type="email"
              name="email"
              id="email"
              className="border border-gray-300 text-gray-700 sm:text-sm rounded-lg  block w-full p-2.5 bg-gray-200 dark:bg-gray-800 dark:border-gray-600 dark:text-gray-300 "
              onChange={(e) => setEmail(e.target.value)}
              placeholder="name@company.com"
              disabled={emailConfirmed}
              value={email}></input>
            <div className="w-10 ml-2">{emailConfirmed && <a title="Email Verified">✓</a>}</div>
          </div>
        </div>
        {!emailConfirmed && (
          <div className="mt-1">
            <button className="btn btn-link" onClick={handleSendVerificationEmail}>
              Send verification email
            </button>
          </div>
        )}
        {statusMessage && (
          <div className="mt-2">
            <p className="">{statusMessage}</p>
          </div>
        )}
        <div className="mt-4 w-1/2">
          <label htmlFor="email" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
            Your new email address
          </label>
          <div className="flex flex-row items-center">
            <input
              type="email"
              name="newemail"
              id="newemail"
              className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
              onChange={(e) => setNewEmail(e.target.value)}
              placeholder="name@company.com"
              ref={newEmailRef}
              value={newEmail}
              required></input>
            <div className="w-10 ml-2"></div>
          </div>
        </div>
        <div className="w-1/2 mt-4 flex flex-row">
          <button className="btn btn-primary btn-large w-full">Change Email</button>
          <div className="w-10 ml-2"></div>
        </div>
      </div>
    </PageContainer>
  );
};

export default EmailSettingsPage;
