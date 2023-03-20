import PackageSearchBar from "./packageSearchBar";
import PageContainer from "./pageContainer";

const HomePage = () => {
  return (
    <>
      {/* This can't be in the page container due to sticky position*/}
      <div className="bg-primary  dark:bg-primary-900 w-full sticky top-[3.5rem] ">
        <div className="container text-white pb-1 mx-auto center-text">
          <div className="flex flex-col justify-center items-center">
            <h1 className="text-xl md:text-2xl">DPM</h1>
            <h2 className="text-sm md:text-lg ">The Package Manager for Delphi</h2>
          </div>
        </div>
        <div className="container mx-auto flex flex-row items-center justify-center py-2 pt-1 mt-0 ">
          <PackageSearchBar doNavigateOnClear={false} value={""} />
        </div>
      </div>
      <PageContainer className="px-2">
        <h1>Coming Soon</h1>
        <p>
          We're working hard to get this service going - this is here only for testing for now xxxxxxxxxxxxxxxxxxxxxxxxxxxxx ssssssssssssssssssssssss
          sdsd sda sdasdas.
        </p>
        <br />
        <br />
        <br />
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
        <p>Foo bar...</p>
      </PageContainer>
    </>
  );
};

export default HomePage;
