import axios from "@/api/axios";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { AxiosError } from "axios";
import { QRCodeSVG } from "qrcode.react";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import PageContainer from "../../pageContainer";

type AuthenticatorDetailsModel = {
	sharedKey: string;
	authenticatorUri: string;
} | null;

const EnableAuthenticatorPage = () => {
	const [errMsg, setErrorMessage] = useState("");
	const [config, setConFig] = useState<AuthenticatorDetailsModel>(null);
	const [code, setCode] = useState("");
	const navigate = useNavigate();
	const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		try {
			const response = await axios.post("/ui/account/2fa-verify", { code: code });
			if (response.data?.codes) {
				navigate("/account/showrecoverycodes", {
					state: {
						codes: response.data.codes,
						message: "Your authenticator app has been verified.",
					},
				});
			} else {
				navigate("/account/twofactorauth");
			}
		} catch (err: any) {
			console.log(err);
			setErrorMessage(err?.response?.data);
		}
	};

	const onInput = (event: React.ChangeEvent<HTMLInputElement>) => {
		// event.preventDefault();
		setCode(event.target.value);
	};

	useEffect(() => {
		const fetchConfig = async () => {
			try {
				const response = await axios.get("/ui/account/2fa-keyinfo");
				setErrorMessage("");
				console.log(response.data);
				setConFig(response.data);
			} catch (err: any) {
				if (err?.data) {
					setErrorMessage(err?.data);
				} else {
					setErrorMessage("An unkown error occured while fetching 2fa config : " + err?.status?.toString());
				}
			}
		};
		fetchConfig();
	}, []);

	return (
		<PageContainer className="text-gray-800 dark:text-gray-100">
			<h3>Configure Authenticator app</h3>
			<div className="ml-2 mt-2 px-2 text-base font-light">
				<h3 className="py-2">To use an authenticator app go through the following steps:</h3>
				<ol className="list-disc">
					<li className="mb-2">
						<p>
							Download a two-factor authenticator app like Microsoft Authenticator for{" "}
							<a className="underline" href="https://go.microsoft.com/fwlink/?Linkid=825072">
								Android
							</a>{" "}
							and{" "}
							<a className="underline" href="https://go.microsoft.com/fwlink/?Linkid=825073">
								iOS
							</a>{" "}
							or Google Authenticator for{" "}
							<a className="underline" href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2&amp;hl=en">
								Android
							</a>{" "}
							and{" "}
							<a className="underline" href="https://itunes.apple.com/us/app/google-authenticator/id388497605?mt=8">
								iOS
							</a>
							.
						</p>
					</li>
					<li className="mb-2">
						<p>
							Scan the QR Code or enter this key{" "}
							<kbd className="rounded-md bg-gray-900 px-2 py-1 font-mono text-white dark:bg-gray-200 dark:text-gray-800">{`${config?.sharedKey}`}</kbd>{" "}
							into your two factor authenticator app. Spaces and casing do not matter.
						</p>

						{config?.authenticatorUri && (
							<QRCodeSVG value={`${config?.authenticatorUri}`} level="H" className="m-4 h-48 w-48 bg-white p-2 text-black" />
						)}
					</li>
					<li className="mb-2">
						<p className="mb-2 py-2">
							Once you have scanned the QR code or input the key above, your two factor authentication app will provide you with a unique code. Enter
							the code in the confirmation box below.
						</p>
						<div className="flex flex-row">
							<div className="">
								<form onSubmit={handleSubmit}>
									<div className="mb-3 flex flex-col">
										<label className="control-label form-label mb-1">Verification Code</label>
										<Input
											id="code"
											name="code"
											onChange={onInput}
											value={code}
											className="focus:ring-primary-600 focus:border-primary-600 block w-full rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm"
											autoComplete="off"
											placeholder="Please enter the code."
										/>
										{errMsg !== "" && <span className="text-danger"></span>}
									</div>

									<Button type="submit" className="w-full" disabled={!code}>
										Verify Code
									</Button>
									{errMsg !== "" && (
										<div className="text-red-600" role="alert">
											{errMsg}
										</div>
									)}
								</form>
							</div>
						</div>
					</li>
				</ol>
			</div>
		</PageContainer>
	);
};

export default EnableAuthenticatorPage;
