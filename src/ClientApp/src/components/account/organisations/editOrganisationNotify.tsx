import { Button } from "@/components/ui/button";
import { Checkbox, CheckedState } from "@/components/ui/checkbox";
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel } from "@/components/ui/form";
import { UpdateNotificationSchema } from "@/schemas";
import { UserOrganisation } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { ChangeEvent, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { UpdateOrganisationSettings } from "./organisationApi";

type EditOrganisationNotifyProps = {
	organisation: UserOrganisation;
	setErrors: (value: string[]) => void;
}

const EditOrganisationNotify = ({ organisation, setErrors }: EditOrganisationNotifyProps) => {
	const [changeEnabled, setChangeEnabled] = useState(false);


	const updateNotificationForm = useForm<z.infer<typeof UpdateNotificationSchema>>({
		resolver: zodResolver(UpdateNotificationSchema),
		defaultValues: {
			allowContact: organisation.allowContact,
			notifyOnPublish: organisation.notifyOnPublish,
		},
		mode: "onSubmit"
	})


	function updateNotificationInputChange(event: ChangeEvent<HTMLInputElement> | CheckedState, field: any): void {

		field.onChange(event).then(() => {
			const dirtyFields = updateNotificationForm.formState.dirtyFields;
			let isDirty = (dirtyFields.allowContact ?? false) || (dirtyFields.notifyOnPublish ?? false);
			setChangeEnabled(isDirty);
		});
	}


	async function onSubmitUpdateNotifications(values: z.infer<typeof UpdateNotificationSchema>) {

		let model = {
			id: organisation.id,
			allowContact: values.allowContact,
			notifyOnPublish: values.notifyOnPublish
		};

		var result = await UpdateOrganisationSettings(model);
		if (result.succeeded) {
			organisation.allowContact = model.allowContact;
			organisation.notifyOnPublish = model.notifyOnPublish;

			//setCurrentOrg((prev) => prev ? { ...prev, email: values.email } : null);
			updateNotificationForm.reset({
				allowContact: organisation.allowContact,
				notifyOnPublish: organisation.notifyOnPublish
			});
			setChangeEnabled(false);
			//navigate(location.pathname, { replace: true, state: null });
		}
		else {
			setErrors(result.errors);
		}
	}



	return (
		<Form {...updateNotificationForm}>
			<form onSubmit={updateNotificationForm.handleSubmit(onSubmitUpdateNotifications)}>
				<FormField
					control={updateNotificationForm.control}
					name="allowContact"
					render={({ field }) => (
						<FormItem>
							<div className="flex items-center">
								<FormControl>
									<Checkbox id="allowContact" checked={field.value} onCheckedChange={(event) => updateNotificationInputChange(event, field)} >Allow Contact</Checkbox>
								</FormControl>
								<FormLabel controlType="checkbox" htmlFor="allowContact" className="ml-2">Users can contact the organisation through the DPM Gallery</FormLabel>
							</div>
							<FormDescription className="ml-6">This setting allows other registered users of the site to contact your organization about packages that it owns using the Contact Owners form, or to request that your organization become an owner of their package. Disabling this setting means users cannot contact your organization for these reasons.</FormDescription>
						</FormItem>
					)} />
				<FormField
					control={updateNotificationForm.control}
					name="notifyOnPublish"
					render={({ field }) => (
						<FormItem>
							<div className="flex items-center">
								<FormControl>
									<Checkbox id="notifyOnPublish" checked={field.value} onCheckedChange={(event) => updateNotificationInputChange(event, field)} className="" ></Checkbox>
								</FormControl>
								<FormLabel controlType="checkbox" htmlFor="notifyOnPublish" className="ml-2" >Notify when a package is published to DPM Gallery</FormLabel>
							</div>
							<FormDescription className="ml-6">This setting enables notifications whenever a package is published to your account. We recommend enabling this setting so that you can inspect whether a package was published intentionally and so that you can be notified if there are unexpected delays in publishing your package.
								Note: We will always send important account management and security notices. Also, we never reveal the organization email address to other users.</FormDescription>
						</FormItem>
					)} />
				<Button type="submit" disabled={!changeEnabled} className="mt-4" >Save changes</Button>
			</form>


		</Form>



	)

}


export default EditOrganisationNotify;