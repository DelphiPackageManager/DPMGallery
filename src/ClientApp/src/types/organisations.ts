import { PagedList } from "@/lib/paging";
import { SetStateAction } from "react";

export enum MemberRole {
	Collaborator = 0,
	Administrator = 1,
}

export type OrganisationMember = {
	orgId: number;
	userName: string;
	role: MemberRole;
	avatarUrl: string;
};

export type OrgName = {
	id: number;
	name: string;
}


export type UserOrganisation = {
	id: number;
	name: string;
	email: string;
	role: MemberRole;
	adminCount: number;
	collaboratorCount: number;
	packageCount: number;
	allowContact: boolean;
	notifyOnPublish: boolean;
	avatarUrl: string;
	members: Array<OrganisationMember>;
};

export type UserOrganisationCreateModel = {
	name: string;
	email: string;
}

export type UpdateOrganisationEmailModel = {
	id: number;
	email: string;
}

export type UpdateOrganisationSettingsModel = {
	id: number;
	allowContact: boolean;
	notifyOnPublish: boolean;
}

export type AddOrganisationMemberModel = {
	orgId: number;
	userName: string;
	role: MemberRole;
}


export type UserOrganisationsResult = {
	result?: PagedList<UserOrganisation> | null;
	error: string | null;
}



export type OrganisationSettings = {
	allowContact: boolean;
	notifyOnPublish: boolean;
};

export type EditableOrganisation = {
	id: number;
	name: string;
	emailAddress: string;
	settings: OrganisationSettings;
	members: OrganisationMember[] | null;
};

export const memberRoleToString = (role: MemberRole): string => {
	switch (role) {
		case MemberRole.Administrator:
			return "Administrator";
		case MemberRole.Collaborator:
			return "Collaborator";
	}
};


export type EditOrganisationProps = {
	organisation: UserOrganisation;
	updateOrganisation: React.Dispatch<React.SetStateAction<UserOrganisation | null>>
}