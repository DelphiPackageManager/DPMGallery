import { createRef, useEffect } from "react";

const useProgress = (value: number) => {

	const ref = createRef<HTMLDivElement>();

	useEffect(() => {
		if (!ref.current)
			return;
		ref.current.style.setProperty("--progress", value + "%");
		ref.current.setAttribute("aria-valuenow", value.toString());

	}, [value])

	return { ref };
}

export default useProgress;