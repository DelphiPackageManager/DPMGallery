import Modal from "@/components/ui/modal";
import { Spinner } from "@/components/ui/spinner";
import useAxiosPrivate from "@/hooks/useAxiosPrivate";
import * as Form from "@radix-ui/react-form";
import { FormEvent, useEffect, useState } from "react";

export type NewOrganisationProps = {
  afterSave(): void;
};

const CHECKUSEREXISTs_URL = "/ui/account/user-exists";

const NewOrganisation = ({ afterSave }: NewOrganisationProps) => {
  const [orgName, setOrgName] = useState("");
  const [debouncedOrgName, setDebouncedOrgName] = useState("");
  const [orgExists, setOrgExists] = useState(false);
  const [saving, setSaving] = useState(false);

  const axios = useAxiosPrivate();

  useEffect(() => {
    const delayInputTimeoutId = setTimeout(() => {
      setDebouncedOrgName(orgName);
    }, 500);
    return () => clearTimeout(delayInputTimeoutId);
  }, [orgName, 500]);

  async function checkUserExists(orgName: string) {
    if (orgName === "") return;
    try {
      let url = `${CHECKUSEREXISTs_URL + "/" + orgName}`;
      const response = await axios.get(url);
      if (response?.status == 200) {
        setOrgExists(true);
      }
    } catch (err: any) {
      setOrgExists(false);
    }
  }

  useEffect(() => {
    checkUserExists(debouncedOrgName);
  }, [debouncedOrgName]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);

    //let data = Object.fromEntries(new FormData(event.currentTarget));
    //await updateContact(contact.id, data);
    afterSave();
  }

  return (
    <Form.Root className="w-full" onSubmit={handleSubmit}>
      <div className="flex flex-col w-full h-full ">
        <fieldset disabled={saving} className="grow group flex flex-col w-full justify-between">
          <div className="grow">
            <Form.Field name="newOrg" className="flex flex-col mt-2">
              <div className="flex justify-between items-baseline max-w-sm">
                <Form.Label>Organisation Name</Form.Label>
                <Form.Message className="text-sm text-red-700 dark:text-orange-700" match="valueMissing">
                  Please enter the org name.
                </Form.Message>
                <Form.Message className="text-sm text-red-700 dark:text-orange-700" match="typeMismatch" forceMatch={orgExists}>
                  Organisation or user exists
                </Form.Message>
              </div>
              <Form.Control asChild required>
                <input
                  id="newOrg"
                  name="newOrg"
                  size={60}
                  type="text"
                  className="w-96"
                  value={orgName}
                  onChange={(e) => setOrgName(e.currentTarget.value)}
                />
              </Form.Control>
              <p className="text-muted-foreground text-sm mt-1">This will be your organization account on https://delphi.dev/profiles/{orgName}</p>
            </Form.Field>

            <Form.Field name="newEmail" className="flex flex-col mt-2">
              <div className="flex justify-between items-baseline max-w-sm">
                <Form.Label>New Email</Form.Label>
                <Form.Message className="text-sm text-red-700 dark:text-orange-700" match="typeMismatch">
                  Please provide a valid email address
                </Form.Message>
                <Form.Message className="text-sm text-red-700 dark:text-orange-700" match="valueMissing">
                  Please enter an email address
                </Form.Message>
              </div>
              <Form.Control asChild required>
                <input id="newEmail" name="newEmail" size={60} type="email" className="w-96"></input>
              </Form.Control>
            </Form.Field>
          </div>

          <div className="w-full mt-8 space-x-6 text-right">
            <Modal.Close className="rounded px-4 py-2 text-sm font-medium text-gray-500 hover:text-gray-600">Cancel</Modal.Close>
            <Form.Submit asChild>
              <button
                className="inline-flex items-center justify-center rounded bg-green-500 px-4 py-2 text-sm font-medium text-white hover:bg-green-600 group-disabled:pointer-events-none"
                type="submit">
                <Spinner className="absolute h-4 group-enabled:opacity-0" />
                <span className="group-disabled:opacity-0">Save</span>
              </button>
            </Form.Submit>
          </div>
        </fieldset>
      </div>
    </Form.Root>
  );
};

export default NewOrganisation;
