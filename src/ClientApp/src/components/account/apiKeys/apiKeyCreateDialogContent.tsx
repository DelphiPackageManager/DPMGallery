
import axios from "@/api/axios";
import ErrorDisplayCard from "@/components/errorDisplayDialog";
import { Button } from "@/components/ui/button";
import { Checkbox, CheckedState } from "@/components/ui/checkbox";
import { DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { ApiKey, ApiKeyCreateModel, ApiKeyScope } from "@/types/apiKeys";
import { Constants } from "@/types/constants";
import { UserOrganisation } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { ChangeEvent, useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { createApiKey } from "./apiKeysApi";


const formSchema = z.object({
	name: z.string().trim().min(2).max(Constants.FieldLength.Medium).regex(Constants.RegExPatterns.UserName, { message: "Name must be alphanumeric and may contain hyphens, dots, ampersands and underscores" }),
	expires: z.number().min(1).max(365),
	packageOwner: z.string(),
	globPattern: z.string(),
	packageList: z.string(),
	scopes: z.nativeEnum(ApiKeyScope)
})


export default function ApiKeyCreateDialogContent(props: { id: number, onSuccess: (data: ApiKey) => void }) {
	const { onSuccess, id } = props;
	const [errors, setErrors] = useState([] as string[]);
	const [canPush, setCanPush] = useState(true);
	const [pushScopes, setPushScopes] = useState<ApiKeyScope>(ApiKeyScope.none);

	const form = useForm<z.infer<typeof formSchema>>({
		resolver: zodResolver(formSchema),
		defaultValues: {
			name: "",
			expires: 365,
			//packageOwner : currentuser
		},
		mode: "onChange"
	})


	useEffect(() => {
		form.reset();
	}, [id]);


	async function onSubmit(values: z.infer<typeof formSchema>) {

		// ✅ Values are validated here.
		let apiKeyCreateModel: ApiKeyCreateModel = {
			name: values.name,
			expiresInDays: values.expires,
			packageOwner: values.packageOwner,
			globPattern: values.globPattern,
			packageList: values.packageList,
			scopes: values.scopes
		};

		const result = await createApiKey(apiKeyCreateModel);
		if (result.succeeded)
			onSuccess(result.data);
		else
			setErrors(result.errors);

	}

	const [changeEnabled, setChangeEnabled] = useState(false);

	function inputOnChange(event: ChangeEvent<HTMLInputElement> | CheckedState, field: any): void {

		field.onChange(event).then(() => {
			const dirtyFields = form.formState.dirtyFields;
			let isDirty = (dirtyFields.name ?? false) || (form.formState.dirtyFields.expires ?? false);
			setChangeEnabled(isDirty);
		});
	}

	const errorDescription = errors.length > 1 ? "Error occurred while creating a new API key:" : "An error occurred while creating a new API key:";
	function clearErrors() {
		setErrors([]);
	}

	// timeout reference to be used in onChange event of the username field
	// to trigger the validation after 1 second of inactivity
	const timeOutRef = useRef<ReturnType<typeof setTimeout>>();

	return (
		<DialogContent>
			<DialogHeader>
				<DialogTitle>Create API key</DialogTitle>
				<DialogDescription>Enter details of new API key and click Save.</DialogDescription>
			</DialogHeader>
			<div className="rounded-md border border-gray-100 p-4 pt-1 text-base font-normal dark:border-gray-900">
				<Form {...form}>
					<form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col" autoComplete="off">
						<FormField control={form.control} name="name" render={({ field }) => (
							<FormItem>
								<FormLabel >Name</FormLabel>
								<FormControl>
									<Input type="text" id="name" size={60} {...field} onChange={
										(event) => {
											inputOnChange(event, field);
										}
									} />
								</FormControl>
								<FormDescription />
								<FormMessage />
							</FormItem>

						)}
						/>

						<FormField control={form.control} name="packageOwner" render={({ field }) => (
							<FormItem>
								<FormLabel>Package Owner</FormLabel>
								<FormControl>
									<div className="mt-6 flex flex-row gap-2">
										<div className="">
											<Select name="packageOwner" onValueChange={(value) => field.onChange(value)} defaultValue={field.value}>


											</Select>
										</div>
									</div>
								</FormControl>
							</FormItem>
						)} />

						<FormField control={form.control} name="scopes" render={({ field }) => (
							<FormItem>
								<FormControl>
									<>
										<div className="align-baseline">
											<Checkbox id="push" checked={canPush}
												onCheckedChange={(e) => {
													setCanPush(e == true);
													if (!canPush) {
														var value = pushScopes;
														value &= ~ApiKeyScope.pushNewPackage;
														value &= ~ApiKeyScope.pushPackageVersion;
														setPushScopes(value);
														field.onChange(value);
													}
												}} />
											<Label htmlFor="push" size="sm" className="ml-2">Push</Label>
										</div>
										<div className="p-2">
											<RadioGroup defaultValue="comfortable" disabled={!canPush}>
												<div className="flex items-center space-x-2">
													<RadioGroupItem value="default" id="scopeNewAndVersion" checked={(pushScopes & ApiKeyScope.pushNewPackage) === ApiKeyScope.pushNewPackage}
														onChange={(e) => {
															setPushScopes((x) => x | ApiKeyScope.pushNewPackage | ApiKeyScope.pushPackageVersion);
															field.onChange(pushScopes);
														}} />
													<Label htmlFor="scopeNewAndVersion">Push new packages and package versions</Label>
												</div>
												<div className="flex items-center space-x-2">
													<RadioGroupItem value="default" id="scopeVersion" checked={(pushScopes & ApiKeyScope.pushPackageVersion) === ApiKeyScope.pushPackageVersion}
														onChange={(e) => {
															setPushScopes((x) => (x &= ~ApiKeyScope.pushNewPackage) | ApiKeyScope.pushPackageVersion);
															field.onChange(pushScopes);
														}} />
													<Label htmlFor="r2">Push new package versions</Label>
												</div>
											</RadioGroup>
										</div>
										<Checkbox id="unlist" onCheckedChange={(e) => {

										}} />
										<Label htmlFor="unlist">Unlist or relist package versions</Label>
									</>
								</FormControl>
							</FormItem>
						)} />

						<DialogFooter className="pt-4 sm:justify-end">
							<ErrorDisplayCard errors={errors} errorDescription={errorDescription} clearErrors={clearErrors} >
							</ErrorDisplayCard>
							<Button size="default" variant="default" disabled={!changeEnabled} type="submit" className="mr-2">Save</Button>
						</DialogFooter>
					</form>

				</Form >

			</div >

		</DialogContent >

	);


}