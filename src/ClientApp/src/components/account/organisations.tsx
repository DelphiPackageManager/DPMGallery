import { useEffect, useState } from "react";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import { MemberRole, UserOrganisation, memberRoleToString } from "../../types";
import PageContainer from "../pageContainer";
import { Button } from "../ui/button";
import Modal from "../ui/modal";
import EditOrganisation from "./organisations/editOrganisation";
import NewOrganisation from "./organisations/newOrganisation";

function OrganisationRow({ item }: { item: UserOrganisation }) {
  const [open, setOpen] = useState(false);

  return (
    <tr key={item.id} className="py-2">
      <td className="py-1 text-left"> {item.name}</td>
      <td className="py-1 text-left">{memberRoleToString(item.role)}</td>
      <td className="py-1 text-center">{item.adminCount} </td>
      <td className="py-1 text-center">{item.collaboratorCount}</td>
      <td className="py-1 text-center">{item.packageCount}</td>
      <td className="py-1 text-right">
        {item.role === MemberRole.Administrator && (
          <Modal open={open} onOpenChange={setOpen}>
            <Modal.Button>
              <span>edit</span>
            </Modal.Button>
            <Modal.Content title="Edit Organisation">
              <EditOrganisation key={item.id} organisation={item} afterSave={() => setOpen(false)} />
            </Modal.Content>
          </Modal>
        )}
      </td>
    </tr>
  );
}

const OrganisationsPage = () => {
  const [organisations, setOrganisations] = useState<UserOrganisation[]>([]);
  const [errMsg, setErrorMessage] = useState("");
  const [open, setOpen] = useState(false);
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

  const afterSaveNewOrg = () => {
    setOpen(false);
    fetchOrganisations();
  };

  return (
    <PageContainer>
      <h3 className="mb-2">Manage Organisations</h3>
      <h3 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {errMsg}
      </h3>
      {organisations.length == 0 && <p>You are not a member of any organisations, create one here</p>}
      <div className="my-2">
        <Modal open={open} onOpenChange={setOpen}>
          <Modal.Button asChild>
            <Button>Create Organisation</Button>
          </Modal.Button>
          <Modal.Content title="Create Organisation">
            <NewOrganisation afterSave={() => afterSaveNewOrg} />
          </Modal.Content>
        </Modal>
      </div>

      {organisations.length > 0 && (
        <table className="w-full">
          <thead>
            <tr className="border-b border-gray-600">
              <th className="text-md font-semibold tracking-wide text-left">Organisation</th>
              <th className="text-md font-semibold tracking-wide text-left">Member Role</th>
              <th className="text-md font-semibold tracking-wide text-centert">Administrators</th>
              <th className="text-md font-semibold tracking-wide text-centert">Collaborators</th>
              <th className="text-md font-semibold tracking-wide text-center">Packages</th>
              <th className="text-md font-semibold tracking-wide text-right"></th>
            </tr>
          </thead>
          <tbody>
            {organisations.map((item) => {
              return <OrganisationRow key={item.id} item={item} />;
            })}
          </tbody>
        </table>
      )}
    </PageContainer>
  );
};

export default OrganisationsPage;
