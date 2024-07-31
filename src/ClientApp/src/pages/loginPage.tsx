import { SITE_URL } from "@/types/constants";
import * as React from "react";
import { useEffect, useRef, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import axios from "../api/axios";
import Meta from "../components/meta";
import PageContainer from "../components/pageContainer";
import { Button } from "../components/ui/button";
import { AuthContextInterface, User } from "../context/AuthProvider";
import useAuth from "../hooks/useAuth";

const LoginPage = () => {
	const { login, logout } = useAuth();

	const navigate = useNavigate();
	const location = useLocation();
	const from = location.state?.from || "/";

	const errRef = useRef<HTMLParagraphElement>(null);

	const [user, setUser] = useState("");
	const [pwd, setPwd] = useState("");
	const [rememberMe, setRememberMe] = useState(false);
	const [errMsg, setErrorMessage] = useState("");

	//clear the error when ever the user or pwd changes
	useEffect(() => {
		setErrorMessage("");
	}, [user, pwd]);

	const LOGIN_URL = "/ui/auth/login";

	const EXTERNAL_LOGIN = `/ui/auth/external-login?returnurl=${from}`;

	const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
		event.preventDefault();

		try {
			const response = await axios.post(LOGIN_URL, { username: user, password: pwd, rememberMe: rememberMe });
			if (response?.data?.requires2fa) {
				logout(); //why?
				navigate("/loginwith2fa", {
					state: {
						from: from,
						rememberMe: rememberMe,
					},
				});
				return;
			}
			if (response?.data?.lockedOut) {
				navigate("/lockedout");
				return;
			}
			const username = response?.data?.userName;
			const email = response?.data?.email;
			const emailConfirmed = response?.data?.emailConfirmed;
			const roles = response?.data?.roles;
			const avatarUrl = response?.data?.avatarUrl;
			const twoFactorEnabled = response?.data?.twoFactorEnabled;

			const loginUser: User = {
				userName: username,
				email: email,
				emailConfirmed: emailConfirmed,
				roles: roles,
				avatarUrl: avatarUrl,
				twoFactorEnabled: twoFactorEnabled,
			};
			login(loginUser);
			navigate(from, { replace: true });
		} catch (err: any) {
			console.log("error during login");
			console.log(err);
			if (!err?.response) {
				setErrorMessage("No Server Response");
			} else if (err.response?.status === 400) {
				setErrorMessage("Missing Username or Password");
			} else if (err.response?.status === 401) {
				if (err.response?.data) {
					setErrorMessage(err.response?.data);
				} else {
					setErrorMessage("Invalid username or password");
				}
			} else {
				setErrorMessage("Login Failed : Error code :  " + `${err?.response?.status}`);
			}
			logout();
			errRef.current?.focus();
		}
	};

	return (
		<PageContainer className="">
			<Meta title="DPM - Login" canonical={`${SITE_URL}/login`} />

			<div>
				<p ref={errRef} className={errMsg ? "errmsg" : "offscreen"} aria-live="assertive">
					{errMsg}
				</p>
				<div className="mx-auto flex flex-col items-center justify-center px-6 py-8 lg:py-4">
					<a href="#" className="mb-6 flex items-center text-2xl font-semibold text-gray-800 dark:text-white">
						<img className="mr-2 h-8 w-8" src="/img/dpm32.png" alt="logo"></img>DPM
					</a>
					<div className="w-full rounded-lg border bg-white shadow-md shadow-gray-200 dark:border-gray-700 dark:bg-gray-800 dark:shadow-none dark:shadow-gray-800 sm:max-w-md md:mt-0 xl:p-0">
						<div className="space-y-4 p-6 sm:p-8 md:space-y-6">
							<h1 className="text-xl font-bold leading-tight tracking-tight text-gray-900 dark:text-white md:text-2xl">Log In to your account</h1>
							<form className="space-y-4 md:space-y-6" onSubmit={handleSubmit}>
								<div>
									<label htmlFor="userName" className="mb-2 block text-sm font-medium text-gray-800 dark:text-white">
										UserName or Email Address
									</label>
									<input
										type="text"
										name="userName"
										id="userName"
										autoComplete="username"
										onChange={(e) => setUser(e.target.value)}
										value={user}
										className="focus:ring-primary-600 focus:border-primary-600 block w-full rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm"
										required
										autoFocus></input>
								</div>
								<div>
									<label htmlFor="password" className="mb-2 block text-sm font-medium text-gray-900 dark:text-white">
										Password
									</label>
									<input
										type="password"
										name="password"
										id="password"
										autoComplete="current-password"
										onChange={(e) => setPwd(e.target.value)}
										value={pwd}
										className="focus:ring-primary-600 focus:border-primary-600 block w-full rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm"
										required></input>
								</div>
								<div className="flex items-center justify-between">
									<div className="flex items-start">
										<div className="flex h-5 items-center">
											<input
												id="remember"
												aria-describedby="remember"
												type="checkbox"
												onChange={(e) => setRememberMe(e.target.checked)}
												value={rememberMe ? "checked" : "unchecked"}
												className="focus:ring-3 focus:ring-primary-300 dark:focus:ring-primary-600 h-4 w-4 rounded border border-gray-300 bg-gray-50 dark:border-gray-600 dark:bg-gray-700 dark:ring-offset-gray-800"></input>
										</div>
										<div className="ml-3 text-sm">
											<label htmlFor="remember" className="text-gray-500 dark:text-gray-300">
												Remember me
											</label>
										</div>
									</div>
									<Link to="/forgotpassword" className="text-primary-600 dark:text-primary-500 text-sm font-medium hover:underline">
										Forgot password?
									</Link>
								</div>
								<Button type="submit" className="w-full">
									Log In
								</Button>
								<p className="text-sm font-light text-gray-500 dark:text-gray-400">
									Don’t have an account yet?{" "}
									<Link to="/createaccount" replace className="text-primary-600 dark:text-primary-500 font-medium hover:underline">
										Create an Account
									</Link>
								</p>
							</form>
						</div>
					</div>
					<div className="pt-3">
						<div className="flex items-center">
							<div className="h-0.5 w-full bg-gray-200 dark:bg-gray-700"></div>
							<div className="px-5 text-center text-gray-500 dark:text-gray-400">or</div>
							<div className="h-0.5 w-full bg-gray-200 dark:bg-gray-700"></div>
						</div>

						<form method="POST" action={EXTERNAL_LOGIN}>
							<input name="returnurl" className="hidden" defaultValue={from} />
							<div className="flex items-center justify-center gap-4 py-4">
								<Button name="provider" value="Google" variant="secondary">
									<svg className="h-4 w-4" fill="currentColor">
										<use href="#google" />
									</svg>

									<span className="pl-2">Log In with Google</span>
								</Button>
								<Button variant="secondary" name="provider" value="GitHub">
									<svg className="h-5 w-5">
										<use href="#github" />
									</svg>
									<span className="pl-2">Log In with GitHub</span>
								</Button>
							</div>
						</form>
					</div>
				</div>
			</div>
		</PageContainer>
	);
};

export default LoginPage;
