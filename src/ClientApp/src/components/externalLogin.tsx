import PageContainer from "./pageContainer";

const ExternalLoginPage = () => {
  const returnUrl = "";

  return (
    <PageContainer>
      <h1>Create Account</h1>
      <h2 id="external-login-title" className="mt-2">
        Associate your @Model.ProviderDisplayName account.
      </h2>
      <hr />

      <p id="external-login-description" className="text-info mb-4 mt-4">
        You've successfully authenticated with <strong>@Model.ProviderDisplayName</strong>. Please enter an email address for this site below and
        click the Register button to finish logging in.
      </p>
      <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto lg:py-4 w-full ">
        <div className="bg-white rounded-lg shadow-md dark:shadow-none shadow-gray-200 dark:shadow-gray-800 border md:mt-0 sm:max-w-md xl:p-0 dark:bg-gray-800 dark:border-gray-700">
          <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
            <form method="post" className="flex flex-row space-y-4 md:space-y-6">
              <input hidden id="returnUrl" name="returnUrl" />
              <div className="flex flex-col mb-3 gap-3">
                <label htmlFor="userName" className="text-sm font-medium text-gray-800 dark:text-white">
                  UserName
                </label>

                <input
                  className="mb-2 border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  autoComplete="user"
                  id="userName"
                  name="userName"
                  placeholder="Please enter your username."
                />
                <label htmlFor="email" className=" text-sm font-medium text-gray-800 dark:text-white">
                  Email
                </label>
                <input
                  className="border border-gray-300 text-gray-900 sm:text-sm rounded-lg focus:ring-primary-600 focus:border-primary-600 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
                  id="email"
                  name="email"
                  autoComplete="email"
                  placeholder="Please enter your email."
                />
                <button type="submit" className="w-96 btn btn-lg btn-primary">
                  Register
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </PageContainer>
  );
};

export default ExternalLoginPage;
