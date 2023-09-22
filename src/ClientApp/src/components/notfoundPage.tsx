import { SITE_URL } from "@/constants";
import Meta from "./meta";
import PageContainer from "./pageContainer";

const NotFoundPage = () => {
  return (
    <PageContainer>
      <Meta title="DPM - Page Not Found" canonical={`${SITE_URL}/404`} />

      <h1>Page not found</h1>
    </PageContainer>
  );
};

export default NotFoundPage;
