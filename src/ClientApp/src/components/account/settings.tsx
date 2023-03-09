import { Tab } from "@headlessui/react";
import PageContainer from "../pageContainer";

const UserSettingsPage = () => {
  const lightTabClasses = "ui-selected:bg-primary ui-selected:text-white ui-not-selected:bg-gray-100  ui-not-selected:text-gray-800 ";
  const darkTabClasses =
    "dark:ui-selected:bg-primary-800 ui-selected:text-white dark:ui-not-selected:bg-gray-800  dark:ui-not-selected:text-gray-200";
  const tabClasses = "px-3 py-2 outline-0 " + lightTabClasses + darkTabClasses;

  return (
    <PageContainer>
      <h3>User Settings</h3>
      <div className="mt-4">
        <Tab.Group>
          <Tab.List className=" border-t border-l border-r border-gray-200 dark:border-gray-700 text-base bg-gray-100 dark:bg-gray-800 ">
            <Tab className={tabClasses}>Email Address</Tab>
            <Tab className={tabClasses}>External Logins</Tab>
            <Tab className={tabClasses}>2 Factor Authentication</Tab>
          </Tab.List>
          <Tab.Panels className="p-2 border border-gray-200 dark:border-gray-700 text-sm text-gray-800 dark:text-gray-50">
            <Tab.Panel>
              <div className="py-2">Change Email Address</div>
            </Tab.Panel>
            <Tab.Panel>
              <div className="py-2">Manange external logins</div>
            </Tab.Panel>
            <Tab.Panel>
              <div className="py-2">
                <h2>Manage 2 Factor Authentication</h2>
              </div>
            </Tab.Panel>
          </Tab.Panels>
        </Tab.Group>
      </div>
    </PageContainer>
  );
};

export default UserSettingsPage;
