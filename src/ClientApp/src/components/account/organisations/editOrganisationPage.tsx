import PageContainer from "@/components/pageContainer";
import PageHeader from "@/components/pageHeader";
import { useParams } from "react-router-dom";

const EditOrganisationPage = () => {
	const { orgName } = useParams();


	return (
		<PageContainer>
			<PageHeader title="Edit Organisation " />
			<div> {orgName} </div>

		</PageContainer>
	)

}

export default EditOrganisationPage;