import PageContainer from "../../pageContainer";
import PageHeader from "../../pageHeader";
import OrganisationsTable from "./organisationsTable";

const OrganisationsPage = () => {

	return (
		<PageContainer>
			<PageHeader title="Manage Organisations" />
			<OrganisationsTable />
		</PageContainer>
	);
};

export default OrganisationsPage;
