import { Outlet } from 'react-router-dom';
import Footer from './footer';
import NavBar from './navbar';

const Layout = () => {
    return (
       <>
            <div className="flex flex-col m-0 h-screen dark:bg-gray-800 bg-gray-50 dark:text-gray-300 text-gray-900" >
                <NavBar />
                <div className='flex-grow dark:bg-gray-800 bg-gray-50'>
                    <Outlet />
                </div>
                <Footer/>
            </div>     
       </> 
    );
};

export default Layout;