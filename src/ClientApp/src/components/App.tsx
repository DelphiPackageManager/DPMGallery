import { createBrowserRouter, createRoutesFromElements, Navigate, Route, RouterProvider } from "react-router-dom";
import ConfirmEmailPage from "./confirmEmailPage";
import DownloadsPage from "./downloadsPage";

import AccountLayout from "./account/accountLayout";
import APIKeysPage from "./account/apiKeys";
import EnableAuthenticatorPage from "./account/enableauthenticator";
import ExternalLoginsPage from "./account/externalLogins";
import GenerateRecoveryCodesPage from "./account/generateRecoveryCodes";
import MyPackagesPage from "./account/myPackages";
import OrganisationsPage from "./account/organisations";
import ResetAuthenticatorAppPage from "./account/resetAuthenticatorApp";
import UserSettingsPage from "./account/settings";
import ShowRecoveryCodesPage from "./account/showRecoveryCodes";
import TwoFactorAuthenticationPage from "./account/twoFactorAuthentication";
import ErrorPage from "./errorPage";
import ExternalLoginPage from "./externalLogin";
import ForgotPasswordPage from "./forgotPasswordPage";
import HomePage from "./homePage";
import Layout, { LayoutLoader } from "./layout";
import LockedOutPage from "./lockedOutPage";
import LoginPage from "./loginPage";
import LoginWith2faPage from "./loginWith2fa";
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

const App = () => {
  const router = createBrowserRouter(
    createRoutesFromElements(
      <Route path="/" element={<Layout />} errorElement={<ErrorPage />}>
        <Route index element={<HomePage />} />
        <Route path="/packages" element={<PackagesPage />} />
        <Route path="/packages/:packageId/:packageVersion/" element={<PackagePage />} />
        <Route path="/downloads" element={<DownloadsPage />} />

        <Route path="/login" element={<LoginPage />} />
        <Route path="/loginwith2fa" element={<LoginWith2faPage />} />
        <Route path="/lockedout" element={<LockedOutPage />} />
        <Route path="/externallogin" element={<ExternalLoginPage />} />
        <Route path="/createaccount" element={<RegisterPage />} />
        <Route path="/confirmemail" element={<ConfirmEmailPage />} />
        <Route path="/forgotpassword" element={<ForgotPasswordPage />} />

        <Route path="/policies/terms" element={<TermsPage />} />
        <Route path="/policies/privacy" element={<PrivacyPage />} />
        <Route path="/policies/package" element={<PackagePoliciesPage />} />

        <Route path="/profiles/:userName" element={<ProfilePage />} />

        <Route element={<RequireAuthRoute allowedRoles={["RegisteredUser", "Administrator"]} />}>
          <Route path="/upload" element={<UploadPage />} />
          <Route path="/account" element={<AccountLayout />}>
            <Route path="/account/settings" element={<UserSettingsPage />} />
            <Route path="/account/apikeys" element={<APIKeysPage />} />
            <Route path="/account/packages" element={<MyPackagesPage />} />
            <Route path="/account/externallogins" element={<ExternalLoginsPage />} />
            <Route path="/account/twofactorauth" element={<TwoFactorAuthenticationPage />} />
            <Route path="/account/enableauthenticator" element={<EnableAuthenticatorPage />} />
            <Route path="/account/resetauthenticator" element={<ResetAuthenticatorAppPage />} />
            <Route path="/account/showrecoverycodes" element={<ShowRecoveryCodesPage />} />
            <Route path="/account/generaterecoverycodes" element={<GenerateRecoveryCodesPage />} />
            <Route path="/account/organisations" element={<OrganisationsPage />} />
          </Route>
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Route>
    )
  );

  return <RouterProvider router={router} />;
};

export default App;
