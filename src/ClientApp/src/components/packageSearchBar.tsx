import { useEffect, useRef, useState } from "react";

export interface IPackageSearchBarProps {
  value: string;
  onChange?: (value: string) => void;
}

const PackageSearchBar = ({ value, onChange }: IPackageSearchBarProps) => {
  const [currentValue, setCurrentValue] = useState<string>(value);
  const inputTxt = useRef<HTMLInputElement>(null);
  const _handleKeyDown = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      //trigger search

      if (onChange) {
        onChange(currentValue);
      }
    } else if (event.key === "Escape") {
      setCurrentValue("");
      //clear search
      if (onChange) {
        onChange("");
      }
    }
    return true;
  };

  useEffect(() => {
    setCurrentValue(value);
    inputTxt?.current?.focus();
  }, [value]);

  const doOnChange = (v: string) => {
    setCurrentValue(v);

    //if (onChange) onChange(value);
  };

  const onSearchIconClick = () => {
    if (onChange) {
      onChange(currentValue);
    }
  };

  return (
    <div className="flex justify-center items-center py-2 w-full max-w-sm md:max-w-xl">
      <div className="w-full flex flex-col">
        <div className="relative flex justify-around items-center ">
          <span className="absolute inset-y-0 left-0 flex items-center justify-center">
            <button
              type="button"
              aria-label="Search"
              className="p-2 focus:outline-none text-gray-400 dark:text-gray-800"
              onClick={() => onSearchIconClick()}>
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
            className="searchBar w-full focus:outline-none focus:shadow-none p-2 !pl-10 text-base text-gray-900  rounded border-none placeholder:text-gray-300 dark:placeholder:text-gray-400 dark:text-gray-900"
            placeholder="Search Packages"
            autoComplete="off"
            onKeyDown={_handleKeyDown}
            onChange={(e) => doOnChange(e.target.value)}
            value={currentValue}
            ref={inputTxt}
          />
        </div>
      </div>
    </div>
  );
};

export default PackageSearchBar;
