import { useNavigate } from "react-router";
import { Button } from "./ui/button";

const UnAuthorizedPage = () => {
  const navigate = useNavigate();

  const goBack = () => navigate(-1);

  return (
    <section>
      <h1>You are not Authorized to view the requested page</h1>
      <div className="flex flex-col">
        <Button variant="link" onClick={goBack}>
          Go Back
        </Button>
      </div>
    </section>
  );
};

export default UnAuthorizedPage;
