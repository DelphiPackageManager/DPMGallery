import { useState } from "react";

export default function useModal() {
  const [isOpen, setisOpen] = useState(false);

  const showModal = () => {
    setisOpen(true);
  };

  const hideModal = () => {
    setisOpen(false);
  };

  return {
    isOpen,
    showModal,
    hideModal,
  };
}
