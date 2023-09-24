import PackageSearchBar from "../components/packageSearchBar";
import PageContainer from "../components/pageContainer";

const HomePage = () => {
  return (
    <>
      {/* This can't be in the page container due to sticky position */}
      <div className="bg-brand w-full sticky top-[3.5rem] ">
        <div className="container mx-auto flex flex-row items-center justify-center py-2 pt-1 mt-0 ">
          <PackageSearchBar doNavigateOnClear={false} value={""} />
        </div>
      </div>

      <PageContainer className="px-2 ">
        <div className="pt-8 text-gray-600 dark:text-gray-400 mb-8">
          <div className="flex flex-row items-center ">
            <img src="/img/dpm-large.png" className="mr-4 w-16 h-16 md:w-20 md:h-20" />
            <h1 className="pb-4  align-middle text-lg md:text-4xl  dark:text-gray-200">DPM, the package manager for Delphi developers.</h1>
          </div>

          <h3>Open source package manager for Delphi XE2 - 11.3 </h3>
        </div>
        <div className="flex flex-row flex-wrap md:flex-nowrap  gap-4 ">
          <div className="bg-brand text-gray-100 p-4 rounded-md basis-full ">
            <a href="https://docs.delphi.dev/getting-started/installing.html" target="_blank">
              <div className="flex flex-row h-full">
                <div className="text-left h-12 w-12 mb-2">
                  <img src="/img/dpm32.png" />
                </div>
                <div>
                  <h3>Get Started</h3>
                  <p>Get started using DPM with your projects.</p>
                </div>
              </div>
            </a>
          </div>
          <div className="bg-brand text-gray-100 p-4 rounded-md basis-full">
            <a href="https://docs.delphi.dev/getting-started/creating-packages.html" target="_blank">
              <div className="flex flex-row h-full">
                <div className="text-left h-12 w-12 mb-2">
                  <img src="/img/dpm32.png" />
                </div>
                <div>
                  <h3>Publish Packages</h3>
                  <p>Learn how to author and publish packages.</p>
                </div>
              </div>
            </a>
          </div>
        </div>
      </PageContainer>
    </>
  );
};

export default HomePage;
