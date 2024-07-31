
import ErrorDisplayCard from "@/components/errorDisplayDialog";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { ApiKey, ApiKeyRegenerateModel } from "@/types/apiKeys";
import { expiryDays } from "@/utils/dateUtils";
import { closestTo } from "@/utils/numberUtils";
import { zodResolver } from "@hookform/resolvers/zod";
import { format } from 'date-fns';
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { regenerateApiKey } from "./apiKeysApi";

const formSchema = z.object({
	expires: z.number().min(1).max(365),
	changeExpiry: z.boolean()
})


export default function ApiKeyRegenerateDialogContent(props: { id: number, apiKey: ApiKey, onSuccess: (data: ApiKey) => void }) {
	const { onSuccess, id, apiKey } = props;
	const [errors, setErrors] = useState([] as string[]);

	const expiresUtc = new Date(apiKey.expiresUTC);
	const expiresInDays = expiryDays(expiresUtc);
	const closestExpiryInDays = closestTo(expiresInDays, [1, 90, 180, 270, 365]);

	let expiryText = expiresInDays < 0
		? `Key expired on ${format(expiresUtc, "Pp")}`
		: `Key is currently set to expire on ${format(expiresUtc, "Pp")}`;

	let expiryText2;
	if (expiresInDays === 1)
		expiryText2 = "(tomorrow)";
	else if (expiresInDays === 0)
		expiryText2 = "(today)";
	else if (expiresInDays > 0)
		expiryText2 = `(${expiresInDays} days from now)`;
	else if (expiresInDays === -1)
		expiryText2 = "(yesterday)";
	else if (expiresInDays < 0)
		expiryText2 = `(${-expiresInDays} days ago)`;

	const form = useForm<z.infer<typeof formSchema>>({
		resolver: zodResolver(formSchema),
		defaultValues: {
			expires: closestExpiryInDays,
			changeExpiry: false
		},
		mode: "onChange"
	})

	useEffect(() => {
		form.reset();
	}, [id]);


	async function onSubmit(values: z.infer<typeof formSchema>) {

		// ✅ Values are validated here.
		const apiKeyId = apiKey.id;
		if (!apiKeyId)
			return;

		let apiKeyRegenerateModel: ApiKeyRegenerateModel = {
			expiresInDays: values.expires,
			id: apiKeyId,
			changeExpiry: values.changeExpiry,
		};

		const result = await regenerateApiKey(apiKeyRegenerateModel);
		if (result.succeeded)
			onSuccess(result.data);
		else
			setErrors(result.errors);

	}

	const errorDescription = errors.length > 1 ? "Error occurred while creating a new API key:" : "An error occurred while creating a new API key:";
	function clearErrors() {
		setErrors([]);
	}

	return (

		<DialogContent>
			<DialogHeader>
				<DialogTitle>Regenerate API Key</DialogTitle>
				<DialogDescription>Optionally change the expiry date and click Regenerate to generate a new key with the name "{apiKey.name}".</DialogDescription>
			</DialogHeader>
			<div className="m-2 mt-0 rounded-md border border-gray-100 p-4 pt-1 dark:border-gray-900">
				<Form {...form}>
					<form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col" autoComplete="off">
						<div className="flex flex-wrap space-x-1 text-sm"><div className="text-nowrap">{expiryText}</div><div className="text-nowrap">{expiryText2}</div></div>
						<FormField control={form.control} name="changeExpiry" render={({ field }) => (
							<FormItem controlType="checkbox" className="!mt-4">
								<FormControl>
									<Checkbox checked={field.value} onCheckedChange={field.onChange} />
								</FormControl>
								<FormLabel controlType="checkbox" >Change expiry date</FormLabel>
								<FormDescription />
								<FormMessage />
							</FormItem>
						)}
						/>
						{form.watch("changeExpiry") && (
							<FormField control={form.control} name="expires" render={({ field }) => (
								<FormItem>
									<FormLabel>Expires</FormLabel>
									<FormControl>
										<Select name="expires" onValueChange={(value) => field.onChange(parseInt(value))} defaultValue={field.value.toString()}>
											<SelectTrigger className="w-fit">
												<SelectValue placeholder="0" />
											</SelectTrigger>
											<SelectContent>
												<SelectItem value="1">1 Day</SelectItem>
												<SelectItem value="90">90 Days</SelectItem>
												<SelectItem value="180">180 Days</SelectItem>
												<SelectItem value="270">270 Days</SelectItem>
												<SelectItem value="365">365 Days</SelectItem>
											</SelectContent>
										</Select>
									</FormControl>
									<FormDescription />
									<FormMessage />
								</FormItem>
							)}
							/>)}
						<DialogFooter className="pt-4 sm:justify-start">
							<ErrorDisplayCard errors={errors} errorDescription={errorDescription} clearErrors={clearErrors} >
								<Button size="default" variant="default" type="submit" className="mr-2">Regenerate</Button>
							</ErrorDisplayCard>
						</DialogFooter>
					</form>

				</Form>

			</div>

		</DialogContent >

	);


}