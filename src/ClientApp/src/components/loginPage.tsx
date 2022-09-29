import * as React from 'react';
import PageContainer from './pageContainer';

export interface ILoginPageProps {
}

const  LoginPage = (props: ILoginPageProps) => {
  return (
    <PageContainer className='text-center'>
        <h1>Login</h1>
    </PageContainer>
  );
}

export default LoginPage;