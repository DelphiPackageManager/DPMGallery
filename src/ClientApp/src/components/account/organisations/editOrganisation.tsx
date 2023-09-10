import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import Modal from "@/components/ui/modal";
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Spinner } from "@/components/ui/spinner";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import useAuth from "@/hooks/useAuth";
import { MemberRole, OrganisationMember, UserOrganisation, memberRoleToString } from "@/types";
import { FormEvent, useEffect, useState } from "react";

export type EditOrganisationProps = {
  organisation: UserOrganisation;
  afterSave(): void;
};

type MemberRowProps = {
  member: OrganisationMember;
  currentUserName?: string;
  canDelete: boolean;
  onDelete: (member: OrganisationMember) => void;
};

const MemberRow = ({ member, currentUserName, canDelete, onDelete }: MemberRowProps) => {
  const thatsYou = member.userName === currentUserName ? " (thats you)" : "";

  const onDeleteClick = (member: OrganisationMember) => {
    onDelete(member);
  };

  const canDeleteMember = member.role !== MemberRole.Administrator || canDelete;

  return (
    <tr key={member.id} className="py-2">
      <td className="py-1 text-left">
        <div className="flex items-center">
          <img src={member.avatarUrl} className="w-10 h-10 mr-2" />
          {member.userName}
          {thatsYou}
        </div>
      </td>
      <td className="py-1 text-left">{memberRoleToString(member.role)}</td>
      <td>
        {canDeleteMember && (
          <Button type="button" variant={"destructive"} className="w-8 h-8 p-0" onClick={() => onDeleteClick(member)}>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5">
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </Button>
        )}
      </td>
    </tr>
  );
};

const EditOrganisation = ({ organisation, afterSave }: EditOrganisationProps) => {
  const [saving, setSaving] = useState(false);
  const { currentUser } = useAuth();
  const [currentOrg, setCurrentOrg] = useState(organisation);

  const [canDeleteAdmin, setCanDeleteAdmin] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);

    //let data = Object.fromEntries(new FormData(event.currentTarget));
    console.log("calling afterSave");
    //await updateContact(contact.id, data);
    afterSave();
  }

  useEffect(() => {
    var admins = currentOrg.members.filter(function (element) {
      return element.role === MemberRole.Administrator;
    }).length;
    setCanDeleteAdmin(admins > 1);
  }, [organisation]);

  const onMemberDelete = (member: OrganisationMember) => {
    //
    const newMembers = currentOrg.members.filter((item) => item !== member);

    setCurrentOrg((prev) => ({ ...prev, members: newMembers }));
  };

  return (
    <form onSubmit={handleSubmit} className="flex flex-col min-w-full">
      <h2 className="mt-2">{currentOrg.name}</h2>
      <fieldset disabled={saving} className="flex-grow group flex flex-col w-full justify-between">
        <div className="w-full mt-4 min-w-full">
          <Tabs defaultValue="email" className="flex flex-col content-stretch w-full">
            <TabsList className="w-full border-b border-primary text-base bg-white dark:bg-gray-800 items-start">
              <TabsTrigger value="email">Email</TabsTrigger>
              <TabsTrigger value="notify">Notifications</TabsTrigger>
              <TabsTrigger value="members">Members</TabsTrigger>
            </TabsList>
            <TabsContent value="email" className="">
              <p>Email : {currentOrg.email}</p>
              <div className="flex flex-col mt-2">
                <label htmlFor="newEmail">New Email</label>
                <input id="newEmail" name="newEmail" size={60} type="email" className="w-96"></input>
              </div>
            </TabsContent>
            <TabsContent value="notify">
              <div className="flex mt-2 px-2 items-center">
                <Checkbox name="allowContact" id="allowContact" defaultChecked={currentOrg.allowContact}></Checkbox>
                <label htmlFor="allowContact" className="cursor-pointer ml-2">
                  Users can contact the organisation through the DPM Gallery
                </label>
              </div>
              <p className="ml-8">This setting allows registered users of this site to contact your organisation via the Contact Owners form.</p>
              <div className="flex mt-2 px-2 items-center">
                <Checkbox name="notifyOnPublish" id="notifyOnPublish" defaultChecked={currentOrg.notifyOnPublish}></Checkbox>
                <label htmlFor="notifyOnPublish" className="cursor-pointer ml-2">
                  Notify when a package is published to NuGet Gallery
                </label>
              </div>
              <p className="ml-8 break-words whitespace-break-spaces">
                This setting enables notifications whenever a package is published to your account. (recommended)
              </p>
            </TabsContent>
            <TabsContent value="members">
              <div className="flex flex-row gap-4 items-end">
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
                      <SelectItem value="Collaborator">Collaborator</SelectItem>
                      <SelectItem value="Administrator">Administrator</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                <Button variant="create" type="button">
                  Add
                </Button>
              </div>
              <p className="mt-2">A collaborator can manage the organization's packages but cannot manage the organization's memberships.</p>
              <div className="mt-2 overflow-y-auto">
                <table className="w-full">
                  <thead className="sticky top-0 z-10">
                    <tr className="border-b border-gray-600">
                      <th className="text-md font-semibold tracking-wide text-left">Member</th>
                      <th className="text-md font-semibold tracking-wide text-left">Role</th>
                      <th className="text-md font-semibold tracking-wide text-left">Delete Member</th>
                    </tr>
                  </thead>
                  <tbody>
                    {currentOrg.members.map((member) => {
                      return (
                        <MemberRow
                          key={member.id}
                          member={member}
                          currentUserName={currentUser ? currentUser.userName : ""}
                          canDelete={canDeleteAdmin}
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
        <div className="w-full mt-8 space-x-6 text-right">
          <Modal.Close className="rounded px-4 py-2 text-sm font-medium text-gray-500 hover:text-gray-600">Cancel</Modal.Close>
          <button
            className="inline-flex items-center justify-center rounded bg-green-500 px-4 py-2 text-sm font-medium text-white hover:bg-green-600 group-disabled:pointer-events-none"
            type="submit">
            <Spinner className="absolute h-4 group-enabled:opacity-0" />
            <span className="group-disabled:opacity-0">Save</span>
          </button>
        </div>
      </fieldset>
    </form>
  );
};

export default EditOrganisation;
