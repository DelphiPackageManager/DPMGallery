import { useRef, useState } from "react";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";

interface SetPasswordPageProps {
  setHasPassword: React.Dispatch<React.SetStateAction<boolean>>;
  setPageTitle: React.Dispatch<React.SetStateAction<string>>;
}

const SETPWD_URL = "/ui/account/set-password";
const SetPasswordPage: React.FunctionComponent<SetPasswordPageProps> = (props) => {
  const [statusMessage, setStatusMessage] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [disabled, setDisabled] = useState(false);
  const [errMsg, setErrorMessage] = useState("");
  const passwordRef = useRef<HTMLInputElement>(null);
  const axios = useAxiosPrivate();

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setErrorMessage("");
    if (newPassword !== confirmPassword) {
      setErrorMessage("Passwords do not match");
      passwordRef?.current?.focus();
      return;
    }
    if (newPassword.length < 8 || newPassword.length > 256) {
      setErrorMessage("Passwords must be at least 8 characters (max 256)");
      passwordRef?.current?.focus();
      return;
    }
    setDisabled(true);
    try {
      const response = await axios.post(SETPWD_URL, { newPassword: newPassword });
      if (response?.status == 200) {
        setStatusMessage(response?.data);
        setNewPassword("");
        setConfirmPassword("");
        setTimeout(() => {
          //switch back to change password view
          props.setHasPassword(true);
          props.setPageTitle("Change Password");
        }, 3000);
      }
    } catch (err: any) {
      setErrorMessage(err?.statusText);
      setDisabled(false);
    }
  };

  return (
    <form className="mt-4 w-full" onSubmit={handleSubmit}>
      <p className="text-info">You do not have a local password for this site. Add a local account so you can log in without an external login.</p>
      <p className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
        {errMsg}
      </p>
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
          disabled={disabled}
          ref={passwordRef}
          className=""
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
          disabled={disabled}
          className=""
          placeholder=""></input>
      </div>
      <div className="mt-4">
        <button type="submit" className="btn btn-primary w-60" disabled={disabled}>
          Set password
        </button>
      </div>
      {statusMessage && <p>{statusMessage}</p>}
    </form>
  );
};

export default SetPasswordPage;
