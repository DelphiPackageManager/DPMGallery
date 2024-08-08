import * as React from "react"

import { cn } from "@/lib/utils"

export interface InputProps
	extends React.InputHTMLAttributes<HTMLInputElement> { }

const Input = React.forwardRef<HTMLInputElement, InputProps>(
	({ className, type, ...props }, ref) => {
		return (
			<input
				type={type}
				className={cn(
					"focus:outline-none dark:focus:outline-none focus:border-primary dark:focus:border-primary  block rounded-lg border  p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400  sm:text-sm",
					className
				)}
				ref={ref}
				{...props}
			/>
		)
	}
)
Input.displayName = "Input"

export { Input }

