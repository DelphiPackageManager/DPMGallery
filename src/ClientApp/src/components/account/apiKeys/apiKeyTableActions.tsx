import { Button } from "@/components/ui/button";
import { Dialog, DialogTrigger } from "@/components/ui/dialog";
import { ApiKey } from "@/types/apiKeys";
import { useState } from "react";
import ApiKeyCreateDialogContent from "./apiKeyCreateDialogContent";

interface ApiKeyTableActionsProps {
	onCreate: (newKey: ApiKey) => void
}

export default function ApiKeyTableActions(props: ApiKeyTableActionsProps) {

	const { onCreate } = props;
	const [createOpen, setCreateOpen] = useState(false)
	const [apiKeyDialogId, setApiKeyDialogId] = useState(0);

	function onSuccess(data: ApiKey): void {
		setCreateOpen(false);
		onCreate(data);
	}

	function onCreateOpenChange(open: boolean): void {
		setCreateOpen(open);

		//reset dialog content on close
		if (!open)
			setApiKeyDialogId(x => x + 1);
	}

	return (

		<Dialog open={createOpen} onOpenChange={onCreateOpenChange}>
			<DialogTrigger asChild>
				<Button className="ml-6" variant="default" size="sm">Create new API Key</Button>
			</DialogTrigger>
			<ApiKeyCreateDialogContent id={apiKeyDialogId} onSuccess={onSuccess} />
		</Dialog>
	);

}
