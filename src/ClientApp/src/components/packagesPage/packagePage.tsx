import React, {useEffect, useRef} from 'react';
import usePackageInfo, {InfoParams} from "../../hooks/usePackageDetails";
import PageContainer from '../pageContainer'
import { NavLink, useParams } from 'react-router-dom'
import { ClockIcon, ArrowDownTrayIcon, AtSymbolIcon, TagIcon, InformationCircleIcon } from '@heroicons/react/24/outline'
import { Tab } from '@headlessui/react'

import { useRemark } from 'react-remark';
import remarkGfm from 'remark-gfm'
//TODO : find a cleaner way to do this.
// import DelphiXE2 from '../../assets/Delphi-XE2.svg';
// import DelphiXE3 from '../../assets/Delphi-XE3.svg';
// import DelphiXE4 from '../../assets/Delphi-XE4.svg';
// import DelphiXE5 from '../../assets/Delphi-XE5.svg';
// import DelphiXE6 from '../../assets/Delphi-XE6.svg';
// import DelphiXE7 from '../../assets/Delphi-XE7.svg';
// import DelphiXE8 from '../../assets/Delphi-XE8.svg';
// import Delphi10 from '../../assets/Delphi-10.0.svg';
// import Delphi10_1 from '../../assets/Delphi-10.1.svg';
// import Delphi10_2 from '../../assets/Delphi-10.2.svg';
// import Delphi10_3 from '../../assets/Delphi-10.3.svg';
// import Delphi10_4 from '../../assets/Delphi-10.4.svg';
// import Delphi11 from '../../assets/Delphi-11.x.svg';
// import Delphi12 from '../../assets/Delphi-12.x.svg';
import usePackageDetails from '../../hooks/usePackageDetails';

type PackagePageProps = {
    //dasdasd: string;
};

const PackagePage: React.FC<PackagePageProps> = ({ }) => {
    let { packageId, packageVersion } = useParams();
    const effectRan = useRef(false)

    const [reactContent, setMarkdownSource] = useRemark();

    const controller = new AbortController();

    const [{loading, error, packageInfo}, getPackageInfo] = usePackageDetails();

    useEffect(() => {

      //get details from server here

          if (effectRan.current === true ) {
            getPackageInfo({packageId, packageVersion},controller);       
        }
        return () => {
            if (effectRan.current === true ) {
                controller.abort()
            }
            effectRan.current = true;
        
        }

      setMarkdownSource(`# readme must be markdown

> **one** sdfdfsdfd

two`);
    }, []);    
    
    
    
    
    let owner = 'vincent'; //test
    let totalDownloads = '1,234,343';

    const lightTabClasses = "ui-selected:bg-primary ui-selected:text-white ui-not-selected:bg-white  ui-not-selected:text-gray-800 ";
    const darkTabClasses = "dark:ui-selected:bg-primary-800 ui-selected:text-white/80 dark:ui-not-selected:bg-gray-800  dark:ui-not-selected:text-gray-200"
    const tabClasses = "px-3 py-2 outline-0 " + lightTabClasses + darkTabClasses ;



    return (
        <PageContainer>
            {error && <span className='text-red'>{error}</span>}
            <div className='flex flex-col-reverse md:flex-row justify-between w-full text-gray-300'>
                <div className='md:min-w-[16rem] md:mb-2 md:mt-6 px-2' >
                    <h2 className='text-base font-medium'>Owners</h2>
                    <div className='flex flex-row py-2 mb-2'>
                        <div className='w-12 h-12 mr-2'>
                            <img src='https://www.gravatar.com/avatar/537d208f522e2d50f727370037b3a75f' className='rounded-lg object-cover' />
                        </div>
                        <div className='py-2'>
                            <NavLink to={`/profiles/${owner}`}>vincent</NavLink>
                        </div>
                    </div>
                    <h2 className='text-base font-medium'>Downloads</h2>
                    <div className="py-1">
                        <span className='text-sm'>Total : </span>
                        <span className='text-xs'>{totalDownloads}</span>
                    </div>
                    <div className="py-1">
                        <span className='text-sm'>This version : </span>
                        <span className='text-xs'>{totalDownloads}</span>
                    </div>
                </div>
                <div className='grow'>
                    <div className='flex flex-row items-center mb-3 text-gray-700 dark:text-gray-100'>
                        <h1 className='text-2xl mr-3'>{packageId}</h1>
                        <span className='text-lg text-gray-300'>{packageVersion}</span>
                    </div>
                    <div className='flex flex-row items-start space-x-2 mb-2'>
                       {/* <img src={DelphiXE2} className="w-6 h-6"  title='Delphi XE2' />
                        <img src={DelphiXE3} className="w-6 h-6"  title='Delphi XE3' />
                        <img src={DelphiXE4} className="w-6 h-6"  title='Delphi XE4' />
                        <img src={DelphiXE5} className="w-6 h-6"  title='Delphi XE5' />
                        <img src={DelphiXE6} className="w-6 h-6"  title='Delphi XE6' />
                        <img src={DelphiXE7} className="w-6 h-6"  title='Delphi XE7' />
                        <img src={DelphiXE8} className="w-6 h-6"  title='Delphi XE8' />
                        <img src={Delphi10} className="w-6 h-6"  title='Delphi 10.0' />
                        <img src={Delphi10_1} className="w-6 h-6"  title='Delphi 10.1' />
                        <img src={Delphi10_2} className="w-6 h-6"  title='Delphi 10.2' />
                        <img src={Delphi10_3} className="w-6 h-6"  title='Delphi 10.3' />
                        <img src={Delphi10_4} className="w-6 h-6"  title='Delphi 10.4' />
                        <img src={Delphi11} className="w-6 h-6"  title='Delphi 11.x' />
                        <img src={Delphi12} className="w-6 h-6"  title='Delphi 12.x' />
    */}
                    </div>
                    <div className="flex items-center bg-primary-500/20 dark:bg-primary-500/50 text-gray-500 dark:text-gray-100 text-sm px-4 py-2 rounded-md " role="alert">
                        <InformationCircleIcon className='w-6 h-6 mr-2' />
                        <p>This is a prerelease version of Newtonsoft.Json.</p>
                    </div>
                    <div className='my-2'>
                        <Tab.Group>
                            <Tab.List className="border-b border-gray-100 dark:border-gray-700 text-base bg-white dark:bg-gray-800 ">
                                <Tab className={tabClasses}>Description</Tab>
                                <Tab className={tabClasses}>Compilers/Platforms</Tab>
                                <Tab className={tabClasses}>Versions</Tab>
                            </Tab.List>
                            <Tab.Panels className="text-sm mt-1 text-gray-800 dark:text-gray-50">
                                <Tab.Panel>
                                    {reactContent}
                                </Tab.Panel>
                                <Tab.Panel>Content 2</Tab.Panel>
                                <Tab.Panel>Content 3</Tab.Panel>
                            </Tab.Panels>
                        </Tab.Group>
                    </div>
                </div>
            </div>



        </PageContainer>
    );
};

export default PackagePage;
