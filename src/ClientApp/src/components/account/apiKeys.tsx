import { useState } from "react";
import PageContainer from "../pageContainer";
import NewApiKey from "./apiKeys/newApiKey";
const APIKeysPage = () => {
  const [showNewKey, setShowNewKey] = useState(false);

  return (
    <PageContainer>
      <h3>API Keys</h3>
      <p className="mt-2">
        An API key is a token that can identify you to DPM Gallery. The dpm command-line utility allows you to submit a package to the gallery using
        your API key to authenticate.
      </p>
      <p className="mt-2">
        Always keep your API keys a secret! If one of your keys is accidentally revealed, you can always generate a new one at any time. You can also
        remove existing API keys if necessary.
      </p>
      <div className="mt-2">
        <button className="btn btn-primary" onClick={() => setShowNewKey(!showNewKey)}>
          Create New Key
        </button>
        <NewApiKey hidden={showNewKey} />
      </div>
    </PageContainer>
  );
};

export default APIKeysPage;
