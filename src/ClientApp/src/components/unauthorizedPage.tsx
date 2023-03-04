import { useNavigate } from "react-router";

const UnAuthorizedPage = () => {
  const navigate = useNavigate();

  const goBack = () => navigate(-1);

  return (
    <section>
      <h1>You are not Authorized to view the requested page</h1>
      <div className="flex flex-col">
        <button onClick={goBack}>Go Back</button>
      </div>
    </section>
  );
};

export default UnAuthorizedPage;
