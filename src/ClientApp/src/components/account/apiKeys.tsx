import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import useAuth from "../../hooks/useAuth";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import useModal from "../../hooks/useModal";
import { ApiKey } from "../../types";
import Modal from "../modal";
import PageContainer from "../pageContainer";
import { Button } from "../ui/button";
import ApiKeyCard from "./apiKeys/apiKeyCard";
import NewApiKey from "./apiKeys/newApiKey";

const APIKeysPage = () => {
  const { isOpen, showModal, hideModal } = useModal();
  const [errMsg, setErrorMessage] = useState("");
  const { currentUser } = useAuth();
  const emailConfirmed = currentUser && currentUser.emailConfirmed;
  const [apiKeys, setApiKeys] = useState<ApiKey[]>([]);
  const axios = useAxiosPrivate();

  const fetchApiKeys = async () => {
    try {
      const response = await axios.get<ApiKey[]>("/ui/account/apikeys");
      if (response?.data) {
        setApiKeys(response.data);
      }
    } catch (err: any) {
      if (err?.response) {
        if (err.response.statusMessage) {
          setErrorMessage(err.response.statusMessage);
        } else {
          setErrorMessage("Error fetching external login details - Error :  " + err.response.status.toString());
        }
      }
    }
  };

  useEffect(() => {
    //
    if (emailConfirmed) {
      fetchApiKeys();
      //fetch apikeys from server
    }
  }, []);

  const onNewApiKey = async (apiKey: ApiKey) => {};

  const renderApiKeys = apiKeys.map((item, index) => {
    return <ApiKeyCard apiKey={item} key={index} />;
  });

  return (
    <div>
      <h3 className="mb-2">API Keys - not functional yet</h3>
      <h2 className={errMsg ? "" : "offscreen"} aria-live="assertive">
        {errMsg}
      </h2>
      {emailConfirmed && (
        <>
          <p className="mt-2">
            An API key is a token that can identify you to DPM Gallery. The dpm command-line utility allows you to submit a package to the gallery
            using your API key to authenticate.
          </p>
          <p className="mt-2">
            Always keep your API keys a secret! If one of your keys is accidentally revealed, you can always generate a new one at any time. You can
            also remove existing API keys if necessary.
          </p>
          <div className="mt-2">
            <Button onClick={() => showModal()}>Create New Key</Button>
          </div>
          <Modal isOpen={isOpen} title="New Api Key">
            <NewApiKey hide={hideModal} onNewApiKey={onNewApiKey} />
          </Modal>
          <div className="flex flex-col w-full">{renderApiKeys}</div>
        </>
      )}
      {!emailConfirmed && (
        <p>
          You must verify your{" "}
          <Link className="text-sky-600 dark:text-sky-500 underline" to="/account/email">
            email address
          </Link>{" "}
          before you can create Api Keys.
        </p>
      )}
    </div>
  );
};

export default APIKeysPage;
