import { differenceInDays } from 'date-fns';

export function expiryDays(dateTime: Date | string) {
	const expiryDate = new Date(dateTime);
	const now = new Date();

	const expiryDays = differenceInDays(expiryDate, now);
	return expiryDays;
}