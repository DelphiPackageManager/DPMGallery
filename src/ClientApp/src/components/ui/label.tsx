import * as LabelPrimitive from "@radix-ui/react-label";
import { cva, type VariantProps } from "class-variance-authority";
import * as React from "react";

import { cn } from "@/lib/utils";

const labelVariants = cva("text-base font-medium leading-none my-1 peer-disabled:cursor-not-allowed peer-disabled:opacity-70",
	{
		variants: {
			variant: {
				default: "",
				form: "mb-2 inline-block text-sm font-medium text-gray-900 dark:text-white",
				form_checkbox: "inline-block text-sm font-medium text-gray-900 dark:text-white mb-0 pb-0",
			},
			size: {
				default: "text-sm",
				sm: "text-sm",
				md: "text-md",
				lg: "text-lg",
			},
		},
		defaultVariants: {
			variant: "default",
			size: "default",
		},
	}
);

const Label = React.forwardRef<
	React.ElementRef<typeof LabelPrimitive.Root>,
	React.ComponentPropsWithoutRef<typeof LabelPrimitive.Root> &
	VariantProps<typeof labelVariants>
>(({ className, ...props }, ref) => (
	<LabelPrimitive.Root
		ref={ref}
		className={cn(labelVariants(), className)}
		{...props}
	/>
))
Label.displayName = LabelPrimitive.Root.displayName

export { Label };

