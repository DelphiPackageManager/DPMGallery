import { Outlet } from 'react-router-dom';
import Footer from './footer';
import NavBar from './navbar';

const Layout = () => {
    return (
       <>
            <div className="flex flex-col m-0 h-screen bg-gray-50 dark:bg-slate-800 text-gray-600 dark:text-gray-300" >
                <NavBar />
                <div className="flex-grow">
                    <Outlet />
                </div>              
            </div>     
            <Footer/>
       </> 
    );
};

export default Layout;