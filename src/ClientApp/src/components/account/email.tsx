import { useEffect, useRef, useState } from "react";
import axios from "../../api/axios";
import useAuth from "../../hooks/useAuth";
import { validateEmail } from "../../utils";
import PageContainer from "../pageContainer";

const SENDVERIFYEMAIL_URL = "/ui/account/send-verify-email";

const EmailSettingsPage = () => {
  const [statusMessage, setStatusMessage] = useState("");
  const [email, setEmail] = useState("");
  const [currentEmail, setCurrentEmail] = useState("");
  const [emailConfirmed, setEmailConfirmed] = useState(false);
  const [changeEnabled, setChangeEnabled] = useState(false);
  const { currentUser } = useAuth();
  const newEmailRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    setCurrentEmail(currentUser?.email || "");
    setEmailConfirmed(currentUser?.emailConfirmed || false);
    setChangeEnabled(false);
  }, []);

  const handleSendVerificationEmail = async (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    try {
      const response = await axios.post(SENDVERIFYEMAIL_URL, { currentEmail });
      if (response?.status == 200) {
        setStatusMessage(response?.data);
      }
    } catch (err: any) {
      setStatusMessage(err?.statusText);
    }
  };

  const handleNewEmailChanged = async (event: React.ChangeEvent<HTMLInputElement>) => {
    event.preventDefault();
    setEmail(event.target.value);
    const isValid = validateEmail(event.target.value) !== null;
    setChangeEnabled(isValid && event.target.value !== currentEmail);
  };

  const CHANGEEMAIL_URL = "/ui/account/change-email";

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const ok = confirm(`Change Email?`);
    if (!ok) {
      return;
    }

    try {
      const response = await axios.post(CHANGEEMAIL_URL, { newEmail: email });
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
              type="text"
              name="currentEmail"
              id="currentEmail"
              size={50}
              className="border border-gray-300 text-gray-700 sm:text-sm rounded-lg  block p-2.5 bg-gray-200 dark:bg-gray-900 dark:border-gray-700 dark:text-gray-300 "
              placeholder=""
              disabled
              defaultValue={currentEmail}></input>
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
          <div className="mt-4">
            <p className="">{statusMessage}</p>
          </div>
        )}
        <form className="w-1/2 mt-4 flex flex-col" method="POST" onSubmit={handleSubmit}>
          <div className="mt-4 w-full">
            <label htmlFor="email" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
              Your new email address
            </label>
            <div className="flex flex-row items-center">
              <input
                type="email"
                name="email"
                id="email"
                size={53}
                className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                onChange={(e) => handleNewEmailChanged(e)}
                placeholder="name@company.com"
                pattern="/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i"
                ref={newEmailRef}
                value={email}
                required></input>
              <div className="w-10 ml-2"></div>
            </div>
          </div>
          <button className="mt-4 btn btn-primary btn-large disabled:bg-gray-500 disabled:hover:bg-gray-500" disabled={!changeEnabled} type="submit">
            Change Email
          </button>
          <div className="w-10 ml-2"></div>
        </form>
      </div>
    </PageContainer>
  );
};

export default EmailSettingsPage;
