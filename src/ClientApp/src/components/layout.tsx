import { Outlet } from 'react-router-dom';
import Footer from './footer';
import NavBar from './navbar';

const Layout = () => {
    return (
       <>
            <div className="flex flex-col m-0 h-screen bg-gray-800 text-gray-300" >
                <NavBar />
                <div className='flex-grow dark:bg-gray-800 bg-gray-50 mt-[3.5rem]'>
                    <Outlet />
                </div>
                <Footer/>
            </div>     
       </> 
    );
};

export default Layout;