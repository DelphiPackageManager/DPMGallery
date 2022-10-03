import { Outlet } from 'react-router-dom';
import Footer from './footer';
import NavBar from './navbar';

const Layout = () => {
    return (
       <>
            <div className="flex flex-col m-0 h-screen bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100" >
                <NavBar />
                <div className='flex-grow mt-[3.5rem]'>
                    <Outlet />
                </div>
                <Footer/>
            </div>     
       </> 
    );
};

export default Layout;