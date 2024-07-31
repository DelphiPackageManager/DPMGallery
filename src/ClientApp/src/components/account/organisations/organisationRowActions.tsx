import { AlertDialog, AlertDialogTrigger } from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { Dialog, DialogTrigger } from "@/components/ui/dialog";
import { ApiResult } from "@/types/api";
import { UserOrganisation } from "@/types/organisations";
import { Pencil, X } from "lucide-react";
import { useState } from "react";
import OrganisationDeleteAlertContent from "./organisationDeleteAlertContent";
import OrganisationEditDialogContent from "./organisationDialogEditContent";

export default function OrganisationRowActions(props: { org: UserOrganisation, onUpdate: (org?: UserOrganisation | null) => void }) {
	const { org, onUpdate } = props;
	const [editOpen, setEditOpen] = useState(false)
	const [orgDialogId, setOrgDialogId] = useState(0);
	const [errors, setErrors] = useState([] as string[])

	function onEditSuccess(org: UserOrganisation): void {
		setEditOpen(false);
		onUpdate(org);
	}


	function onEditOpenChange(open: boolean): void {
		setEditOpen(open);

		//reset dialog content on close
		if (!open)
			setOrgDialogId(x => x + 1);
	}


	function onDeleteComplete(result: ApiResult): void {
		if (result.succeeded)
			onUpdate();
		else
			setErrors(result.errors); //TODO: show error message
	}

	return (<div className="flex">
		{org?.id &&
			<>
				<Dialog open={editOpen} onOpenChange={onEditOpenChange}>
					<DialogTrigger asChild>
						<Button title="Edit Organisation" size="vsm_icon" variant="ghost"><Pencil size={16} strokeWidth={1.5} /></Button>
					</DialogTrigger>
					<OrganisationEditDialogContent id={orgDialogId} editOrg={org} onSuccess={onEditSuccess} />
				</Dialog>
				<AlertDialog>

					<AlertDialogTrigger asChild>
						<Button className="enabled:hover:bg-red-100 disabled:pointer-events-auto" title="Delete Organisation" size="vsm_icon" variant="ghost"><X size={16} strokeWidth={1.5} /></Button>
					</AlertDialogTrigger>
					<OrganisationDeleteAlertContent orgId={org.id} onComplete={onDeleteComplete} />
				</AlertDialog>
			</>}
	</div>)
}