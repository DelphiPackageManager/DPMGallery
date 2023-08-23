import { createBrowserRouter, createRoutesFromElements, Navigate, Route, RouterProvider } from "react-router-dom";
import ConfirmEmailPage from "./confirmEmailPage";
import DownloadsPage from "./downloadsPage";
import StatsPage from "./statsPage";

import DisableAuthenticatorPage from "./account/2fa/disable2fa";
import EnableAuthenticatorPage from "./account/2fa/enableauthenticator";
import GenerateRecoveryCodesPage from "./account/2fa/generateRecoveryCodes";
import ResetAuthenticatorAppPage from "./account/2fa/resetAuthenticatorApp";
import ShowRecoveryCodesPage from "./account/2fa/showRecoveryCodes";
import TwoFactorAuthenticationPage from "./account/2fa/twoFactorAuthentication";
import AccountLayout from "./account/accountLayout";
import APIKeysPage from "./account/apiKeys";
import EmailSettingsPage from "./account/email";
import ExternalLoginsPage from "./account/externalLogins";
import MyPackagesPage from "./account/myPackages";
import OrganisationsPage from "./account/organisations";
import PasswordSettingsPage from "./account/password";
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
import ResetPasswordPage from "./resetPasswordPage";
import RickRoll from "./rickRoll";
import UploadPage from "./uploadPage";

const App = () => {
  const router = createBrowserRouter(
    createRoutesFromElements(
      <Route path="/" element={<Layout />} errorElement={<ErrorPage />}>
        <Route index element={<HomePage />} />
        <Route path="/packages" element={<PackagesPage />} />
        <Route path="/packages/:packageId/:packageVersion/" element={<PackagePage />} />
        <Route path="/packages/:packageId/" element={<PackagePage />} />
        <Route path="/downloads" element={<DownloadsPage />} />
        <Route path="/stats" element={<StatsPage />} />

        <Route path="/login" element={<LoginPage />} />
        <Route path="/loginwith2fa" element={<LoginWith2faPage />} />
        <Route path="/lockedout" element={<LockedOutPage />} />
        <Route path="/externallogin" element={<ExternalLoginPage />} />
        <Route path="/createaccount" element={<RegisterPage />} />
        <Route path="/confirmemail" element={<ConfirmEmailPage />} />
        <Route path="/forgotpassword" element={<ForgotPasswordPage />} />
        <Route path="/resetpassword" element={<ResetPasswordPage />} />

        <Route path="/policies/terms" element={<TermsPage />} />
        <Route path="/policies/privacy" element={<PrivacyPage />} />
        <Route path="/policies/package" element={<PackagePoliciesPage />} />
        <Route path="/profiles/:userName" element={<ProfilePage />} />

        <Route path="/wp-login" element={<RickRoll />} />
        <Route path="/wp-login.php" element={<RickRoll />} />
        <Route path="/owa/auth.owa" element={<RickRoll />} />
        <Route path="/.git/config" element={<RickRoll />} />

        <Route element={<RequireAuthRoute allowedRoles={["RegisteredUser", "Administrator"]} />}>
          <Route path="/upload" element={<UploadPage />} />
          <Route path="/account" element={<AccountLayout />}>
            <Route path="/account/email" element={<EmailSettingsPage />} />
            <Route path="/account/password" element={<PasswordSettingsPage />} />
            <Route path="/account/apikeys" element={<APIKeysPage />} />
            <Route path="/account/packages" element={<MyPackagesPage />} />
            <Route path="/account/externallogins" element={<ExternalLoginsPage />} />
            <Route path="/account/twofactorauth" element={<TwoFactorAuthenticationPage />} />
            <Route path="/account/enableauthenticator" element={<EnableAuthenticatorPage />} />
            <Route path="/account/disable2fa" element={<DisableAuthenticatorPage />} />
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
