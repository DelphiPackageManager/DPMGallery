import { useEffect, useState } from "react";

import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import useModal from "../../hooks/useModal";
import { MemberRole, UserOrganisation } from "../../types";
import Modal from "../modal";
import PageContainer from "../pageContainer";
import EditOrganisation from "./organisations/editOrganisation";

const OrganisationsPage = () => {
  const { isOpen: addIsOpen, showModal: addShowMoal, hideModal: addHideModal } = useModal();
  const { isOpen: editIsOpen, showModal: editShowModal, hideModal: editHideModal } = useModal();
  const [organisations, setOrganisations] = useState<UserOrganisation[]>([]);
  const [errMsg, setErrorMessage] = useState("");
  const [editOrgId, setEditOrgId] = useState(-1);
  const axios = useAxiosPrivate();
  const fetchOrganisations = async () => {
    try {
      const response = await axios.get<UserOrganisation[]>("/ui/account/user-organisations");
      if (response?.data) {
        setOrganisations(response.data);
      }
    } catch (err: any) {
      if (err?.response) {
        if (err.response.statusMessage) {
          setErrorMessage(err.response.statusMessage);
        } else {
          setErrorMessage("Error fetching external login details - Error :  " + err.response.status.toString());
        }
      }
    }
  };
  useEffect(() => {
    fetchOrganisations();
  }, []);

  const handleEditClick = (event: React.MouseEvent<HTMLButtonElement>, orgId: number) => {
    event.preventDefault();
    setEditOrgId(orgId);
    editShowModal();
  };

  const memberRoleToString = (role: MemberRole): string => {
    switch (role) {
      case MemberRole.Administrator:
        return "Administrator";
      case MemberRole.Collaborator:
        return "Collaborator";
    }
  };

  return (
    <PageContainer>
      <h3 className="mb-2">Manage Organisations</h3>
      <h3 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {errMsg}
      </h3>
      {organisations.length == 0 && <p>You are not a member of any organisations, create one here</p>}
      <div className="my-2">
        <button className="btn btn-primary">Create Organisation</button>
      </div>

      {organisations.length > 0 && (
        <table className="w-full">
          <thead>
            <tr className="border-b border-gray-600">
              <th className="text-md font-semibold tracking-wide text-left">Organisation</th>
              <th className="text-md font-semibold tracking-wide text-left">Member Role</th>
              <th className="text-md font-semibold tracking-wide text-centert">Members</th>
              <th className="text-md font-semibold tracking-wide text-center">Packages</th>
              <th className="text-md font-semibold tracking-wide text-right"></th>
            </tr>
          </thead>
          <tbody>
            {organisations.map((item, index) => {
              return (
                <tr key={index} className="py-2">
                  <td className="py-1 text-left"> {item.name}</td>
                  <td className="py-1 text-left">{memberRoleToString(item.role)}</td>
                  <td className="py-1 text-center">{item.administrators} </td>
                  <td className="py-1 text-center">{item.collaborators}</td>
                  <td className="py-1 text-right">
                    {item.role === MemberRole.Administrator && <button onClick={(e) => handleEditClick(e, item.id)}>edit</button>}
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      )}
      <Modal title="Edit" isOpen={editIsOpen}>
        <EditOrganisation orgId={editOrgId} hide={editHideModal} />
      </Modal>
    </PageContainer>
  );
};

export default OrganisationsPage;
