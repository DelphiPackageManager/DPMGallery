import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import PackageSearchBar from "../components/packageSearchBar";
import PageContainer from "../components/pageContainer";

const HomePage = () => {
	const navigate = useNavigate();
	const [query, setQuery] = useState("");

	useEffect(() => {
		if (query) navigate(`/packages?q=${query}`);
	}, [query]);

	return (
		<>
			{/* This can't be in the page container due to sticky position */}
			<div className="sticky top-[3.5rem] w-full bg-brand">
				<div className="container z-10 mx-auto mt-0 flex flex-row items-center justify-center py-2 pt-1">
					<PackageSearchBar value={query} onChange={setQuery} />
				</div>
			</div>

			<PageContainer className="px-2">
				<div className="mb-8 pt-8 text-gray-600 dark:text-gray-400">
					<div className="flex flex-row items-center">
						<img src="/img/dpm-large.png" className="mr-4 h-16 w-16 md:h-20 md:w-20" />
						<h1 className="pb-4 align-middle text-lg dark:text-gray-200 md:text-4xl">DPM, the package manager for Delphi developers.</h1>
					</div>
					<h2 className="ml-24 text-xl font-medium">The Open source package manager for Delphi XE2 - 12.x </h2>


				</div>
				<div className="flex flex-row flex-wrap gap-4 md:flex-nowrap">
					<div className="basis-full rounded-md bg-brand p-4 text-gray-100">
						<a href="https://docs.delphi.dev/getting-started/installing.html" target="_blank">
							<div className="flex h-full flex-row">
								<div className="mb-2 h-12 w-12 text-left">
									<img src="/img/dpm32.png" />
								</div>
								<div>
									<h3>Get Started</h3>
									<p>Get started using DPM with your projects.</p>
								</div>
							</div>
						</a>
					</div>
					<div className="basis-full rounded-md bg-brand p-4 text-gray-100">
						<a href="https://docs.delphi.dev/getting-started/creating-packages.html" target="_blank">
							<div className="flex h-full flex-row">
								<div className="mb-2 h-12 w-12 text-left">
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
