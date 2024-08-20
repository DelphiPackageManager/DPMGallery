import axios from "@/api/axios";
import { useEffect, useState } from "react";
import { Link, NavLink, useLocation, useNavigate } from "react-router-dom";
import { useWindowSize } from "usehooks-ts";
import useAuth from "../hooks/useAuth";
import DarkModeToggle from "./darkModeToggle";

import { HoverCard, HoverCardContent, HoverCardTrigger } from "./ui/hover-card";

// import useAuth so we can tell if logged in
export default function NavBar() {
	const [isNavExpanded, setIsNavExpanded] = useState(false);
	const [menuOpen, setMenuOpen] = useState(false);
	const { currentUser } = useAuth();

	let { state, pathname } = useLocation();
	const { width, height } = useWindowSize();

	const buttonHandler = (event: React.MouseEvent<HTMLButtonElement>) => {
		event.preventDefault();
		setIsNavExpanded(!isNavExpanded);
	};

	//close the mobile menu when we change routes.
	useEffect(() => {
		setIsNavExpanded(false);
	}, [pathname, width, height]);

	const navStyle =
		"block py-2 pr-4 pl-3 border-b border-primary-600 md:hover:bg-inherit md:dark:hover:bg-inherit md:hover:opacity-80 md:border-0 md:p-0";

	const isLoggedIn = currentUser !== null ? true : false;

	const Profile = () => {
		const navigate = useNavigate();
		const { logout } = useAuth();
		const handlClick = async (event: React.MouseEvent<HTMLDivElement>) => {
			event.preventDefault();
			//post to logout endpoint and then redirect to login page.
			try {
				await axios.post("/ui/auth/logout");
				logout();
				navigate("/login");
			} catch (err: any) {
				//not much we can do here.
			}
		};

		if (!isLoggedIn) {
			return (
				<Link
					className="mx-1 rounded-lg py-2 text-sm font-medium hover:opacity-80 focus:ring-4"
					to="/login"
					state={state}
					title="Login In or Create an account">
					Log In
				</Link>
			);
		} else {
			return (
				<HoverCard openDelay={200} closeDelay={50000}>
					<HoverCardTrigger asChild>
						<div>
							<label tabIndex={0} className="m-1 px-1 pl-2">{`${currentUser?.userName}`}</label>
							<span className="caret"></span>
						</div>
					</HoverCardTrigger>
					<HoverCardContent className="dropdown-content z-20 w-80" align="end" >
						<ul tabIndex={0} className="w-72">
							<li className="dropdown-header">
								<div className="flex flex-row items-center gap-3 py-1">
									<div className="h-16 w-16">
										<img className="rounded-lg" src={`${currentUser?.avatarUrl}`} alt="" />
									</div>
									<div className="">
										<div>{`${currentUser?.userName}`}</div>
										<div className="text-sm text-gray-400">{`${currentUser?.email}`}</div>
									</div>
								</div>
							</li>
							<li className="divider"></li>
							<li>
								<Link to={`/profiles/${currentUser?.userName}`}>My Profile</Link>
							</li>
							<li>
								<Link to="/account/email">Account Settings</Link>
							</li>
							<li>
								<Link to="/account/organisations">Manage Organisations</Link>
							</li>
							<li>
								<Link to="/account/apikeys">API Keys</Link>
							</li>
							<li>
								<Link to="/account/packages">Manage Packages</Link>
							</li>
							<li className="divider"></li>
							<li>
								<Link to="/account/packages/upload">Upload Packages</Link>
							</li>
							<li className="divider"></li>
							<li>
								<div className="link" onClick={handlClick}>
									Log Out
								</div>
							</li>
						</ul>
					</HoverCardContent>
				</HoverCard>


				// <div className="dropdown dropdown-hover">
				// 	<label tabIndex={0} className="m-1 p-2">{`${currentUser?.userName}`}</label>
				// 	<ul tabIndex={0} className="dropdown-content w-72">
				// 		<li className="dropdown-header">
				// 			<div className="flex flex-row items-center gap-3">
				// 				<div className="w-18 h-18">
				// 					<img className="rounded-md" src={`${currentUser?.avatarUrl}`} alt="" />
				// 				</div>
				// 				<div className="">
				// 					<div>{`${currentUser?.userName}`}</div>
				// 					<div className="text-sm text-gray-400">{`${currentUser?.email}`}</div>
				// 				</div>
				// 			</div>
				// 		</li>
				// 		<li className="divider"></li>
				// 		<li>
				// 			<Link to={`/profiles/${currentUser?.userName}`}>My Profile</Link>
				// 		</li>
				// 		<li>
				// 			<Link to="/account/email">Account Settings</Link>
				// 		</li>
				// 		<li>
				// 			<Link to="/account/organisations">Manage Organisations</Link>
				// 		</li>
				// 		<li>
				// 			<Link to="/account/apikeys">API Keys</Link>
				// 		</li>
				// 		<li>
				// 			<Link to="/account/packages">Manage Packages</Link>
				// 		</li>
				// 		<li className="divider"></li>
				// 		<li>
				// 			<Link to="/account/packages/upload">Upload Packages</Link>
				// 		</li>
				// 		<li className="divider"></li>
				// 		<li>
				// 			<div className="link" onClick={handlClick}>
				// 				Log Out
				// 			</div>
				// 		</li>
				// 	</ul>
				// </div>
			);
		}
	};

	return (
		<nav className="fixed top-0 z-10 min-h-[3.5rem] w-full bg-brand px-4 py-2 text-white md:px-6">
			<div className="mx-auto flex max-w-7xl flex-wrap items-center justify-between">
				<div className="flex items-center">
					<NavLink className="mr-3 self-center whitespace-nowrap text-xl text-white" to="/">
						DPM
					</NavLink>
				</div>
				<div className="flex flex-row items-center md:order-2">
					<DarkModeToggle />
					<Profile />
					<button
						type="button"
						onClick={buttonHandler}
						className="ml-1 inline-flex items-center p-2 text-sm hover:opacity-80 focus:outline-none md:hidden"
						aria-controls="mobile-menu-2"
						aria-expanded="false">
						<span className="sr-only">Open main menu</span>
						<svg className="h-6 w-6" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
							<path
								fillRule="evenodd"
								d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
								clipRule="evenodd"></path>
						</svg>
						<svg className="hidden h-6 w-6" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
							<path
								fillRule="evenodd"
								d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
								clipRule="evenodd"></path>
						</svg>
					</button>
				</div>

				<div
					className={`${isNavExpanded ? "" : "hidden"
						} absolute md:relative top-14 md:top-[unset] left-0 md:left-[unset] justify-between items-center w-full md:flex md:w-auto md:order-1 bg-brand`}
					id="mobile-menu-2">
					<ul className="mt-4 flex flex-col font-medium md:mt-0 md:flex-row md:space-x-8">
						<li>
							<NavLink to="/" className={navStyle} aria-current="page">
								Home
							</NavLink>
						</li>
						<li>
							<NavLink to="/packages" className={navStyle} aria-current="page">
								Packages
							</NavLink>
						</li>
						<li>
							<NavLink to="/stats" className={navStyle} aria-current="page">
								Statistics
							</NavLink>
						</li>
						<li>
							<a href="https://docs.delphi.dev" target="_blank" className={navStyle}>
								Documentation
							</a>
						</li>
						<NavLink to="/downloads" className={navStyle} aria-current="page">
							Downloads
						</NavLink>
						<li>
							<a className={navStyle} href="https://www.finalbuilder.com/resources/blogs/tag/dpm" target="_blank">
								Blog
							</a>
						</li>
					</ul>
				</div>
			</div>
		</nav>
	);
}
