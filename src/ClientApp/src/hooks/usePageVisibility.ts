/*
 * This react hook tracks page visibility using browser page visibility api.
 * Reference: https://developer.mozilla.org/en-US/docs/Web/API/Page_Visibility_API
 *
 * Use: const pageVisibilityStatus = usePageVisibility();
 * Return type: boolean
 */

import { useEffect, useState } from "react";

export default function usePageVisibility() {
  const [isPageVisible, setVisibilityStatus] = useState(!document.hidden);

  useEffect(() => {
    function handleVisibilityChange() {
      setVisibilityStatus(!document.hidden);
    }
    const visibilityChange = "visibilitychange";

    document.addEventListener(visibilityChange, handleVisibilityChange);

    return () => {
      document.removeEventListener(visibilityChange, handleVisibilityChange);
    };
  }, []);

  return isPageVisible;
}
