import { Button } from "@/components/ui/button";
import { MemberRole, memberRoleToString, OrganisationMember } from "@/types/organisations";
import { X } from "lucide-react";


type MemberRowProps = {
	member: OrganisationMember;
	currentUserName?: string;
	onDelete: (member: OrganisationMember) => void;
	adminCount: number;
};

const MemberRow = ({ member, currentUserName, adminCount, onDelete }: MemberRowProps) => {
	const thatsYou = member.userName === currentUserName ? " (thats you)" : "";

	const canDeleteMember: boolean = adminCount > 1;// || member.role === MemberRole.Collaborator;

	const onDeleteClick = (member: OrganisationMember) => {
		alert(member.userName);
		//onDelete(member);
	};



	return (
		<tr key={member.id} className="my-1 py-3">
			<td className="py-1 text-left">
				<div className="flex items-center">
					<img src={member.avatarUrl} className="mr-2 h-10 w-10 rounded-md" />
					{member.userName}
					{thatsYou}
				</div>
			</td>
			<td className="py-1 text-left">{memberRoleToString(member.role)}</td>
			<td className="text-left">
				{canDeleteMember && (
					<Button type="button" variant={"destructive"} className="h-8 w-8 p-0" onClick={() => onDeleteClick(member)}>
						<X size={16} strokeWidth={1.5} />
					</Button>
				)}
			</td>
		</tr>
	);
};

export default MemberRow;