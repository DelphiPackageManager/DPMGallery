import PageContainer from "@/components/pageContainer";
import { Button } from "@/components/ui/button";
import { Checkbox, CheckedState } from "@/components/ui/checkbox";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import useAuth from "@/hooks/useAuth";
import { UpdateEmailSchema } from "@/schemas";
import { OrganisationMember, UserOrganisation } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { ChangeEvent, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { UpdateOrganisationEmail } from "./organisationApi";
import MemberRow from "./orgMemberRow";

export type EditOrganisationProps = {
	organisation: UserOrganisation;
}

const EditOrganisation = ({ organisation }: EditOrganisationProps) => {

	const { currentUser } = useAuth();
	const [canDeleteAdmin, setCanDeleteAdmin] = useState(false);
	const [errors, setErrors] = useState<string[]>([]);
	const [changeEnabled, setChangeEnabled] = useState(false);
	const [currentEmail, setCurrentEmail] = useState(organisation.email);

	const onMemberDelete = (member: OrganisationMember) => {
		//
		if (organisation) {
			const newMembers = organisation.members.filter((item) => item.id !== member.id);
			var newOrg = { ...organisation, members: newMembers };
			organisation = newOrg;
			//setCurrentOrg(newOrg);
		}
	};

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
			//navigate(location.pathname, { replace: true, state: null });
		}
		else {
			setErrors(result.errors);
		}
	}

	if (errors.length > 0) {
		return <>
			{errors.map(function (error: string, index: number) {
				return (
					<div key={index}>
						{error}
					</div>
				)
			})}
		</>
	}


	return (
		<div>

			<div>
				<Tabs defaultValue="email" className="flex flex-col">
					<TabsList className="w-full items-start border-b border-primary bg-white text-base dark:bg-gray-800">
						<TabsTrigger value="email">Email</TabsTrigger>
						<TabsTrigger value="notify">Notifications</TabsTrigger>
						<TabsTrigger value="members">Members</TabsTrigger>
					</TabsList>
					<TabsContent value="email" className="text-sm">
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
								<Button type="submit" disabled={!changeEnabled} >Submit</Button>
							</form>
						</Form>


					</TabsContent>
					<TabsContent value="notify">
						<div className="mt-2 flex items-center px-2">
							<Checkbox name="allowContact" id="allowContact" defaultChecked={organisation?.allowContact}></Checkbox>
							<label htmlFor="allowContact" className="ml-2 cursor-pointer">
								Users can contact the organisation through the DPM Gallery
							</label>
						</div>
						<p className="ml-8">This setting allows registered users of this site to contact your organisation via the Contact Owners form.</p>
						<div className="mt-2 flex items-center px-2">
							<Checkbox name="notifyOnPublish" id="notifyOnPublish" defaultChecked={organisation?.notifyOnPublish}></Checkbox>
							<label htmlFor="notifyOnPublish" className="ml-2 cursor-pointer">
								Notify when a package is published to NuGet Gallery
							</label>
						</div>
						<p className="ml-8 whitespace-break-spaces break-words">
							This setting enables notifications whenever a package is published to your account. (recommended)
						</p>
					</TabsContent>
					<TabsContent value="members">
						<div className="flex flex-row items-end gap-4">
							<div className="">
								<label htmlFor="newMember">Enter username of new member</label>
								<input type="text" size={50} id="newMember" placeholder="Add existing delphi.dev user" />
							</div>
							<div className="flex flex-col items-start">
								<label htmlFor="role">Select Role</label>
								<Select name="role" defaultValue="Collaborator">
									<SelectTrigger className="w-[180px]">
										<SelectValue placeholder="Select role" />
									</SelectTrigger>
									<SelectContent>
										<SelectItem key={"collab"} value="Collaborator">
											Collaborator
										</SelectItem>
										<SelectItem key={"admin"} value="Administrator">
											Administrator
										</SelectItem>
									</SelectContent>
								</Select>
							</div>
							<Button variant="create" type="button">
								Add
							</Button>
						</div>
						<p className="mt-2 whitespace-break-spaces">
							A collaborator can manage the organization's packages but cannot manage the organization's memberships.
						</p>
						<div className="mt-2 overflow-y-auto">
							<table className="w-full">
								<thead className="sticky top-0 z-10">
									<tr className="border-b border-gray-600">
										<th className="text-md text-left font-semibold tracking-wide">Member</th>
										<th className="text-md text-left font-semibold tracking-wide">Role</th>
										<th className="text-md text-left font-semibold tracking-wide">Remove Member</th>
									</tr>
								</thead>
								<tbody>
									{organisation.members?.map((member: OrganisationMember) => {
										return (
											<MemberRow
												key={member.userName}
												member={member}
												currentUserName={currentUser ? currentUser.userName : ""}
												canDeleteAdmin={canDeleteAdmin}
												onDelete={onMemberDelete}
											/>
										);
									})}
								</tbody>
							</table>
						</div>
					</TabsContent>
				</Tabs>
			</div>

		</div >
	)

};

export default EditOrganisation;
