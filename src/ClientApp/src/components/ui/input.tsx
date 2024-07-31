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
					"focus:ring-primary-600 focus:border-primary-600 block rounded-lg border border-gray-300 p-2.5 text-gray-900 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500 sm:text-sm",
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
