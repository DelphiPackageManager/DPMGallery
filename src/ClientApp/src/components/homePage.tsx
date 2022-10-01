import PackageSearchBar from "./packageSearchBar";

const HomePage = () => {
    return (
        <>
            <div className="bg-gray-900 w-full sticky top-[3.5rem] ">
                <div className="container text-gray-100 pb-1 mx-auto center-text">
                    <div className="flex flex-col justify-center items-center">
                        <h1 className="text-xl md:text-2xl">DPM</h1>
                        <h2 className="text-sm md:text-lg ">The Package Manager for Delphi</h2>
                    </div>
                </div>
                <div className="container mx-auto flex flex-row items-center justify-center py-2 pt-1 mt-0 ">
                    <PackageSearchBar doNavigateOnClear={false} value={''} />
                </div>
            </div>
            <div className="w-full pt-4">
                <div className="container mx-auto max-w-6xl text-base text-gray-900 dark:text-gray-300 pb-2  text-center">
                    <div className="px-2">
                    <h1>Coming Soon</h1>
                    <p>We're working hard to get this service going - this is here only for testing for now xxxxxxxxxxxxxxxxxxxxxxxxxxxxx ssssssssssssssssssssssss sdsd sda sdasdas.</p>
                    <br/>
                    <br/>
                    <br/>
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
                    </div>
                </div>
            </div>
        </>
    )
}

export default HomePage;