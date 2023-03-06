import { NavLink, Outlet } from "react-router-dom";
import PageContainer from "../pageContainer";

const AccountLayout = () => {
  return (
    <PageContainer className="pt-2">
      <div className="flex flex-row flex-wrap md:flex-nowrap gap-2 ">
        <div className="sidebar">
          <ul className="list-none w-full">
            <li>
              <NavLink to="/account/settings">Settings</NavLink>
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
        <div className="items-start">
          <Outlet />
        </div>
      </div>
    </PageContainer>
  );
};

export default AccountLayout;
