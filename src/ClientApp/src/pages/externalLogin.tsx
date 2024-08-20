import axios from "@/api/axios";
import { Fragment, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { AxiosError } from "axios";
import PageContainer from "../components/pageContainer";
import { Button } from "../components/ui/button";
import { User } from "../context/AuthProvider";
import useAuth from "../hooks/useAuth";

type ExternalDetails = {
	email: string;
	providerDisplayName: string;
} | null;

type ExternalLoginModel = {
	email: string;
	userName: string;
} | null;

const ExternalLoginPage = () => {
	const { login } = useAuth();
	const [userName, setUserName] = useState("");
	const [email, setEmail] = useState("");
	const [details, setDetails] = useState<ExternalDetails>(null);
	const [errMsg, setErrorMessage] = useState("");
	let { returnUrl } = useParams() || "/";
	let { remoteError } = useParams() || false;
	const navigate = useNavigate();

	const fetchDetails = async () => {
		try {
			const response = await axios.get<ExternalDetails>("/ui/auth/external-details");
			if (response?.data) {
				setDetails(response.data);
				setEmail(response.data.email);
			}
		} catch (err: any) {
			if (err?.message) {
				setErrorMessage(err.message);
			} else {
				setErrorMessage("Error fetching external login details - Unknown error");
			}
		}
	};

	useEffect(() => {
		if (remoteError) {
			setErrorMessage("Error from external provider: " + remoteError);
		} else {
			fetchDetails();
		}
	}, []);

	const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
		event.preventDefault();

		const model: ExternalLoginModel = {
			email,
			userName,
		};
		try {
			const response = await axios.post("/ui/auth/external-create-account", model);
			if (response?.status == 200) {
				const username = response?.data?.userName;
				const email = response?.data?.email;
				const emailConfirmed = response?.data?.emailConfirmed;
				const roles = response?.data?.roles;
				const avatarUrl = response?.data?.avatarUrl;
				const twoFactorEnabled = response?.data?.twoFactorEnabled;

				const newUser: User = {
					userName: username,
					email: email,
					emailConfirmed: emailConfirmed,
					roles: roles,
					avatarUrl: avatarUrl,
					twoFactorEnabled: twoFactorEnabled,
				};
				login(newUser);
				navigate(returnUrl ? returnUrl : "/", { replace: true });
				//        }
			}
		} catch (err: any) {
			if (err?.response?.data) {
				setErrorMessage("Error creating external login :  " + err?.response?.data);
			} else {
				setErrorMessage("Error creating external login :  " + "unknown error");
			}
		}
	};

	return (
		<PageContainer>
			<h1>Create Account</h1>
			<p className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
				{errMsg}
			</p>
			{!remoteError && (
				<div>
					<h2 id="external-login-title" className="mt-2">
						Associate your {`${details?.providerDisplayName}`} account.
					</h2>
					<hr />
					{!errMsg && (
						<p id="external-login-description" className="text-info mb-4 mt-4">
							You've successfully authenticated with <strong>{`${details?.providerDisplayName}`}</strong>. Please enter a username/email for this site
							below and click the Create Account button to finish logging in.
						</p>
					)}
					<div className="mx-auto flex w-full flex-col items-center justify-center px-6 py-8 lg:py-4">
						<div className="rounded-lg border bg-white shadow-md shadow-gray-200 dark:border-gray-700 dark:bg-gray-800 dark:shadow-none dark:shadow-gray-800 sm:max-w-md md:mt-0 xl:p-0">
							<div className="space-y-4 p-6 sm:p-8 md:space-y-6">
								<form className="flex flex-row space-y-4 md:space-y-6" onSubmit={handleSubmit}>
									<input hidden id="returnUrl" name="returnUrl" />
									<div className="mb-3 flex flex-col gap-3">
										<label htmlFor="userName" className="text-sm font-medium text-gray-800 dark:text-white">
											UserName
										</label>

										<input
											className="focus:ring-primary-600 focus:border-primary-600 mb-2 block w-full rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm"
											autoComplete="user"
											id="userName"
											name="userName"
											placeholder="Please enter your username."
											value={userName}
											size={200}
											onChange={(e) => setUserName(e.target.value)}
										/>
										<label htmlFor="email" className="text-sm font-medium text-gray-800 dark:text-white">
											Email
										</label>
										<input
											className="focus:ring-primary-600 focus:border-primary-600 block w-full rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm"
											id="email"
											name="email"
											autoComplete="email"
											placeholder="Please enter your email."
											value={email}
											onChange={(e) => setEmail(e.target.value)}
										/>
										<Button type="submit">Create Account</Button>
									</div>
								</form>
							</div>
						</div>
					</div>
				</div>
			)}
		</PageContainer>
	);
};

export default ExternalLoginPage;
