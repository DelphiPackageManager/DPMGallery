

import { AlertDialog, AlertDialogTrigger } from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { Dialog, DialogTrigger } from "@/components/ui/dialog";
import { ApiResult } from "@/types/api";
import { ApiKey } from "@/types/apiKeys";
import { RefreshCcwDot, X } from "lucide-react";
import { useState } from "react";
import ApiKeyDeleteAlertContent from "./apiKeyDeleteAlertContent";
import ApiKeyRegenerateDialogContent from "./apiKeyRegenerateDialogContent";

export default function ApiKeyRowActions(props: { apiKey: ApiKey, onUpdate: (apiKey?: ApiKey | null) => void }) {

	const { apiKey, onUpdate } = props;
	const [editOpen, setEditOpen] = useState(false)
	const [apiKeyDialogId, setApiKeyDialogId] = useState(0);
	const [errors, setErrors] = useState([] as string[])

	function onRegenerateSuccess(apiKey: ApiKey): void {
		setEditOpen(false);
		onUpdate(apiKey);
	}

	function onEditOpenChange(open: boolean): void {
		setEditOpen(open);

		//reset dialog content on close
		if (!open)
			setApiKeyDialogId(x => x + 1);
	}

	function onDeleteComplete(result: ApiResult): void {
		if (result.succeeded)
			onUpdate();
		else
			setErrors(result.errors); //TODO: show error message
	}

	return (
		<div className="flex">
			{apiKey?.id &&
				<>
					<Dialog open={editOpen} onOpenChange={onEditOpenChange}>
						<DialogTrigger asChild>
							<Button title="Regenerate API key" size="vsm_icon" variant="ghost"><RefreshCcwDot size={16} strokeWidth={1.5} /></Button>
						</DialogTrigger>
						<ApiKeyRegenerateDialogContent id={apiKeyDialogId} apiKey={apiKey} onSuccess={onRegenerateSuccess} />
					</Dialog>
					<AlertDialog>

						<AlertDialogTrigger asChild>
							<Button className="enabled:hover:bg-red-100 disabled:pointer-events-auto" title="Delete API key" size="vsm_icon" variant="ghost"><X size={16} strokeWidth={1.5} /></Button>
						</AlertDialogTrigger>
						<ApiKeyDeleteAlertContent apiKeyId={apiKey.id} onComplete={onDeleteComplete} />
					</AlertDialog>
				</>}
		</div>
	);
}

