
export type ApiResult = {
	succeeded: boolean;
	errors: string[];
	data?: any;
}


export type ProblemDetails = {
	title: string;
	status: number;
	detail: string;
	type: string;
}
