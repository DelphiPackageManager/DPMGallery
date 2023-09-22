import * as Dialog from "@radix-ui/react-dialog";
import { ReactNode } from "react";

//This is just a simpler wrapper over radix ui dialog

export default function Modal({ open, onOpenChange, children }: { open?: boolean; onOpenChange?(open: boolean): void; children: ReactNode }) {
  return (
    <Dialog.Root open={open} onOpenChange={onOpenChange}>
      {children}
    </Dialog.Root>
  );
}

function ModalContent({ title, children }: { title: string; children: ReactNode }) {
  return (
    <Dialog.Portal className="fixed top-0 left-0 z-50 w-full h-full overflow-x-hidden">
      <Dialog.Overlay className="fixed inset-0 bg-black/50 data-[state=closed]:animate-[dialog-overlay-hide_200ms] data-[state=open]:animate-[dialog-overlay-show_200ms]" />
      <Dialog.Content className="fixed flex left-1/2 top-1/2 min-w-[30%] min-h-lg -translate-x-1/2 -translate-y-1/2 rounded-md bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 shadow data-[state=closed]:animate-[dialog-content-hide_200ms] data-[state=open]:animate-[dialog-content-show_200ms]">
        <div className="w-full flex flex-col px-6 py-4">
          <div className="flex items-center justify-between">
            <Dialog.Title className="text-xl">{title}</Dialog.Title>
            <Dialog.Close className="text-gray-400 hover:text-gray-500">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6">
                <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </Dialog.Close>
          </div>
          <div className="grow flex">{children}</div>
        </div>
      </Dialog.Content>
    </Dialog.Portal>
  );
}

Modal.Button = Dialog.Trigger;
Modal.Close = Dialog.Close;
Modal.Content = ModalContent;
