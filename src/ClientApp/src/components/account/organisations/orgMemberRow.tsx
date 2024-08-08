import { Button } from "@/components/ui/button";
import { MemberRole, memberRoleToString, OrganisationMember } from "@/types/organisations";
import { X } from "lucide-react";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";


type MemberRowProps = {
	member: OrganisationMember;
	currentUserName?: string;
	onDelete: (member: OrganisationMember) => void;
	adminCount: number;
};

const MemberRow = ({ member, currentUserName, adminCount, onDelete }: MemberRowProps) => {
	const thatsYou = member.userName === currentUserName ? " (thats you)" : "";
	const [canDeleteMember, setCanDeleteMember] = useState(false)

	const onDeleteClick = (member: OrganisationMember) => {
		onDelete(member);
	};

	useEffect(() => {
		setCanDeleteMember(adminCount > 1 || member.role === MemberRole.Collaborator);
	}, [adminCount])


	return (
		<tr key={member.userName} className="my-1 py-3">
			<td className="py-1 text-left">
				<div className="flex items-center">
					<img src={member.avatarUrl} className="mr-2 h-10 w-10 rounded-md" />
					<Link to={`/profiles/${member.userName}`} className="mr-2 text-sky-600 hover:underline dark:text-sky-600">{member.userName}</Link>{thatsYou}
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