import { Button } from "@/components/ui/button";
import { CheckedState } from "@/components/ui/checkbox";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { UpdateEmailSchema } from "@/schemas";
import { UserOrganisation } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { ChangeEvent, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { UpdateOrganisationEmail } from "./organisationApi";


type EditOrganisationEmailProps = {
	organisation: UserOrganisation;
	setErrors: (value: string[]) => void;
}

const EditOrganisationEmail = ({ organisation, setErrors }: EditOrganisationEmailProps) => {

	const [changeEnabled, setChangeEnabled] = useState(false);
	const [currentEmail, setCurrentEmail] = useState(organisation.email);


	const updateEmailForm = useForm<z.infer<typeof UpdateEmailSchema>>({
		resolver: zodResolver(UpdateEmailSchema),
		defaultValues: {
			email: "",
		},
		mode: "onChange"
	})

	function updateEmailInputChange(event: ChangeEvent<HTMLInputElement> | CheckedState, field: any): void {

		field.onChange(event).then(() => {
			const dirtyFields = updateEmailForm.formState.dirtyFields;
			let isDirty = (dirtyFields.email ?? false);
			setChangeEnabled(isDirty);
		});
	}

	async function onSubmitUpdateEmail(values: z.infer<typeof UpdateEmailSchema>) {
		// Do something with the form values.
		// ✅ This will be type-safe and validated.
		let model = {
			id: organisation.id,
			email: values.email
		}

		var result = await UpdateOrganisationEmail(model);
		if (result.succeeded) {
			organisation.email = values.email;
			setCurrentEmail(values.email);
			//setCurrentOrg((prev) => prev ? { ...prev, email: values.email } : null);
			updateEmailForm.reset();
			setChangeEnabled(false);
			//navigate(location.pathname, { replace: true, state: null });
		}
		else {
			setErrors(result.errors);
		}
	}

	return (
		<Form {...updateEmailForm}>
			<form onSubmit={updateEmailForm.handleSubmit(onSubmitUpdateEmail)} className="space-y-8">
				<FormField
					control={updateEmailForm.control}
					name="email"
					render={({ field }) => (
						<FormItem>
							<p className="my-2 pt-2">Current Email Address : {currentEmail}</p>
							<div className="flex w-96 items-center">
								<FormLabel>New Email</FormLabel>
								<FormMessage className="flex-grow text-right" />
							</div>
							<FormControl>
								<Input placeholder="you@somewhere.com" size={55} {...field} required onChange={(e) => updateEmailInputChange(e, field)} />
							</FormControl>
						</FormItem>
					)}
				/>
				<Button type="submit" disabled={!changeEnabled} >Save changes</Button>
			</form>
		</Form>

	)

}

export default EditOrganisationEmail;
