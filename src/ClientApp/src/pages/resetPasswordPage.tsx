import { useRef, useState } from "react";
import axios from "../api/axios";

import { Input } from "@/components/ui/input";
import { useNavigate, useSearchParams } from "react-router-dom";
import PageContainer from "../components/pageContainer";
import { Button } from "../components/ui/button";

const RESETPWD_URL = "/ui/auth/resetpassword";

const ResetPasswordPage = () => {
	const passwordRef = useRef<HTMLInputElement>(null);

	const navigate = useNavigate();
	let [searchParams] = useSearchParams();
	let theCode = searchParams.get("code");

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [confirmPassword, setConfirmPassword] = useState("");
	const [submitted, setSubmitted] = useState(false);
	const [errMsg, setErrorMessage] = useState("");

	const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		console.log(password);
		console.log(confirmPassword);
		if (password !== confirmPassword) {
			setErrorMessage("Passwords do not match");
			passwordRef?.current?.focus();
			return;
		}
		if (password.length < 8 || password.length > 256) {
			setErrorMessage("Passwords must be at least 8 characters (max 256)");
			passwordRef?.current?.focus();
			return;
		}
		try {
			const response = await axios.post(RESETPWD_URL, { email: email, code: theCode, password: password });
			if (response?.status == 200) {
				setSubmitted(true);
				setTimeout(() => {
					navigate("/login");
				}, 8000);
			}
		} catch (err: any) {
			if (!err?.response) {
				if (err.response?.data) {
					setErrorMessage(err.response?.data);
				} else {
					setErrorMessage("Something went wrong on the server. Our admins have been alerted.");
				}
			}
		}
	};

	return (
		<PageContainer>
			<div className="mx-auto flex flex-col items-center justify-center px-6 py-8 lg:py-4">
				<a href="#" className="mb-6 flex items-center text-2xl font-semibold text-gray-800 dark:text-white">
					<img className="mr-2 h-8 w-8" src="/img/dpm32.png" alt="logo"></img>DPM
				</a>

				<div className="w-full rounded-lg bg-white shadow dark:border dark:border-gray-700 dark:bg-gray-800 sm:max-w-md md:mt-0 xl:p-0">
					<div className="space-y-4 p-6 sm:p-8 md:space-y-6">
						<h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 dark:text-white md:text-2xl">Reset Password</h1>
						<p className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
							{errMsg}
						</p>

						{!submitted && (
							<form className="space-y-4 md:space-y-6" onSubmit={handleSubmit}>
								<div>
									<label htmlFor="email" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
										Your email
									</label>
									<Input
										type="email"
										name="email"
										id="email"
										className="focus:ring-primary-600 focus:border-primary-600 block w-full rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm"
										onChange={(e) => setEmail(e.target.value)}
										placeholder="name@company.com"
										required />
								</div>
								<div>
									<label htmlFor="password" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
										Password
									</label>
									<input
										type="password"
										name="password"
										id="password"
										ref={passwordRef}
										className="focus:ring-primary-600 focus:border-primary-600 block w-full rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm"
										onChange={(e) => setPassword(e.target.value)}
										required></input>
								</div>
								<div>
									<label htmlFor="confirm-password" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
										Confirm password
									</label>
									<input
										type="password"
										name="confirm-password"
										id="confirm-password"
										className="focus:ring-primary-600 focus:border-primary-600 block w-full rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm"
										onChange={(e) => setConfirmPassword(e.target.value)}
										required></input>
								</div>

								<Button type="submit" className="w-full">
									Reset Password
								</Button>
							</form>
						)}
						{submitted && (
							<div>
								<p>If an account exists with that email, we reset your password. If not, well then we didn't</p>
							</div>
						)}
					</div>
				</div>
			</div>
		</PageContainer>
	);
};

export default ResetPasswordPage;
