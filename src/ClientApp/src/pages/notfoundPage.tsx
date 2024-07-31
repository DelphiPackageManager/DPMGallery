import { SITE_URL } from "@/types/constants";
import Meta from "../components/meta";
import PageContainer from "../components/pageContainer";

const NotFoundPage = () => {
	return (
		<PageContainer>
			<Meta title="DPM - Page Not Found" canonical={`${SITE_URL}/404`} />

			<h1>Page not found</h1>
		</PageContainer>
	);
};

export default NotFoundPage;
