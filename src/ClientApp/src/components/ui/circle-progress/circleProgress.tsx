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
	}, [])

	return <div className="progress" ref={ref} aria-valuenow={value}></div>
}

export default CircleProgress;