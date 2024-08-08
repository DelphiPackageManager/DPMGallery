import axios from "@/api/axios";
import { Label } from "@radix-ui/react-label";
import { useEffect, useRef, useState } from "react";
import useAuth from "../../hooks/useAuth";
import { validateEmail } from "../../utils";
import PageContainer from "../pageContainer";
import PageHeader from "../pageHeader";
import { Button } from "../ui/button";
import { Input } from "../ui/input";

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
			setStatusMessage(err?.message);
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
			setStatusMessage(response?.data);
			setChangeEnabled(false);
			setCurrentEmail(email);
			setEmail("");
		} catch (err: any) {
			setStatusMessage(err?.statusText);
		}
	};

	return (
		<PageContainer>
			<PageHeader title="Manage Email" />
			<div className="mt-4 pl-2">
				<div className="">
					<label htmlFor="email" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
						Your current email address
					</label>

					<div className="flex flex-row items-center">
						<Input
							type="text"
							name="currentEmail"
							id="currentEmail"
							size={60}
							className="block rounded-lg border border-gray-300 bg-gray-200 p-2.5 text-gray-700 dark:border-gray-700 dark:bg-gray-900 dark:text-gray-300 sm:text-sm"
							placeholder=""
							disabled
							defaultValue={currentEmail} />
						<div className="ml-2 w-10">{emailConfirmed && <a title="Email Verified">✓</a>}</div>
					</div>
				</div>
				{!emailConfirmed && (
					<div className="mt-1">
						<Button variant="link" onClick={handleSendVerificationEmail}>
							Send verification email
						</Button>
					</div>
				)}
				{statusMessage && (
					<div className="mt-4">
						<p className="">{statusMessage}</p>
					</div>
				)}
				<form className="mt-4 flex max-w-fit flex-col" method="POST" onSubmit={handleSubmit}>
					<div className="mt-4 w-full">
						<label htmlFor="email" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
							Your new email address
						</label>
						<div className="flex flex-row items-center">
							<Input name="email" id="email" size={60}
								className=""
								onChange={(e) => handleNewEmailChanged(e)}
								placeholder="name@company.com"
								pattern="/^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i"
								ref={newEmailRef}
								value={email}
								required />
							<div className="ml-2 w-10"></div>
						</div>
					</div>
					<Button size="lg" className="mt-4" disabled={!changeEnabled} type="submit">
						Change Email
					</Button>
					<div className="ml-2 w-10"></div>
				</form>
				<p className="mt-6 italic text-muted-foreground">We use the organization's name and its verified email address to get its publicly visible Gravatar profile picture (or a default image if it does not have a Gravatar profile picture).
					Please go to <a className="not-italic text-primary" href="gravatar.com">gravatar.com</a> to change the profile picture for your organization.</p>
			</div>
		</PageContainer>
	);
};

export default EmailSettingsPage;
