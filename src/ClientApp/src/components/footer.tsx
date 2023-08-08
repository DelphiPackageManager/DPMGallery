import * as React from "react";
import { NavLink } from "react-router-dom";

const Footer = () => {
  return (
    <footer className="p-4 sm:p-6 bg-primary dark:bg-primary-900 text-base text-white ">
      <div className="mx-auto max-w-6xl">
        <div className="md:flex md:justify-between">
          <div className="mb-6 md:mb-0">
            <a href="https://delphi.dev" className="flex items-center">
              {/* <img src="https://flowbite.com/docs/images/logo.svg" className="mr-3 h-8" alt="DPM Logo" /> */}
              <span className="self-center text-xl whitespace-nowrap ">DPM</span>
            </a>
          </div>
          <div className="grid grid-cols-3 gap-2 md:gap-4 sm:grid-cols-3">
            <div>
              <h2 className="mb-1 md:mb-2 text-sm md:text-sm font-semibold uppercase">Support</h2>
              <ul className="text-white text-opacity-70 text-sm">
                <li className="mb-3">
                  <a href="https://github.com/DelphiPackageManager/DPM/issues" target="_blank" className="hover:text-opacity-100 hover:text-white">
                    Issues
                  </a>
                </li>
                <li>
                  <a
                    href="https://github.com/DelphiPackageManager/DPM/discussions"
                    target="_blank"
                    className="hover:text-opacity-100 hover:text-white">
                    Discussions
                  </a>
                </li>
              </ul>
            </div>
            <div>
              <h2 className="mb-1 md:mb-2 text-xs md:text-sm font-semibold uppercase">Resources</h2>
              <ul className="text-white text-opacity-70 text-sm">
                <li className="mb-3">
                  <a href="https://docs.delphi.dev/dpm" target="_blank" className="hover:text-opacity-100 hover:text-white ">
                    Documentation
                  </a>
                </li>
                <li>
                  <a href="https://www.finalbuilder.com/resources/blogs" target="_blank" className="hover:text-opacity-100 hover:text-white">
                    Blogs
                  </a>
                </li>
                <li></li>
              </ul>
            </div>
            <div>
              <h2 className="mb-1 md:mb-2 text-xs md:text-sm font-semibold uppercase ">Legal</h2>
              <ul className="text-white text-opacity-70 text-sm">
                <li className="mb-3">
                  <NavLink to="/policies/privacy" className="hover:text-opacity-100 hover:text-white">
                    Privacy Policy
                  </NavLink>
                </li>
                <li className="mb-3">
                  <NavLink to="/policies/terms" className="hover:text-opacity-100 hover:text-white">
                    Terms &amp; Conditions
                  </NavLink>
                </li>
                <li>
                  <NavLink to="/policies/package" className="hover:text-opacity-100 hover:text-white">
                    Package Policies
                  </NavLink>
                </li>
              </ul>
            </div>
          </div>
        </div>
        <hr className="my-3 sm:mx-auto border-primary-700 lg:my-4" />
        <div className="sm:flex sm:items-center sm:justify-between">
          <span className="text-xs sm:text-center text-white text-opacity-60  ">
            Copyright © {new Date().getFullYear()} Vincent Parrett & Contributors. All Rights Reserved.
          </span>
          <div className="flex mt-4 space-x-6 sm:justify-center sm:mt-0">
            <a href="https://twitter.com/delphipm" target="_blank" className="text-white text-opacity-60 hover:text-opacity-100">
              <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 24 24" aria-hidden="true">
                <path d="M8.29 20.251c7.547 0 11.675-6.253 11.675-11.675 0-.178 0-.355-.012-.53A8.348 8.348 0 0022 5.92a8.19 8.19 0 01-2.357.646 4.118 4.118 0 001.804-2.27 8.224 8.224 0 01-2.605.996 4.107 4.107 0 00-6.993 3.743 11.65 11.65 0 01-8.457-4.287 4.106 4.106 0 001.27 5.477A4.072 4.072 0 012.8 9.713v.052a4.105 4.105 0 003.292 4.022 4.095 4.095 0 01-1.853.07 4.108 4.108 0 003.834 2.85A8.233 8.233 0 012 18.407a11.616 11.616 0 006.29 1.84" />
              </svg>
            </a>
            <a href="https://github.com/DelphiPackageManager" target="_blank" className="text-white text-opacity-60 hover:text-opacity-100">
              <svg className="w-5 h-5" fill="currentColor">
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
