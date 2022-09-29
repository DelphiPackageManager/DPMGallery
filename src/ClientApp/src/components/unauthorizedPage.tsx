import { useNavigate } from "react-router-dom"
import PageContainer from "./pageContainer";

const Unauthorized = () => {
    const navigate = useNavigate();

    const goBack = () => navigate(-1);

    return (
        <PageContainer className="text-center">
            <h1>Unauthorized</h1>
            <br />
            <p>You do not have access to the requested page.</p>
            <div className="flexGrow">
                <button onClick={goBack}>Go Back</button>
            </div>
        </PageContainer>
    )
}

export default Unauthorized