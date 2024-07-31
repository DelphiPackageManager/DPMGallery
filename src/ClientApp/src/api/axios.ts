import { ApiResult } from "@/types/api";
import axios, { AxiosError, AxiosInstance, isAxiosError } from "axios";

export default axios.create({
	headers: { "Content-Type": "application/json" },
	withCredentials: true,
});

export const createAxiosInitial = () => {
	// const refresh = async () => {
	//   const response = await axios.post("/ui/auth/refresh-token", {
	//     withCredentials: true, //send cookies
	//   });
	//   return response;
	// };

	const axiosInit = axios.create({
		headers: { "Content-Type": "application/json" },
		withCredentials: true,
	});

	axiosInit.interceptors.request.use(
		(config) => {
			return config;
		},
		(error) => Promise.reject(error)
	);

	// axiosInit.interceptors.response.use(
	//   (response) => response,
	//   async (error) => {
	//     const prevRequest = error?.config;
	//     //all the examples I found say 403, however aspnet returns 401
	//     if (error?.response?.status === 401 && !prevRequest?.sent) {
	//       prevRequest.sent = true;

	//       try {
	//         const refreshResponse = await refresh();
	//         if (refreshResponse.status !== 200) {
	//           return Promise.reject(refreshResponse.statusText);
	//         }
	//       } catch (error) {}

	//       return axiosInit(prevRequest);
	//     }
	//     return Promise.reject(error);
	//   }
	// );

	return axiosInit;
};



export function errorToResult(error: any): ApiResult {
	let message: string;

	if (isAxiosError(error)) {
		const axiosErr = error as AxiosError;
		const response = axiosErr.response;
		if (response && response?.status == 400 && response.data && typeof response.data === "string")
			message = response.data as string;
		else
			message = axiosErr.message;
	}
	else if (error instanceof Error)
		message = error.message;

	else
		message = String(error);

	let result = {
		succeeded: false,
		errors: [message]
	};
	return result;
}
