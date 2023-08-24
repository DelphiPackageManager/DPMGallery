import React from "react";
import ReactDOM from "react-dom/client";
import App from "./components/App";
import { AuthProvider, User } from "./context/AuthProvider";

import fetchIdentity from "./fechIdentity";
import "./index.css";

//can't call async from top level functions so wrap as an expression and invoke it.
(async function () {
  let user = await fetchIdentity();
  //note strict mode causes useeffect to be run twice in dev mode - not a bug
  ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
    <React.StrictMode>
      <AuthProvider user={user}>
        <App />
      </AuthProvider>
    </React.StrictMode>
  );
})();
