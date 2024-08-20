import { Button } from "@/components/ui/button";
import { Dialog, DialogTrigger } from "@/components/ui/dialog";
import { UserOrganisation, UserOrganisationCreateModel } from "@/types/organisations";
import { PlusIcon } from "lucide-react";
import { useState } from "react";
import OrganisationCreateDialogContent from "./organisationDialogCreateContent";

interface OrganisationsTableActionsProps {
	onCreate: (newOrg: UserOrganisation) => void
}

export default function OrganisationsTableActions(props: OrganisationsTableActionsProps) {

	const { onCreate } = props;
	const [createOpen, setCreateOpen] = useState(false)
	const [dialogId, setDialogId] = useState(0);

	function onSuccess(data: UserOrganisation): void {
		setCreateOpen(false);
		onCreate(data);
	}

	function onCreateOpenChange(open: boolean): void {
		setCreateOpen(open);

		//reset dialog content on close
		if (!open)
			setDialogId(x => x + 1);
	}

	return (

		<Dialog open={createOpen} onOpenChange={onCreateOpenChange}>
			<DialogTrigger asChild>
				<Button className="ml-6" variant="create" size="sm"><span className="mr-2">Create new Organisation</span> <PlusIcon /></Button>
			</DialogTrigger>
			<OrganisationCreateDialogContent id={dialogId} onSuccess={onSuccess} />
		</Dialog>
	);

}
