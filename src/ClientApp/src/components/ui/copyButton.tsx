

import { cn } from "@/lib/utils";
import { Check, Copy } from "lucide-react";
import { useEffect, useState } from "react";
import { Button } from "./button";

export default function CopyButton(props: { text: string, title: string, size?: "sm" | "md" | "lg" | "xl" | "xxl" }) {
	const [copied, setCopied] = useState(false);

	function copyText() {
		navigator.clipboard.writeText(props.text);
		setCopied(true);
	};

	useEffect(() => {
		const timeout = setTimeout(() => {
			if (copied)
				setCopied(false);
		}, 1500);
		return () => clearTimeout(timeout);
	}, [copied]);

	const sizeNumber = props.size === "sm" ? 8 : props.size === "md" ? 10 : props.size === "lg" ? 12 : props.size === "xl" ? 14 : props.size === "xxl" ? 16 : 8;
	const copiedTextSize = props.size === "sm" ? "text-vsm" : props.size === "md" ? "text-sm" : props.size === "lg" ? "text-md" : props.size === "xl" ? "text-md" : props.size === "xxl" ? "text-lg" : "text-vsm";
	const buttonSize = props.size === "sm" ? "vvsm_icon" : props.size === "md" ? "vsm_icon" : props.size === "lg" ? "sm_icon" : props.size === "xl" ? "sm" : props.size === "xxl" ? "default" : "vvsm_icon";
	const textTopMargin = props.size === "sm" ? "mt-[1px]" : props.size === "md" ? "mt-[2px]" : props.size === "lg" ? "mt-[6px]" : props.size === "xl" ? "mt-[8px]" : props.size === "xxl" ? "mt-[8px]" : "mt-[1px]";

	return (
		<div className="flex overflow-visible">
			<Button title={props.title} onClick={copyText} className="mb-2 flex" size={buttonSize} variant="ghost">
				<div className="relative h-2 w-2">
					<Copy className="absolute left-0 top-0 z-0" size={sizeNumber} strokeWidth={1.5} visibility={copied ? "hidden" : "visible"} />
					<Check className="absolute left-0 top-0 z-10 text-green-500 transition-opacity" size={12} strokeWidth={2} visibility={copied ? "visible" : "hidden"} />
				</div>
			</Button>
			<div className={cn(`ml-1  text-green-500 transition-opacity`, copied ? `visible` : `hidden`, copiedTextSize, textTopMargin)}>copied</div>
		</div>
	);
}