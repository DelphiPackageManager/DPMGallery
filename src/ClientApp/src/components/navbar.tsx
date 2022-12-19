import { useEffect, useState } from "react";
import { NavLink, useLocation } from "react-router-dom";
import { useWindowSize } from "usehooks-ts";
import DarkModeToggle from "./darkModeToggle";

// import useAuth so we can tell if logged in
export default function NavBar() {
  const [isNavExpanded, setIsNavExpanded] = useState(false);
  let location = useLocation();
  const { width, height } = useWindowSize();

  const buttonHandler = (event: React.MouseEvent<HTMLButtonElement>) => {
    event.preventDefault();
    setIsNavExpanded(!isNavExpanded);
  };

  //close the mobile menu when we change routes.
  useEffect(() => {
    setIsNavExpanded(false);
  }, [location, width, height]);

  const navStyle =
    "block py-2 pr-4 pl-3 border-b border-primary-600 hover:bg-primary-900 dark:hover:bg-primary  md:hover:bg-inherit md:dark:hover:bg-inherit md:hover:opacity-80   md:border-0 md:p-0";

  return (
    <nav className="bg-primary dark:bg-primary-900 text-white  min-h-[3.5rem] py-2 z-10 px-4 md:px-6 fixed top-0 w-full ">
      <div className="flex flex-wrap justify-between items-center mx-auto max-w-5xl">
        <div className="flex items-center">
          <NavLink className="self-center text-xl whitespace-nowrap text-white mr-3" to="/">
            DPM
          </NavLink>
        </div>
        <div className="flex flex-row items-center md:order-2">
          <DarkModeToggle />
          <NavLink className=" focus:ring-4 font-medium rounded-lg text-sm py-2 mx-1 hover:opacity-80" to="/login">
            Login
          </NavLink>
          <button
            type="button"
            onClick={buttonHandler}
            className="inline-flex items-center p-2 ml-1 text-sm  md:hidden hover:opacity-80 focus:outline-none "
            aria-controls="mobile-menu-2"
            aria-expanded="false">
            <span className="sr-only">Open main menu</span>
            <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
              <path
                fillRule="evenodd"
                d="M3 5a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 10a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1zM3 15a1 1 0 011-1h12a1 1 0 110 2H4a1 1 0 01-1-1z"
                clipRule="evenodd"></path>
            </svg>
            <svg className="hidden w-6 h-6" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
              <path
                fillRule="evenodd"
                d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                clipRule="evenodd"></path>
            </svg>
          </button>
        </div>

        <div
          className={`${
            isNavExpanded ? "" : "hidden"
          } absolute md:relative top-14 md:top-[unset] left-0 md:left-[unset] bg-primary dark:bg-primary-900 justify-between items-center w-full md:flex md:w-auto md:order-1`}
          id="mobile-menu-2">
          <ul className="flex flex-col mt-4 font-medium md:flex-row md:space-x-8 md:mt-0">
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
              <NavLink to="/upload" className={navStyle} aria-current="page">
                Upload
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
