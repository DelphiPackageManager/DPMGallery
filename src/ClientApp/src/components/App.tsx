import { createBrowserRouter, createRoutesFromElements, Route, RouterProvider } from "react-router-dom";
import ConfirmEmailPage from "./confirmEmailPage";
import DownloadsPage from "./downloadsPage";

import { Fragment, ReactNode, useEffect } from "react";
import { useFetcher } from "react-router-dom";
import { User } from "../context/AuthProvider";
import useAuth from "../hooks/useAuth";
import useAxiosPrivate from "../hooks/useAxiosPrivate";
import LoginPage from ".//loginPage";
import ErrorPage from "./errorPage";
import ForgotPasswordPage from "./forgotPasswordPage";
import HomePage from "./homePage";
import Layout, { LayoutLoader } from "./layout";
import NotFoundPage from "./notfoundPage";
import PackagePoliciesPage from "./packagePoliciesPage";
import PackagePage from "./packagesPage/packagePage";
import PackagesPage from "./packagesPage/packagesPage";
import PrivacyPage from "./policies/privacyPage";
import TermsPage from "./policies/termsPage";
import ProfilePage from "./profilePage";
import RegisterPage from "./registerPage";
import RequireAuthRoute from "./requireAuth";
import UploadPage from "./uploadPage";

type AuthWrapperProps = {
  children: ReactNode;
};

// const AuthWrapper = ({ children }: AuthWrapperProps) => {
//   const { setAuth } = useAuth();
//   const axiosPrivate = useAxiosPrivate();

//   const PROFILE_URL = "/ui/auth/profile";

//   useEffect(() => {
//     const fetchData = async () => {
//       try {
//         //using axios private so we get the new refresh token.
//         const response = await axiosPrivate.post(
//           PROFILE_URL,
//           {},
//           {
//             withCredentials: true,
//           }
//         );
//         if (!response.data) {
//           console.log("no data");
//           return;
//         }

//         const username = response?.data?.userName;
//         const email = response?.data?.email;
//         const emailConfirmed = response?.data?.emailConfirmed;
//         const roles = response?.data?.roles;
//         const avatarUrl = response?.data?.avatarUrl;

//         const currentUser: User = {
//           user: {
//             userName: username,
//             email: email,
//             emailConfirmed: emailConfirmed,
//             roles: roles,
//             avatarUrl: avatarUrl,
//           },
//         };
//         console.log("profile");
//         console.log(currentUser);
//         setAuth(currentUser);
//       } catch (err) {}
//     };
//     fetchData();
//   }, []);

//   return <Fragment>{children}</Fragment>;
// };

const App = () => {
  const router = createBrowserRouter(
    createRoutesFromElements(
      <Route path="/" element={<Layout />} loader={LayoutLoader} errorElement={<ErrorPage />}>
        <Route index element={<HomePage />} />
        <Route path="/packages" element={<PackagesPage />} />
        <Route path="/packages/:packageId/:packageVersion/" element={<PackagePage />} />

        <Route element={<RequireAuthRoute allowedRoles={["RegisteredUser", "Administrator"]} />}>
          <Route path="/upload" element={<UploadPage />} />
        </Route>
        <Route path="/downloads" element={<DownloadsPage />} />

        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/confirmemail" element={<ConfirmEmailPage />} />
        <Route path="/forgotpassword" element={<ForgotPasswordPage />} />

        <Route path="/policies/terms" element={<TermsPage />} />
        <Route path="/policies/privacy" element={<PrivacyPage />} />
        <Route path="/policies/package" element={<PackagePoliciesPage />} />

        <Route path="/profiles/:userName" element={<ProfilePage />} />

        <Route path="*" element={<NotFoundPage />} />
      </Route>
    )
  );

  return <RouterProvider router={router} />;
};

export default App;
