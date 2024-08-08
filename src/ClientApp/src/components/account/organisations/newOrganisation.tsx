import axios from "@/api/axios";
import { Input } from "@/components/ui/input";
import Modal from "@/components/ui/modal";
import { Spinner } from "@/components/ui/spinner";
import * as Form from "@radix-ui/react-form";
import { FormEvent, useEffect, useState } from "react";

export type NewOrganisationProps = {
	afterSave(): void;
};

const CHECKUSEREXISTs_URL = "/ui/account/user-exists";

const NewOrganisation = ({ afterSave }: NewOrganisationProps) => {
	const [orgName, setOrgName] = useState("");
	const [debouncedOrgName, setDebouncedOrgName] = useState("");
	const [orgExists, setOrgExists] = useState(false);
	const [saving, setSaving] = useState(false);

	useEffect(() => {
		const delayInputTimeoutId = setTimeout(() => {
			setDebouncedOrgName(orgName);
		}, 500);
		return () => clearTimeout(delayInputTimeoutId);
	}, [orgName, 500]);

	async function checkUserExists(orgName: string) {
		if (orgName === "") return;
		try {
			let url = `${CHECKUSEREXISTs_URL + "/" + orgName}`;
			const response = await axios.get(url);
			if (response?.status == 200) {
				setOrgExists(true);
			}
		} catch (err: any) {
			setOrgExists(false);
		}
	}

	useEffect(() => {
		checkUserExists(debouncedOrgName);
	}, [debouncedOrgName]);

	async function handleSubmit(event: FormEvent<HTMLFormElement>) {
		event.preventDefault();
		setSaving(true);

		//let data = Object.fromEntries(new FormData(event.currentTarget));
		//await updateContact(contact.id, data);
		afterSave();
	}

	return (
		<Form.Root className="w-full" onSubmit={handleSubmit}>
			<div className="flex h-full w-full flex-col">
				<fieldset disabled={saving} className="group flex w-full grow flex-col justify-between">
					<div className="grow">
						<Form.Field name="newOrg" className="mt-2 flex flex-col">
							<div className="flex max-w-sm items-baseline justify-between">
								<Form.Label>Organisation Name</Form.Label>
								<Form.Message className="text-sm text-red-700 dark:text-orange-700" match="valueMissing">
									Please enter the org name.
								</Form.Message>
								<Form.Message className="text-sm text-red-700 dark:text-orange-700" match="typeMismatch" forceMatch={orgExists}>
									Organisation or user exists
								</Form.Message>
							</div>
							<Form.Control asChild required>
								<Input
									id="newOrg"
									name="newOrg"
									size={60}
									type="text"
									className="w-96"
									value={orgName}
									onChange={(e) => setOrgName(e.currentTarget.value)}
								/>
							</Form.Control>
							<p className="mt-1 text-sm text-muted-foreground">This will be your organization account on https://delphi.dev/profiles/{orgName}</p>
						</Form.Field>

						<Form.Field name="newEmail" className="mt-2 flex flex-col">
							<div className="flex max-w-sm items-baseline justify-between">
								<Form.Label>New Email</Form.Label>
								<Form.Message className="text-sm text-red-700 dark:text-orange-700" match="typeMismatch">
									Please provide a valid email address
								</Form.Message>
								<Form.Message className="text-sm text-red-700 dark:text-orange-700" match="valueMissing">
									Please enter an email address
								</Form.Message>
							</div>
							<Form.Control asChild required>
								<Input id="newEmail" name="newEmail" size={60} type="email" className="w-96"></Input>
							</Form.Control>
						</Form.Field>
					</div>

					<div className="mt-8 w-full space-x-6 text-right">
						<Modal.Close className="rounded px-4 py-2 text-sm font-medium text-gray-500 hover:text-gray-600">Cancel</Modal.Close>
						<Form.Submit asChild>
							<button
								className="inline-flex items-center justify-center rounded bg-green-500 px-4 py-2 text-sm font-medium text-white hover:bg-green-600 group-disabled:pointer-events-none"
								type="submit">
								<Spinner className="absolute h-4 group-enabled:opacity-0" />
								<span className="group-disabled:opacity-0">Save</span>
							</button>
						</Form.Submit>
					</div>
				</fieldset>
			</div>
		</Form.Root>
	);
};

export default NewOrganisation;
