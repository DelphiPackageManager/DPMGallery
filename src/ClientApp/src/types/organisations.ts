import { PagedList } from "@/lib/paging";

export enum MemberRole {
	Collaborator = 0,
	Administrator = 1,
}

export type OrganisationMember = {
	id: number;
	userName: string;
	role: MemberRole;
	avatarUrl: string;
};

export type UserOrganisation = {
	id: number;
	name: string;
	email: string;
	memberId: number;
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
