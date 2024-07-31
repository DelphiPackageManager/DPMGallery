
import { AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogDestructiveAction, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";
import { ApiResult } from "@/types/api";
import { deleteApiKey } from "./apiKeysApi";

export default function ApiKeyDeleteAlertContent(props: { apiKeyId: number, onComplete: (result: ApiResult) => void }) {

	const { apiKeyId, onComplete } = props;

	async function onContinue() {
		if (!apiKeyId)
			return;
		const result = await deleteApiKey(apiKeyId);
		onComplete(result);
	}

	return (

		<AlertDialogContent>
			<AlertDialogHeader>
				<AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
				<AlertDialogDescription>
					This action cannot be undone. This will permanently delete the API key from the server. Alternatively, you can disable the API key using the checkbox in the Enabled column.
				</AlertDialogDescription>
			</AlertDialogHeader>
			<AlertDialogFooter>
				<AlertDialogCancel>Cancel</AlertDialogCancel>
				<AlertDialogDestructiveAction onClick={onContinue}>Continue</AlertDialogDestructiveAction>
			</AlertDialogFooter>
		</AlertDialogContent>

	);

}

