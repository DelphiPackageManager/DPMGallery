import { useLocation, useSearchParams } from "react-router-dom";
import PageContainer from "../../pageContainer";

const ShowRecoveryCodesPage = () => {
  const location = useLocation();
  const recoverycodes = location?.state?.codes;
  const message = location?.state?.message;

  const codes = recoverycodes.split(",");
  const col1: string[] = [];
  const col2: string[] = [];

  codes.forEach((item: any, i: number) => {
    if (i % 2 === 0) {
      col1.push(item);
    } else {
      col2.push(item);
    }
  });

  return (
    <PageContainer>
      <h3>Recovery Codes</h3>
      <div className="alert alert-@statusMessageClass " role="alert">
        <span>{message}</span>
      </div>
      <div className="alert alert-warning" role="alert">
        <p>
          <strong>Put these codes in a safe place.</strong>
        </p>
        <p>If you lose your device and don't have the recovery codes you will lose access to your account.</p>
      </div>
      <div className="flex flex-row mt-2 gap-0 bg-yellow-200 text-yellow-700 font-mono">
        <div className="flex-grow p-2 items-start justify-center">
          {col1.map((code, index) => (
            <div>{code}</div>
          ))}
        </div>
        <div className="flex-grow p-2 items-start justify-center">
          {col2.map((code, index) => (
            <p>{code}</p>
          ))}
        </div>
      </div>
    </PageContainer>
  );
};

export default ShowRecoveryCodesPage;
