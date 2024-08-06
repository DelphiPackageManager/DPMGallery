import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import useAuth from "@/hooks/useAuth";
import { MemberRole, OrganisationMember, UserOrganisation } from "@/types/organisations";
import { useEffect, useState } from "react";
import MemberRow from "./orgMemberRow";

type EditOrganisationMembersProps = {
	organisation: UserOrganisation;
	setErrors: (value: string[]) => void;
}

const EditOrganisationMembers = ({ organisation, setErrors }: EditOrganisationMembersProps) => {

	function calcAdminCount(members: Array<OrganisationMember>): number {
		const result = members.filter((member) => member.role == MemberRole.Administrator);
		return result.length;
	}

	const { currentUser } = useAuth();



	const [adminCount, setAdminCount] = useState(0);

	useEffect(() => {
		const initalAdminCount = calcAdminCount(organisation.members);
		setAdminCount(initalAdminCount);
	}, [])

	const onMemberDelete = (member: OrganisationMember) => {
		//
		if (organisation) {
			const newMembers = organisation.members.filter((item) => item.id !== member.id);
			var newOrg = { ...organisation, members: newMembers };
			organisation = newOrg;
			setAdminCount(calcAdminCount(organisation.members));
		}
	};



	return (<>
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
								adminCount={adminCount}
								onDelete={onMemberDelete}
							/>
						);
					})}
				</tbody>
			</table>

		</div>

	</>)

}

export default EditOrganisationMembers;