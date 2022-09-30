import { NavLink } from "react-router-dom";
import DarkModeToggle from "./darkModeToggle";
// import useAuth so we can tell if logged in
export default function NavBar() {
    return (
        <nav className="bg-gray-200 dark:bg-gray-900 text-gray-900 dark:text-gray-200  top-0 py-2 z-10 border-b border-gray-300 shadow dark:shadow-none dark:border-none sticky">
            <div className="px-2 max-w-7xl mx-auto justify-content-center items-center" >
                <div className="flex justify-between items-center">
                    <div className="mr-2 items-center">
                        <NavLink className="text-3xl" to="/" >DPM</NavLink>
                    </div>

                    <div className="hidden md:flex items-center space-x-1 text-xl">
					    <NavLink className="py-2 px-3 hover:text-gray-800 dark:hover:text-white" to="/packages" >Packages</NavLink>
                        <NavLink className="py-2 px-3 hover:text-gray-800 dark:hover:text-white" to="/upload">Upload</NavLink>
                        <a className="py-2 px-3 hover:text-gray-800 dark:hover:text-white" href="#">Documentation</a>
                        <NavLink className="hidden lg:flex py-2 px-3 hover:text-gray-800 dark:hover:text-white" to="/downloads" >Downloads</NavLink>
                        <a className="hidden lg:flex py-2 px-3 hover:text-gray-800 dark:hover:text-white" href="https://www.finalbuilder.com/resources/blogs/tag/dpm" target="_blank">Blog</a>
				    </div>

                    <div className="flex flex-row items-center">
                       <DarkModeToggle />
                       <NavLink to="/login">Login</NavLink> 
                    </div>
                </div>
            </div>
        </nav>)
}