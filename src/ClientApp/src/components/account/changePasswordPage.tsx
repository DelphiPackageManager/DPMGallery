import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "../../api/axios";
import PageContainer from "../pageContainer";
import SetPasswordPage from "./setPasswordPage";

const CHECKPWDSTATUS_URL = "/ui/account/haspassword";
const CHANGEPWD_URL = "/ui/account/"

const ChangePasswordPage = () => {
  const [hasPassword, setHasPassword] = useState(false);
  const [statusMessage, setStatusMessage] = useState("Checking password status....");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  useEffect(() => {
    (async () => {
      try {
        const response = await axios.get(CHECKPWDSTATUS_URL);
        if (response?.status == 200) {
          setLoading(false);
          setHasPassword(true);
          setStatusMessage("");
        }
      } catch (err: any) {
        if (err?.response?.status == 404) {
          setHasPassword(false);
          setLoading(false);
          setStatusMessage("");
        } else setStatusMessage(err?.message);
      }
    })();

    //check if user has a password set
    //if they logged in with an external login
    //that will not be the case.
  }, []);


  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const ok = confirm(`Change Email?`);
    if (!ok) {
      return;
    }

    try {
      const response = await axios.post(CHANGEPWD_URL, { newEmail: email });
      if (response?.status == 200) {
        setStatusMessage(response?.data);
      }
    } catch (err: any) {
      setStatusMessage(err?.statusText);
    }
  };


  return (
    <PageContainer>
      {statusMessage && <p>{statusMessage}</p>}

      {!loading && hasPassword && (
        <form className="mt-4 w-full">
          <h3>Password</h3>
          <div className="">
            <label htmlFor="currentPassword" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
              Please enter your Current Password.
            </label>
            <input
              type="password"
              name="currentPassword"
              id="currentPassword"
              size={50}
              minLength={8}
              maxLength={256}
              onChange={(e) => setCurrentPassword(e.target.value)}
              value={currentPassword}
              className="border border-gray-300 text-gray-700 sm:text-sm rounded-lg  block p-2.5 bg-gray-200 dark:bg-gray-900 dark:border-gray-700 dark:text-gray-300 "
              placeholder=""></input>
          </div>
          <div className="mt-4">
            <label htmlFor="newPassword" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
              Please enter your New Password.
            </label>
            <input
              type="password"
              name="newPassword"
              id="newPassword"
              size={50}
              minLength={8}
              maxLength={256}
              onChange={(e) => setNewPassword(e.target.value)}
              value={newPassword}
              className="border border-gray-300 text-gray-700 sm:text-sm rounded-lg  block p-2.5 bg-gray-200 dark:bg-gray-900 dark:border-gray-700 dark:text-gray-300 "
              placeholder=""></input>
          </div>
          <div className="mt-4">
            <label htmlFor="confirmPassword" className="block mb-2 text-sm font-medium text-gray-900 dark:text-white">
              Please confirm your New Password.
            </label>
            <input
              type="password"
              name="confirmPassword"
              id="confirmPassword"
              size={50}
              minLength={8}
              maxLength={256}
              onChange={(e) => setConfirmPassword(e.target.value)}
              value={confirmPassword}
              className="border border-gray-300 text-gray-700 sm:text-sm rounded-lg  block p-2.5 bg-gray-200 dark:bg-gray-900 dark:border-gray-700 dark:text-gray-300 "
              placeholder=""></input>
          </div>
          <div className="mt-4">
            <button type="submit" className="btn btn-primary w-60">
              Update password
            </button>
          </div>
        </form>
      )}

      {!loading && !hasPassword && <SetPasswordPage />}
    </PageContainer>
  );
};

export default ChangePasswordPage;
