import { NavLink, Outlet } from "react-router-dom";
import PageContainer from "../pageContainer";

const AccountLayout = () => {
  const activeClassName = "active";
  return (
    <PageContainer className="pt-2">
      <h1 className="">Manage your account</h1>

      <hr />
      <div className="flex flex-row flex-wrap md:flex-nowrap gap-2 mt-2 ">
        <div className="sidebar">
          <ul className="list-none w-full">
            <li>
              <NavLink to="/account/email">Email</NavLink>
            </li>
            <li>
              <NavLink to="/account/changepassword">Password</NavLink>
            </li>
            <li>
              <NavLink to="/account/externallogins">External Logins</NavLink>
            </li>
            <li>
              <NavLink to="/account/twofactorauth">Two-factor authentication</NavLink>
            </li>
            <li>
              <NavLink to="/account/apikeys">API Keys</NavLink>
            </li>
            <li>
              <NavLink to="/account/packages">My Packages</NavLink>
            </li>
            <li>
              <NavLink to="/account/organisations">Manage Organisation</NavLink>
            </li>
          </ul>
        </div>
        <div className="items-start flex-grow">
          <Outlet />
        </div>
      </div>
    </PageContainer>
  );
};

export default AccountLayout;
