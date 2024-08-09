import { ReactNode } from "react";

import { CircleAlert, X } from "lucide-react";
import { Alert, AlertDescription, AlertTitle } from "./ui/alert";
import { Button } from "./ui/button";
import { HoverCard, HoverCardContent, HoverCardTrigger } from "./ui/hover-card";

type ErrorDisplayCardType = {
	children?: ReactNode;
	errors: string[];
	errorDescription: string;
	clearErrors: () => void;
};

//TODO: Add close button to the card
export default function ErrorDisplayCard(props: ErrorDisplayCardType) {
	const { errors, errorDescription, clearErrors } = props;
	let open = errors && errors?.length > 0
	return <HoverCard open={open}>
		<HoverCardTrigger asChild>
			{props.children}
		</HoverCardTrigger>
		<HoverCardContent sideOffset={20} align="end" className="w-96 border-red-200 !p-0 dark:border-red-800">
			<Alert variant="destructive" className="bold border-0 pt-2">
				<AlertTitle className="m-0 flex justify-end">
					<Button variant="ghost" size="vsm_icon" onClick={clearErrors}><X /></Button>
				</AlertTitle>
				<AlertTitle className="m-0 flex dark:text-red-600">
					<CircleAlert />
					<div className="m-0 ml-3 mt-1 font-semibold">{errorDescription}</div>
				</AlertTitle>
				<AlertDescription className="ml-12 dark:font-semibold dark:text-red-600">
					<ul>
						{errors.map((error, index) => (
							<li className="hover:list-disc" key={index}>{error}</li>
						))}
					</ul>
				</AlertDescription>

			</Alert>

		</HoverCardContent>
	</HoverCard>;
}
