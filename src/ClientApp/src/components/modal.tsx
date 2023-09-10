import React, { ReactNode } from "react";

interface ModalType {
  children?: ReactNode;
  isOpen: boolean;
  title: string;
}

export default function Modal(props: ModalType) {
  return (
    <>
      {props.isOpen && (
        <div className="fixed w-[100vw] h-[100vh]  z-40 top-0 left-0 flex  bg-black/70">
          <div className="fixed top-0 left-0 z-50 flex justify-center items-center  w-full h-full overflow-x-hidden overflow-y-auto outline-none">
            <div
              onClick={(e) => e.stopPropagation()}
              className="relative flex flex-col min-w-[30%] min-h-[40%]  p-4 rounded-md bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100">
              <h3 className="w-full text-left px-2 border-b border-gray-200 dark:border-gray-500">{props.title}</h3>

              {props.children}
            </div>
          </div>
        </div>
      )}
    </>
  );
}
