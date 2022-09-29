import * as React from 'react';
import PageContainer from './pageContainer';

export interface IRegisterPageProps {
}

const  RegisterPage = (props: IRegisterPageProps) => {
  return (
   <PageContainer className="text-center">
        <h1>Register</h1>
   </PageContainer>
  );
}

export default RegisterPage;