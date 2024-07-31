import * as React from "react";
import { NavLink } from "react-router-dom";

const Footer = () => {
	return (
		<footer className="bg-brand p-4 text-base text-white sm:p-6">
			<div className="mx-auto max-w-6xl">
				<div className="md:flex md:justify-between">
					<div className="mb-6 md:mb-0">
						<a href="https://delphi.dev" className="flex items-center">
							{/* <img src="https://flowbite.com/docs/images/logo.svg" className="mr-3 h-8" alt="DPM Logo" /> */}
							<span className="self-center whitespace-nowrap text-xl">DPM</span>
						</a>
					</div>
					<div className="grid grid-cols-3 gap-2 sm:grid-cols-3 md:gap-4">
						<div>
							<h2 className="mb-1 text-sm font-semibold uppercase md:mb-2 md:text-sm">Support</h2>
							<ul className="text-sm text-white text-opacity-70">
								<li className="mb-3">
									<a href="https://github.com/DelphiPackageManager/DPM/issues" target="_blank" className="hover:text-white hover:text-opacity-100">
										Issues
									</a>
								</li>
								<li>
									<a
										href="https://github.com/DelphiPackageManager/DPM/discussions"
										target="_blank"
										className="hover:text-white hover:text-opacity-100">
										Discussions
									</a>
								</li>
							</ul>
						</div>
						<div>
							<h2 className="mb-1 text-xs font-semibold uppercase md:mb-2 md:text-sm">Resources</h2>
							<ul className="text-sm text-white text-opacity-70">
								<li className="mb-3">
									<a href="https://docs.delphi.dev/dpm" target="_blank" className="hover:text-white hover:text-opacity-100">
										Documentation
									</a>
								</li>
								<li>
									<a href="https://www.finalbuilder.com/resources/blogs" target="_blank" className="hover:text-white hover:text-opacity-100">
										Blogs
									</a>
								</li>
								<li></li>
							</ul>
						</div>
						<div>
							<h2 className="mb-1 text-xs font-semibold uppercase md:mb-2 md:text-sm">Legal</h2>
							<ul className="text-sm text-white text-opacity-70">
								<li className="mb-3">
									<NavLink to="/policies/privacy" className="hover:text-white hover:text-opacity-100">
										Privacy Policy
									</NavLink>
								</li>
								<li className="mb-3">
									<NavLink to="/policies/terms" className="hover:text-white hover:text-opacity-100">
										Terms &amp; Conditions
									</NavLink>
								</li>
								<li>
									<NavLink to="/policies/package" className="hover:text-white hover:text-opacity-100">
										Package Policies
									</NavLink>
								</li>
							</ul>
						</div>
					</div>
				</div>
				<hr className="my-3 border-gray-300 sm:mx-auto lg:my-4" />
				<div className="sm:flex sm:items-center sm:justify-between">
					<span className="text-xs text-white text-opacity-60 sm:text-center">
						Copyright © {new Date().getFullYear()} Vincent Parrett & Contributors. All Rights Reserved.
					</span>
					<div className="mt-4 flex space-x-6 sm:mt-0 sm:justify-center">
						<a href="https://github.com/DelphiPackageManager" target="_blank" className="text-white text-opacity-60 hover:text-opacity-100">
							<svg className="h-5 w-5" fill="currentColor">
								<use href="#github" />
							</svg>
						</a>
					</div>
				</div>
			</div>
		</footer>
	);
};

export default Footer;
