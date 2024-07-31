import { AlertDialog, AlertDialogTrigger } from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { Dialog, DialogTrigger } from "@/components/ui/dialog";
import { ApiResult } from "@/types/api";
import { UserOrganisation } from "@/types/organisations";
import { Pencil, X } from "lucide-react";
import { useState } from "react";
import { Link } from "react-router-dom";
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
			<div className="flex flex-row items-center gap-1">
				<Link to={`/account/organisation/${org.name.toLowerCase()}`} className="rounded-md p-1.5 hover:bg-primary hover:text-white" ><Pencil size={16} strokeWidth={1.5} /></Link>
				<AlertDialog>
					<AlertDialogTrigger asChild>
						<Button className="hover:text-white enabled:hover:bg-red-500 disabled:pointer-events-auto dark:hover:bg-red-600" title="Delete Organisation" size="vsm_icon" variant="ghost"><X size={16} strokeWidth={1.5} /></Button>
					</AlertDialogTrigger>
					<OrganisationDeleteAlertContent orgId={org.id} onComplete={onDeleteComplete} />
				</AlertDialog>
			</div>}
	</div>)
}