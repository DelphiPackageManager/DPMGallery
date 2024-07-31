import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
//import useAuth from "../../hooks/useAuth";
import axios from "@/api/axios";
import PageHeader from "@/components/pageHeader";
import { Button } from "@/components/ui/button";
import PageContainer from "../../pageContainer";

type TwoFactorAuthModel = {
	twoFactorEnabled: boolean;
	hasAuthenticator: boolean;
	recoveryCodesLeft: number;
	isMachineRemembered: boolean;
} | null;

const TwoFactorAuthenticationPage = () => {
	//const { auth } = useAuth();
	const [twofaConfig, setTwoFaConfig] = useState<TwoFactorAuthModel>(null);
	const [errMsg, setErrorMessage] = useState("");

	useEffect(() => {
		const fetchConfig = async () => {
			try {
				const response = await axios.get("/ui/account/2fa-config");
				setErrorMessage("");
				setTwoFaConfig(response.data);
				//  console.log(response.data);
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

	const handleForgetBrowser = async (event: React.FormEvent<HTMLFormElement>) => {
		event.preventDefault();
		//console.log("forgetting machine");
		try {
			const response = await axios.post("/ui/account/2fa-forget");
			setErrorMessage("");
			setTwoFaConfig(response.data);
			//console.log(response.data);
		} catch (err: any) {
			if (err?.data) {
				setErrorMessage(err?.data);
			} else {
				setErrorMessage("An unkown error occured while fetching 2fa config : " + err?.status.toString());
			}
		}
	};

	return (
		<PageContainer>
			<PageHeader title="Two-factor authentication (2FA)" />
			<div className="pl-2">
				{errMsg !== "" && <h2>{errMsg}</h2>}
				{twofaConfig?.twoFactorEnabled && (
					<div>
						{twofaConfig.recoveryCodesLeft == 0 && (
							<div>
								<strong>You have no recovery codes left.</strong>
								<p>
									You must <Link to="/account/generaterecoverycodes">generate a new set of recovery codes</Link> before you can log in with a recovery
									code.
								</p>
							</div>
						)}
						{twofaConfig?.recoveryCodesLeft < 3 && (
							<div>
								<strong>You have `${twofaConfig.recoveryCodesLeft}` recovery code left.</strong>
								<p>
									You should <Link to="/account/generateRecoveryCodes">generate a new set of recovery codes</Link>.
								</p>
							</div>
						)}
						{twofaConfig?.isMachineRemembered && (
							<form className="inline-block" onSubmit={handleForgetBrowser}>
								<Button type="submit" variant="outline">
									Forget this browser
								</Button>
							</form>
						)}
						<div className="flex flex-row gap-3 pt-2">
							<Link to="/account/disable2fa">
								<Button variant="outline">Disable 2FA</Button>
							</Link>
							<Link to="/account/generaterecoverycodes">
								<Button variant="outline">Reset recovery codes</Button>
							</Link>
						</div>
					</div>
				)}
				<h4 className="mt-2">Authenticator app</h4>
				<div className="pt-2">
					{!twofaConfig?.hasAuthenticator && (
						<Link to="/account/enableauthenticator">
							<Button variant="outline">Add Authenticator app</Button>
						</Link>
					)}
					{twofaConfig?.hasAuthenticator && (
						<div className="flex flex-row gap-3">
							<Link to="/account/enableauthenticator">
								<Button>Set up Authenticator app</Button>
							</Link>
							<Link to="/account/resetauthenticator">
								<Button variant="destructive">Reset Authenticator app</Button>
							</Link>
						</div>
					)}
				</div>
			</div>
		</PageContainer>
	);
};

export default TwoFactorAuthenticationPage;
