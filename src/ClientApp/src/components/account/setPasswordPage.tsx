import { useState } from "react";
import PageContainer from "../pageContainer";

const SetPasswordPage = () => {
  const [statusMessage, setStatusMessage] = useState("Checking password status....");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  return (
    <form className="mt-4 w-full">
      <h3>Set your Password</h3>
      <p className="text-info">You do not have a local password for this site. Add a local account so you can log in without an external login.</p>
      {statusMessage && <p>{statusMessage}</p>}
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
          Set password
        </button>
      </div>
    </form>
  );
};

export default SetPasswordPage;
