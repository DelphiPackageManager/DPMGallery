
import { AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogDestructiveAction, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";
import { ApiResult } from "@/types/api";
import { deleteOrganisation } from "./organisationApi";

export default function OrganisationDeleteAlertContent(props: { orgId: number, onComplete: (result: ApiResult) => void }) {

	const { orgId, onComplete } = props;

	async function onContinue() {
		if (!orgId)
			return;
		const result = await deleteOrganisation(orgId);
		onComplete(result);
	}

	return (

		<AlertDialogContent>
			<AlertDialogHeader>
				<AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
				<AlertDialogDescription>
					This action cannot be undone. This will permanently delete the Organisation from the server.
				</AlertDialogDescription>
			</AlertDialogHeader>
			<AlertDialogFooter>
				<AlertDialogCancel>Cancel</AlertDialogCancel>
				<AlertDialogDestructiveAction onClick={onContinue}>Continue</AlertDialogDestructiveAction>
			</AlertDialogFooter>
		</AlertDialogContent>

	);

}

