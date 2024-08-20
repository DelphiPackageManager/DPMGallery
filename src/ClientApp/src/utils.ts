export const timeAgo = (date: Date) => {
	let now = new Date();
	const seconds: number = Math.floor((now.valueOf() - date.valueOf()) / 1000);

	let interval = Math.floor(seconds / 31536000);
	if (interval > 1) {
		return interval + " years ago";
	}

	interval = Math.floor(seconds / 2592000);
	if (interval > 1) {
		return interval + " months ago";
	}

	interval = Math.floor(seconds / 86400);
	if (interval > 1) {
		return interval + " days ago";
	}

	interval = Math.floor(seconds / 3600);
	if (interval > 1) {
		return interval + " hours ago";
	}

	interval = Math.floor(seconds / 60);
	if (interval > 1) {
		return interval + " minutes ago";
	}

	if (seconds < 10) return "just now";

	return Math.floor(seconds) + " seconds ago";
};

export const validateEmail = (email: string) => {
	if (!email) return false;
	// let regex = new RegExp(
	//   "([!#-'*+/-9=?A-Z^-~-]+(.[!#-'*+/-9=?A-Z^-~-]+)*|\"([]!#-[^-~ \t]|(\\[\t -~]))+\")@([!#-'*+/-9=?A-Z^-~-]+(.[!#-'*+/-9=?A-Z^-~-]+)*|[[\t -Z^-~]*])"
	// );
	// return regex.test(email);
	return String(email)
		.toLowerCase()
		.match(
			/^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
		);
};

export const UTCNow = () => {
	const now = new Date();

	const year = now.getUTCFullYear();
	const month = now.getUTCMonth() + 1;
	const day = now.getUTCDate();
	const hours = now.getUTCHours();
	const minutes = now.getUTCMinutes();
	const seconds = now.getUTCSeconds();
	const milliseconds = now.getUTCMilliseconds();

	const ts = `${year}-${month}-${day} 
              ${hours}:${minutes}:${seconds}.${milliseconds}`;

	return new Date(ts);
};

export class DateUtils {
	public static jsonToJSDate(value: string): any {
		var pattern = /Date\(([^)]+)\)/;
		var results = pattern.exec(value);
		if (results === null || results.length < 1) return null;
		var dt = new Date(parseFloat(results[1]));
		return dt;
	}
}

export function firstOrNull<T>(array: T[]): T | null {
	return array.length === 0 ? null : array[0];
}

export function formatBytes(bytes: number, decimals = 2) {
	if (!+bytes) return '0 Bytes'

	const k = 1024
	const dm = decimals < 0 ? 0 : decimals
	const sizes = ['Bytes', 'KiB', 'MiB', 'GiB', 'TiB', 'PiB', 'EiB', 'ZiB', 'YiB']

	const i = Math.floor(Math.log(bytes) / Math.log(k))

	return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`
}
