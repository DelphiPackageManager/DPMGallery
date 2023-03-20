import * as React from "react";
import { useNavigate } from "react-router-dom";

export interface IPackageSearchBarProps {
  doNavigateOnClear?: boolean;
  value: string;
}

const PackageSearchBar: React.FunctionComponent<IPackageSearchBarProps> = ({ doNavigateOnClear = true, value = "" }) => {
  const navigate = useNavigate();

  const _handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      //trigger search
      const value = event.currentTarget.value;
      navigate(`/packages?q=${value}`);
      return;
    }
    if (event.key === "Escape") {
      //clear search
      event.currentTarget.value = "";
      if (doNavigateOnClear) {
        navigate("/packages");
      }
    }
  };

  return (
    <div className="flex justify-center items-center py-2 w-full max-w-sm md:max-w-xl">
      <div className="w-full flex flex-col">
        <div className="relative flex justify-around items-center ">
          <span className="absolute inset-y-0 left-0 flex items-center justify-center">
            <button type="submit" className="p-2 focus:outline-none text-gray-400">
              <svg
                fill="none"
                stroke="currentColor"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                viewBox="0 0 24 24"
                className="w-6 h-6">
                <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
              </svg>
            </button>
          </span>
          <input
            autoFocus
            type="text"
            name="q"
            id="search"
            className="w-full focus:outline-none focus:shadow-none  py-1 text-base text-gray-900  rounded-full pl-12 border-none placeholder:text-gray-200 dark:placeholder:text-gray-500 dark:bg-gray-700 dark:text-white"
            placeholder="Search Packages"
            autoComplete="off"
            onKeyDown={_handleKeyDown}
            defaultValue={value}
          />
        </div>
      </div>
    </div>
  );
};

export default PackageSearchBar;
