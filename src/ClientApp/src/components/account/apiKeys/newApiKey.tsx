import { useState } from "react";
import CheckList, { CheckListItem } from "./checkList";

export type NewApiKeyProps = {
  hidden: boolean;
};

let dummyItems: CheckListItem[] = [
  {
    value: "VSoft.Awaitable",
    checked: false,
  },
  {
    value: "Sprin4D.Persistence",
    checked: false,
  },
  {
    value: "Three",
    checked: false,
  },
  {
    value: "Four",
    checked: false,
  },
];

const NewApiKey = (props: NewApiKeyProps) => {
  const [keyName, setKeyName] = useState("");
  const [owner, setOwner] = useState("");
  const [canPush, setCanPush] = useState(false);
  const [packages, setPackages] = useState<CheckListItem[]>(dummyItems);
  const radioHandler = (event: React.ChangeEvent<HTMLInputElement>) => {
    //
  };

  const onPackageSelectionChange = (selectedId: number, value: boolean) => {
    let newItem: CheckListItem = {
      value: packages[selectedId].value,
      checked: value,
    };
    let newPackages = packages.map((item, index) => {
      return index == selectedId ? newItem : item;
    });
    setPackages(newPackages);
  };

  return (
    <form className="w-full my-4 p-4 border border-gray-500 dark:border-gray-700" hidden={props.hidden}>
      <div className="flex flex-row gap-2 items-center">
        <div className="flex flex-col">
          <div className="mr-2">
            <label htmlFor="newKeyName">Key Name</label>
          </div>
          <div className="">
            <input
              type="text"
              className="text-gray-900  bg-gray-200 dark:text-gray-50 dark:bg-gray-700 p-1 rounded"
              size={64}
              value={keyName}
              onChange={(e) => setKeyName(e.target.value)}
              id="newKeyName"
              name="newKeyName"></input>
          </div>
        </div>
        <div className="flex flex-col">
          <div className="mr-4">
            <label htmlFor="newKeyExpires">Expires</label>
          </div>
          <div className="">
            <select id="newKeyExpires" defaultValue={365} className="text-gray-900  bg-gray-200 dark:text-gray-50 dark:bg-gray-700 p-1 rounded">
              <option value={1}>1 day</option>
              <option value={90}>90 days</option>
              <option value={180}>180 day</option>
              <option value={270}>270 days</option>
              <option value={365}>365 days</option>
            </select>
          </div>
        </div>
      </div>
      <div className="flex flex-row gap-2 mt-6">
        <div className="flex flex-col">
          <div className="mr-4">
            <label htmlFor="newKeyPackageOwner">Package Owner</label>
          </div>
          <div className="">
            <select
              id="newKeyPackageOwner"
              size={1}
              defaultValue={""}
              placeholder="Select an owner"
              className="w-56 text-gray-900  bg-gray-200 dark:text-gray-50 dark:bg-gray-700 p-1 rounded"
              value={owner}
              onChange={(e) => setOwner(e.target.value)}>
              <option value={""}>Select an owner</option>
              <option value={"vincent"}>vincent</option>
              <option value={"vsoft"}>vsoft</option>
            </select>
          </div>
        </div>
      </div>
      <div className="mt-4">
        <h3>Select Scopes</h3>
        <div className="mt-2 flex flex-col items-start">
          <label htmlFor="canPush" className="flex flex-row">
            <input type="checkbox" id="canPush" name="canPush" className="mr-2" onChange={(e) => setCanPush(e.target?.checked)} />
            Push
          </label>
          <div className="mt-1 ml-6">
            <div className="flex items-center py-1">
              <input
                type="radio"
                id="scopeNewAndVersion"
                name="pushScope"
                className="mr-2 w-5"
                value="scopeNewAndVersion"
                checked
                disabled={!canPush}
              />
              <label htmlFor="scopeNewAndVersion" className="">
                Push new packages and package versions
              </label>
            </div>
            <div className="flex items-center py-1">
              <input type="radio" id="scopeVersion" name="pushScope" className="mr-2 w-5" value="scopeVersion" disabled={!canPush} />
              <label htmlFor="scopeVersion" className="">
                Push new packages and package versions
              </label>
            </div>
          </div>
          <label htmlFor="scopeUnlist" className="flex flex-row">
            <input type="checkbox" id="scopeUnlist" name="scopeUnlist" className="mr-2" />
            Unlist
          </label>
        </div>
      </div>
      <div className="flex flex-col mt-6">
        <h3>Select Packages</h3>
        <p>To select which packages to associate with a key, use a glob pattern, select individual packages, or both.</p>
        <label className="mt-2" htmlFor="globPattern">
          Glob Pattern
        </label>
        <div className="flex mt-2 items-start gap-6">
          <div className="flex-1">
            <input type="text" id="globPattern" />
            <h3 className="mt-2">Available Packages</h3>
            <div className="p-2 border border-gray-700">
              <CheckList height="h-40" items={packages} onItemChanged={onPackageSelectionChange} />
            </div>
          </div>
          <div className="p-3 border border-gray-700 flex-1">
            <p>A glob pattern allows you to replace any sequence of characters with '*'.</p>
            <p className="mt-2">Example glob patterns:</p>
          </div>
        </div>
      </div>
      <div className="flex flex-row items-center justify-around mt-4 ">
        <button className="btn btn-primary btn-small w-48">Create</button>
        <button className="btn btn-outline btn-small w-48">Cancel</button>
      </div>
    </form>
  );
};

export default NewApiKey;
