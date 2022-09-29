import { Routes, Route } from "react-router-dom"
import ConfirmEmailPage from "./components/confirmEmailPage"
import DownloadsPage from "./components/downloadsPage"
import ForgotPasswordPage from "./components/forgotPasswordPage"
import HomePage from "./components/homePage"

import Layout from "./components/layout"
import LoginPage from "./components/loginPage"
import NotFoundPage from "./components/notfoundPage"
import PackagesPage from "./components/packagesPage"
import PrivacyPage from "./components/policies/privacyPage"
import TermsPage from "./components/policies/termsPage"
import RegisterPage from "./components/registerPage"
import UploadPage from "./components/uploadPage"

const  App = () => {
  return (
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route index element={<HomePage />} />
          <Route path="/packages" element={<PackagesPage />} />
          <Route path="/upload" element={<UploadPage />} />
          <Route path="/downloads" element={<DownloadsPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/confirmemail" element={<ConfirmEmailPage />} />
          <Route path="/forgotpassword" element={<ForgotPasswordPage />} />

          <Route path="/policies/terms" element={<TermsPage/>} />
          <Route path="/policies/privacy" element={<PrivacyPage/>} />

          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
  )
}

export default App
