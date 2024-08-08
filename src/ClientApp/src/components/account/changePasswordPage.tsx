import axios from "@/api/axios";
import { useEffect, useRef, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import PageContainer from "../pageContainer";
import PageHeader from "../pageHeader";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import SetPasswordPage from "./setPasswordPage";

const CHECKPWDSTATUS_URL = "/ui/account/haspassword";
const CHANGEPWD_URL = "/ui/account/change-password";

const ChangePasswordPage = () => {
	const navigate = useNavigate();
	const location = useLocation();

	const passwordRef = useRef<HTMLInputElement>(null);
	const [hasPassword, setHasPassword] = useState(false);
	const [pageTitle, setPageTitle] = useState("Password");
	const [statusMessage, setStatusMessage] = useState("");
	const [currentPassword, setCurrentPassword] = useState("");
	const [newPassword, setNewPassword] = useState("");
	const [confirmPassword, setConfirmPassword] = useState("");
	const [loading, setLoading] = useState(false);
	const [errMsg, setErrorMessage] = useState("");
	const [disabled, setDisabled] = useState(false);

	useEffect(() => {
		(async () => {
			setStatusMessage("Checking password status....");
			try {
				const response = await axios.get(CHECKPWDSTATUS_URL);
				if (response?.status == 200) {
					setLoading(false);
					setHasPassword(true);
					setStatusMessage("");
					setPageTitle("Change Password");
				}
			} catch (err: any) {
				if (err?.response?.status == 401) {
					navigate("/login", { state: location.pathname });
				} else if (err?.response?.status == 404) {
					setHasPassword(false);
					setLoading(false);
					setStatusMessage("");
					setPageTitle("Set Password");
				} else setStatusMessage(err?.message);
			}
		})();

		//check if user has a password set
		//if they logged in with an external login
		//that will not be the case.
	}, []);

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

		const ok = confirm(`Change Password?`);
		if (!ok) {
			return;
		}
		setDisabled(true);
		try {
			const response = await axios.post(CHANGEPWD_URL, { oldPassword: currentPassword, newPassword: newPassword });
			if (response?.status == 200) {
				setStatusMessage(response?.data);
				setCurrentPassword("");
				setNewPassword("");
				setConfirmPassword("");
			}
		} catch (err: any) {
			setStatusMessage(err?.statusText);
		} finally {
			setDisabled(false);
		}
	};

	return (
		<PageContainer>
			<PageHeader title={pageTitle} />
			<div className="px-2">
				{statusMessage && <p className="mt-2">{statusMessage}</p>}
				<p className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
					{errMsg}
				</p>
				{!loading && hasPassword && (
					<form className="mt-4 w-full" onSubmit={handleSubmit}>
						<div className="mt-4">
							<Label htmlFor="currentPassword" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
								Please enter your Current Password.
							</Label>
							<Input
								type="password"
								name="currentPassword"
								id="currentPassword"
								size={50}
								minLength={8}
								maxLength={256}
								onChange={(e) => setCurrentPassword(e.target.value)}
								value={currentPassword}
								className=" "
								placeholder="" />
						</div>
						<div className="mt-4">
							<Label htmlFor="newPassword" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
								Please enter your New Password.
							</Label>
							<Input
								type="password"
								name="newPassword"
								id="newPassword"
								size={50}
								minLength={8}
								maxLength={256}
								onChange={(e) => setNewPassword(e.target.value)}
								value={newPassword}
								ref={passwordRef}
								className="" placeholder="" />
						</div>
						<div className="mt-4">
							<Label htmlFor="confirmPassword" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
								Please confirm your New Password.
							</Label>
							<Input
								type="password"
								name="confirmPassword"
								id="confirmPassword"
								size={50}
								minLength={8}
								maxLength={256}
								onChange={(e) => setConfirmPassword(e.target.value)}
								value={confirmPassword}
								className="" placeholder="" />
						</div>
						<div className="mt-4">
							<Button type="submit" className="w-60" disabled={disabled}>
								Update password
							</Button>
						</div>
					</form>
				)}

				{!loading && !hasPassword && <SetPasswordPage setHasPassword={setHasPassword} setPageTitle={setPageTitle} />}
			</div>
		</PageContainer>
	);
};

export default ChangePasswordPage;
