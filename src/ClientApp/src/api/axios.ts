import { ApiResult, ProblemDetails } from "@/types/api";
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
		timeout: 1000 * 10,
		timeoutErrorMessage: "timedOut"
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
	let message: string[] = [];

	if (isAxiosError(error)) {
		const axiosErr = error as AxiosError;
		const response = axiosErr.response;
		if (response?.data) {
			if (response?.status == 400 && typeof response.data === "string")
				message.push(response.data as string);
			else {
				let problemDetails = (response.data as ProblemDetails);
				if (problemDetails) {
					message.push(problemDetails.title);
					message.push(problemDetails.detail);
				}
			}
		}
		else
			message.push(axiosErr.message);
	}
	else if (error instanceof Error)
		message.push(error.message);
	else
		message.push(String(error));

	let result = {
		succeeded: false,
		errors: message
	};
	return result;
}
