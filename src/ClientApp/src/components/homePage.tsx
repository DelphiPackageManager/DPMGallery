import PackageSearchBar from "./packageSearchBar";
import PageContainer from "./pageContainer";

const HomePage = () => {
  return (
    <>
      {/* This can't be in the page container due to sticky position */}
      <div className="bg-primary  dark:bg-primary-900 w-full sticky top-[3.5rem] ">
        <div className="container text-white pb-1 mx-auto center-text">
          {/* 
          <div className="flex flex-col justify-center items-center">
            <h1 className="text-xl md:text-2xl">DPM</h1>
            <h2 className="text-sm md:text-lg ">The Package Manager for Delphi</h2>
          </div>
          */}
        </div>
        <div className="container mx-auto flex flex-row items-center justify-center py-2 pt-1 mt-0 ">
          <PackageSearchBar doNavigateOnClear={false} value={""} />
        </div>
      </div>

      <PageContainer className="px-2">
        <div className="pt-8 text-gray-600 dark:text-gray-400 mb-8">
          <h1 className="pb-4 text-lg md:text-4xl  dark:text-gray-200">DPM, the package manager for Delphi developers.</h1>
          <h3>Open source package manager for Delphi XE2 - 11.3 </h3>
        </div>
        <div className="flex flex-row flex-1 gap-4">
          <div className="bg-primary-700 text-gray-100 p-4 rounded-md w-1/2 ">
            <a href="#">
              <div className="flex flex-col h-full">
                <div className="text-left h-12 w-12 mb-2">Image</div>
                <span>Get Started</span>
              </div>
            </a>
          </div>
          <div className=" bg-primary-700 text-gray-100 p-4 rounded-md w-1/2">
            <a href="#">
              <div className="flex flex-col h-full">
                <div className="text-left h-12 w-12 mb-2">Image</div>
                <span>Publish Packages</span>
              </div>
            </a>
          </div>
        </div>
      </PageContainer>
    </>
  );
};

export default HomePage;
