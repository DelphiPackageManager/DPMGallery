import { useSearchParams } from "react-router-dom";
import PageContainer from "../pageContainer";

const ShowRecoveryCodesPage = () => {
  let [searchParams] = useSearchParams();
  const recoverycodes = searchParams.get("recoverycodes") || "";
  const codes = recoverycodes.split(",");
  const col1: string[] = [];
  const col2: string[] = [];

  codes.forEach((item, i) => {
    if (i % 2 === 0) {
      col1.push(item);
    } else {
      col2.push(item);
    }
  });

  return (
    <PageContainer>
      <h1>Recovery Codes</h1>
      <div className="alert alert-warning" role="alert">
        <p>
          <strong>Put these codes in a safe place.</strong>
        </p>
        <p>If you lose your device and don't have the recovery codes you will lose access to your account.</p>
      </div>
      <div className="flex flex-row mt-2 gap-0 bg-yellow-200 text-yellow-700">
        <div className="flex-grow p-2 items-start justify-center">
          {col1.map((code) => (
            <li>{code}</li>
          ))}
        </div>
        <div className="flex-grow p-2 items-start justify-center">
          {col2.map((code) => (
            <li>{code}</li>
          ))}
        </div>
      </div>
    </PageContainer>
  );
};

export default ShowRecoveryCodesPage;
