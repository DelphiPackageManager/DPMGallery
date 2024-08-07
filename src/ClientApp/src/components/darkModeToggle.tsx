import { useEffect, useState } from "react";

const DarkModeToggle = () => {
  const [darkMode, setDarkMode] = useState<boolean | undefined>(undefined);
  const [preferDark, setPreferDark] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);
	useEffect(() => {
		let pf = window.matchMedia("(prefers-color-scheme: dark)").matches;
		setPreferDark(pf);
		let ldm = pf;
		let dms = localStorage.getItem("darkMode");
		if (dms) {
			ldm = JSON.parse(dms);
		}
		setDarkMode(ldm);
		setIsLoading(false);
	}, []);

	useEffect(() => {
		if (isLoading) {
			return;
		}
		if (darkMode) {
			window.document.documentElement.classList.add("dark");
			if (preferDark) {
				// if we prefer dark then we don't need to store it
				localStorage.removeItem("darkMode");
			} else {
				localStorage.setItem("darkMode", "true");
			}
		} else {
			window.document.documentElement.classList.remove("dark");
			if (preferDark) {
				//user prefers light mode so we need to store it
				localStorage.setItem("darkMode", "false");
			} else {
				// if we prefer dark then we don't need to store it
				localStorage.removeItem("darkMode");
			}
		}
	}, [darkMode]);


  const onClick = () => {
    setDarkMode(!darkMode);
  };

  return (
    <div className="mx-1">
      <button
        id="theme-toggle"
        aria-label="Toggle Theme"
        type="button"
        onClick={onClick}
        className="text-white hover:opacity-80 focus:outline-none text-sm p-2.5">
        {!darkMode && (
          <svg id="theme-toggle-dark-icon" className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
            <path d="M17.293 13.293A8 8 0 016.707 2.707a8.001 8.001 0 1010.586 10.586z"></path>
          </svg>
        )}
        {darkMode && (
          <svg id="theme-toggle-light-icon" className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
            <path
              d="M10 2a1 1 0 011 1v1a1 1 0 11-2 0V3a1 1 0 011-1zm4 8a4 4 0 11-8 0 4 4 0 018 0zm-.464 4.95l.707.707a1 1 0 001.414-1.414l-.707-.707a1 1 0 00-1.414 1.414zm2.12-10.607a1 1 0 010 1.414l-.706.707a1 1 0 11-1.414-1.414l.707-.707a1 1 0 011.414 0zM17 11a1 1 0 100-2h-1a1 1 0 100 2h1zm-7 4a1 1 0 011 1v1a1 1 0 11-2 0v-1a1 1 0 011-1zM5.05 6.464A1 1 0 106.465 5.05l-.708-.707a1 1 0 00-1.414 1.414l.707.707zm1.414 8.486l-.707.707a1 1 0 01-1.414-1.414l.707-.707a1 1 0 011.414 1.414zM4 11a1 1 0 100-2H3a1 1 0 000 2h1z"
              fillRule="evenodd"
              clipRule="evenodd"></path>
          </svg>
        )}
      </button>
    </div>
  );
};

export default DarkModeToggle;
