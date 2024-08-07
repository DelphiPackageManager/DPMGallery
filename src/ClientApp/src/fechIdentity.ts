import { createAxiosInitial } from "./api/axios";
import { User } from "./context/AuthProvider";

const IDENTITY_URL = "/ui/auth/identity";

const fetchIdentity = async (): Promise<User | null> => {
	const axiosInitial = createAxiosInitial();
	try {
		//using axios initial so we get the new refresh token.
		const response = await axiosInitial.post(
			IDENTITY_URL,
			{},
			{
				withCredentials: true,
			}
		);
		if (!response.data) {
			return null;
		}

		const id = response?.data?.id;
		const username = response?.data?.userName;
		const email = response?.data?.email;
		const emailConfirmed = response?.data?.emailConfirmed;
		const roles = response?.data?.roles;
		const avatarUrl = response?.data?.avatarUrl;
		const twoFactorEnabled = response?.data?.twoFactorEnabled;

		const result: User = {
			id: id,
			userName: username,
			email: email,
			emailConfirmed: emailConfirmed,
			roles: roles,
			avatarUrl: avatarUrl,
			twoFactorEnabled: twoFactorEnabled,
		};
		return result;
	} catch (err) {
		return null;
	}
};

export default fetchIdentity;
