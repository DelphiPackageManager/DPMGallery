import React from "react";
import ReactDOM from "react-dom/client";
import { HelmetProvider } from "react-helmet-async";
import App from "./components/App";
import { AuthProvider, User } from "./context/AuthProvider";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { TooltipProvider } from "./components/ui/tooltip";
import fetchIdentity from "./fechIdentity";
import "./index.css";

//can't call async from top level functions so wrap as an expression and invoke it.
(async function () {
  let user = await fetchIdentity();
  const queryClient = new QueryClient();

  //note strict mode causes useeffect to be run twice in dev mode - not a bug
  ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
    <React.StrictMode>
      <AuthProvider user={user}>
        <HelmetProvider>
          <TooltipProvider>
            <QueryClientProvider client={queryClient}>
              <ReactQueryDevtools initialIsOpen={false} />
              <App />
            </QueryClientProvider>
          </TooltipProvider>
        </HelmetProvider>
      </AuthProvider>
    </React.StrictMode>
  );
})();
