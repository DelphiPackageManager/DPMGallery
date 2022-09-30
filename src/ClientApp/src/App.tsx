import { Routes, Route } from "react-router-dom"
import ConfirmEmailPage from "./components/confirmEmailPage"
import DownloadsPage from "./components/downloadsPage"
import ForgotPasswordPage from "./components/forgotPasswordPage"
import HomePage from "./components/homePage"

import Layout from "./components/layout"
import LoginPage from "./components/loginPage"
import NotFoundPage from "./components/notfoundPage"
import PackagePoliciesPage from "./components/packagePoliciesPage"
import PackagePage from "./components/packages/packagePage"
import PackagesPage from "./components/packages/packagesPage"
import PrivacyPage from "./components/policies/privacyPage"
import TermsPage from "./components/policies/termsPage"
import ProfilePage from "./components/profilePage"
import RegisterPage from "./components/registerPage"
import UploadPage from "./components/uploadPage"

const App = () => {
  return (
    <Routes >
      <Route element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="/packages" element={<PackagesPage />} />
        <Route path="/packages/:packageid/:packageVersion/" element={<PackagePage />} />

        <Route path="/upload" element={<UploadPage />} />
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
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}

export default App
