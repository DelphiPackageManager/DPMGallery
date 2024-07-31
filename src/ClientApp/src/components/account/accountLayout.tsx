import { NavLink, Outlet } from "react-router-dom";
import PageContainer from "../pageContainer";

const AccountLayout = () => {
	const activeClassName = "active";
	return (
		<PageContainer className="grow">
			<hr className="mt-1" />
			<div className="mt-1 h-full">
				<div className="flex h-full flex-row flex-wrap gap-2 pb-2 md:flex-nowrap">
					<div className="sidebar">
						<ul className="w-full list-none">
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
								<NavLink to="/account/organisations">Manage Organisations</NavLink>
							</li>
						</ul>
					</div>
					<div className="grow items-start">
						<Outlet />
					</div>
				</div>
			</div>
		</PageContainer>
	);
};

export default AccountLayout;
