export function closestTo(goal: number, possibleValues: number[]) {
	return possibleValues.reduce(function (prev, curr) {
		return (Math.abs(curr - goal) < Math.abs(prev - goal) ? curr : prev);
	});
}
