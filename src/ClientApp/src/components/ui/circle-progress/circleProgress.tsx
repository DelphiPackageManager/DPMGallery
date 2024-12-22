import { useEffect } from "react";
import useProgress from "./useProgress";

type CircleProgressProps = {
	value: number;
	size: number;
}


const CircleProgress = ({ size, value }: CircleProgressProps) => {
	const { ref } = useProgress(value);

	useEffect(() => {
		ref?.current?.style.setProperty("--size", size + "px");
	}, [value])

	return (
		<div className="progress" ref={ref} aria-valuenow={value}>
			<div className="progress-inner">
				<svg fill="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path d="M12,2A10,10,0,1,0,22,12,10,10,0,0,0,12,2Zm5.676,8.237-6,5.5a1,1,0,0,1-1.383-.03l-3-3a1,1,0,1,1,1.414-1.414l2.323,2.323,5.294-4.853a1,1,0,1,1,1.352,1.474Z" /></svg>
			</div>
		</div>
	)
}

export default CircleProgress;