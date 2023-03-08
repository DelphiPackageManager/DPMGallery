import PageContainer from "./pageContainer";
const LockedOutPage = () => {
  return (
    <PageContainer>
      <h1 className="text-danger">Locked out</h1>
      <p className="text-danger">This account has been locked out, please try again later.</p>
    </PageContainer>
  );
};

export default LockedOutPage;
