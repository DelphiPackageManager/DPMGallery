import PackageSearchBar from "./packageSearchBar";

const HomePage = () => {
    return (
        <>
            <div className="bg-gray-200 dark:bg-gray-900">
                <div className="w-full bg-primary">
                    <div className="container text-white pb-2 mx-auto center-text">
                        <div className="flex flex-col justify-center items-center py-1 ">
                            <h1 className="text-2xl md:text-3xl">DPM</h1>
                            <h2 className="text-base md:text-xl ">The Package Manager for Delphi</h2>
                        </div>
                    </div>
                </div>
                <div className="container mx-auto flex flex-row items-center justify-center py-2">
                    <PackageSearchBar doNavigateOnClear={false} value={''} />
                </div>
            </div>
            <div className="w-full pt-4">
                <div className="container text-base text-gray-600 dark:text-gray-300 pb-2 mx-auto max-w-7xl text-center">
                    <h1>Coming Soon</h1>
                    <p>We're working hard to get this service going - this is here only for testing for now.</p>
                </div>
            </div>
        </>
    )
}

export default HomePage;