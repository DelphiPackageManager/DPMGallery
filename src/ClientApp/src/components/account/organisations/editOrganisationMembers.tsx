import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogDestructiveAction, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle } from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import useAuth from "@/hooks/useAuth";
import { checkUserExistsDebounced } from "@/schemas";
import { MemberRole, OrganisationMember, UserOrganisation } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { FormEvent, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useLocation, useNavigate } from "react-router-dom";
import { z } from "zod";
import MemberRow from "./orgMemberRow";
import { AddOrganisationMember, DeleteOrganisationMember } from "./organisationApi";

type EditOrganisationMembersProps = {
	organisation: UserOrganisation;
	setErrors: (value: string[]) => void;
	updateOrganisation: React.Dispatch<React.SetStateAction<UserOrganisation | null>>;

}

const EditOrganisationMembers = ({ organisation, setErrors, updateOrganisation }: EditOrganisationMembersProps) => {

	const { currentUser } = useAuth();
	const navigate = useNavigate();
	const [adminCount, setAdminCount] = useState(0);
	const [confirmOpen, setConfirmOpen] = useState(false);
	const [alertExtraMessage, setAlertExtraMessage] = useState("");
	const [userNameToBeDeleted, setUserNameToBeDeleted] = useState("");
	const [memberToBeDeleted, setMemberToBeDeleted] = useState<OrganisationMember | null>(null);

	function calcAdminCount(members: Array<OrganisationMember>): number {
		const result = members.filter((member) => member.role == MemberRole.Administrator);
		return result.length;
	}

	useEffect(() => {
		const initalAdminCount = calcAdminCount(organisation.members);
		setAdminCount(initalAdminCount);
	}, [])

	const onMemberDelete = (member: OrganisationMember) => {
		setMemberToBeDeleted(member);
		if (member.userName == currentUser?.userName) {
			setUserNameToBeDeleted("YOURSELF!!")
			setAlertExtraMessage("YOU WILL NO LONGER BE ABLE TO ACCESS THIS ORGANISATION!");
		}
		else {
			setUserNameToBeDeleted(member.userName)
			setAlertExtraMessage("");
		}
		setConfirmOpen(true);
	};

	const onAlertSubmit = async () => {
		//		setConfirmOpen(false);
		if (organisation && memberToBeDeleted) {
			const deletingCurrentUser = memberToBeDeleted.userName == currentUser?.userName;
			//call delete api here!
			var result = await DeleteOrganisationMember(organisation.id, memberToBeDeleted.userName);
			if (result.succeeded) {
				const newMembers = organisation.members.filter((item) => item.userName != memberToBeDeleted.userName);
				var newOrg = { ...organisation, members: newMembers };
				updateOrganisation(newOrg);
				//note that organisation is not updated yet until re-render so must use newOrg.
				const initalAdminCount = calcAdminCount(newOrg.members);
				setAdminCount(initalAdminCount);
				if (deletingCurrentUser) {
					//redirect back to organisatons
					navigate("/account/organisations", { replace: true });
				}
			}

			else {
				setErrors(result.errors);
			}
		}
	}

	const AddOrganisationMemberSchema = z.object({
		name: z.string().trim()
			.refine(value => {
				let existing = organisation.members?.find((x) => x.userName == value);
				if (existing)
					return false;
				return true;
			}, "user is already a member")
			.refine(async (value: string) => {
				if (!value)
					return true;

				let result = await checkUserExistsDebounced(value);
				if (result.succeeded) {
					return result.data
				} else
					return false;
			}, "user does not exist"),
		memberRole: z.nativeEnum(MemberRole)
	})


	const addMemberForm = useForm<z.infer<typeof AddOrganisationMemberSchema>>({
		resolver: zodResolver(AddOrganisationMemberSchema),
		defaultValues: {
			name: "",
			memberRole: MemberRole.Collaborator
		},
		mode: "onChange"
	});

	async function onSubmitAddMember(values: z.infer<typeof AddOrganisationMemberSchema>) {
		let model = {
			orgId: organisation.id,
			userName: values.name,
			role: values.memberRole
		};
		addMemberForm.reset();

		var result = await AddOrganisationMember(model);
		if (result.succeeded) {
			var member = result.data.data;
			const newMembers = [...organisation.members, member];
			var newOrg = { ...organisation, members: newMembers };
			updateOrganisation(newOrg)
			const initalAdminCount = calcAdminCount(newOrg.members);
			setAdminCount(initalAdminCount);
		}
		else {
			setErrors(result.errors);
		}

	}

	const { isDirty, isValid } = addMemberForm.formState;

	return (
		<div className="flex flex-col">
			<div className="flex flex-row items-end gap-4">
				<Form {...addMemberForm}>
					<form onSubmit={addMemberForm.handleSubmit(onSubmitAddMember)} className="flex items-end gap-4">
						<FormField
							control={addMemberForm.control}
							name="name"
							render={({ field }) => (
								<FormItem className="">
									<div className="flex gap-2">
										<div className="flex flex-col">
											<div className="flex items-baseline justify-between">
												<FormLabel controlType="input" htmlFor="names" className="ml-2">Enter username to add as member</FormLabel>
												<FormMessage className="flex-grow text-right" />
											</div>
											<FormControl>
												<Input size={55} {...field} id="name" required />
											</FormControl>
										</div>
									</div>
								</FormItem>
							)} />
						<FormField
							control={addMemberForm.control}
							name="memberRole"
							render={({ field }) => (
								<FormItem>
									<FormLabel>Add member role</FormLabel>
									<FormControl>
										<Select name="expires" onValueChange={(value) => field.onChange(parseInt(value))} defaultValue={field.value.toString()} >
											<SelectTrigger className="w-[180px]">
												<SelectValue placeholder="Select role" />
											</SelectTrigger>
											<SelectContent>
												<SelectItem value="0">Collaborator</SelectItem>
												<SelectItem value="1">Administrator</SelectItem>
											</SelectContent>
										</Select>
									</FormControl>

								</FormItem>

							)}
						/>
						<Button type="submit" disabled={!isDirty || !isValid} variant="create" >Add</Button>

					</form>

				</Form>
			</div >
			<p className="text-gray-600">A collaborator can manage the organization's packages but cannot manage the organization's memberships. Learn more.</p>
			<div className="mt-2 overflow-y-auto">
				<table className="w-full">
					<thead className="sticky top-0">
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
									adminCount={adminCount}
									onDelete={onMemberDelete}
								/>
							);
						})}
					</tbody>
				</table>
			</div>
			<AlertDialog open={confirmOpen} onOpenChange={(value) => setConfirmOpen(value)} >
				<AlertDialogContent>
					<AlertDialogHeader>
						<AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
						<AlertDialogDescription asChild >
							<div className="text-sm text-muted-foreground">
								<div>
									<span>Are you sure you want to remove </span><span className="font-medium text-destructive">{userNameToBeDeleted} </span>
									<span>from Organisation </span><span className="font-medium">{organisation.name}</span>
								</div>
								<br />
								<div className="font-medium text-destructive">{alertExtraMessage}</div>
							</div>
						</AlertDialogDescription>
					</AlertDialogHeader>
					<AlertDialogFooter>
						<AlertDialogCancel>Cancel</AlertDialogCancel>
						<AlertDialogDestructiveAction type="submit" onClick={onAlertSubmit} >Continue</AlertDialogDestructiveAction>
					</AlertDialogFooter>
				</AlertDialogContent>
			</AlertDialog>

		</div >
	)

}

export default EditOrganisationMembers;