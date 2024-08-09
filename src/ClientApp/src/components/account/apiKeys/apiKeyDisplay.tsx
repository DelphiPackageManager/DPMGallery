import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import CopyButton from "@/components/ui/copyButton";
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { ApiKey } from "@/types/apiKeys";

export type ApiKeyDisplayProps = {
	apiKey: ApiKey | null;
	open: boolean;
	onOpenChange: (open: boolean) => void;
};

export const ApiKeyDisplay = (props: ApiKeyDisplayProps) => {
	const { apiKey, open, onOpenChange } = props;

	const keyValue = apiKey?.key ?? "";


	return (

		<Dialog open={open} onOpenChange={onOpenChange}>
			<DialogContent>
				<DialogHeader>
					<DialogTitle>New API Key Created</DialogTitle>
					<DialogDescription>New API key "{apiKey?.name}" has been created.</DialogDescription>
				</DialogHeader>
				<div className="flex flex-col items-start space-y-2">
					<div className="flex flex-row space-x-2">
						<Input id="textApiKey" type="text" readOnly={true} size={60} className="w-full outline-none focus:border-gray-300" value={keyValue} />
						{keyValue && <CopyButton title="Copy API key" text={keyValue} size="xl" />}
					</div>
					<div>
						<Alert variant="destructive" className="border-red-800 text-red-800 dark:border-red-600 dark:text-red-600">
							<AlertTitle>Make sure to copy your API key now.</AlertTitle>
							<AlertDescription> You will not be able to see it again after closing this dialog or refreshing the page.</AlertDescription>
						</Alert>
					</div>
				</div>
			</DialogContent>
		</Dialog>

	)
}
