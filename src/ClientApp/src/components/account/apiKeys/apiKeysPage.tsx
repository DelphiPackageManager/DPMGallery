import PageHeader from "@/components/pageHeader";
import PageContainer from "../../../components/pageContainer";
import ApiKeysTable from "./apiKeysTable";

const APIKeysPage = () => {
	return (
		<PageContainer>
			<PageHeader title="API Keys" >
				<p>An API key is a token used to authenticate when pushing packages to the server.</p>
				<p>
					Always keep your API keys a secret! If one of your keys is accidentally revealed, you can always generate a new one at any time. You can also
					remove existing API keys if necessary.
				</p>
			</PageHeader>
			<div className="flex w-full flex-col">
				<ApiKeysTable />
			</div>
		</PageContainer>
	);
};

export default APIKeysPage;
