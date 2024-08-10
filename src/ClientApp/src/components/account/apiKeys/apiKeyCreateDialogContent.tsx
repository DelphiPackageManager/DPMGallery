import ErrorDisplayCard from "@/components/errorDisplayDialog";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox, CheckedState } from "@/components/ui/checkbox";
import { DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import useAuth from "@/hooks/useAuth";
import { ApiKey, ApiKeyCreateModel, ApiKeyScope } from "@/types/apiKeys";
import { Constants } from "@/types/constants";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect, useRef, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { fetchOrganisationNames } from "../organisations/organisationApi";
import { createApiKey, fetchPackageNames } from "./apiKeysApi";
import CheckList, { CheckListItem } from "./checkList";


const formSchema = z.object({
	name: z.string().trim().min(4).max(Constants.FieldLength.Long),
	expires: z.number().min(1).max(365),
	packageOwner: z.number(),
	globPattern: z.string().optional(),
	packageList: z.string().optional(),
	pushScope: z.string().min(1),
	unlistScope: z.nativeEnum(ApiKeyScope)
}).superRefine((values, ctx) => {
	//console.log("values", values)
	if (!values.globPattern && !values.packageList) {
		ctx.addIssue({
			message: 'At least one of glob pattern or packages must be selected.',
			code: z.ZodIssueCode.custom,
			path: ['globPattern'],
		});
		ctx.addIssue({
			message: 'At least one of glob pattern or packages must be selected.',
			code: z.ZodIssueCode.custom,
			path: ['packageList'],
		});
		return false;
	}
	return true;
});

type OrgName = {
	id: number;
	name: string;
}

export default function ApiKeyCreateDialogContent(props: { id: number, onSuccess: (data: ApiKey) => void }) {
	const auth = useAuth();
	const { onSuccess, id } = props;
	const [errors, setErrors] = useState<string[]>([]);
	const [canPush, setCanPush] = useState(true);
	const [orgNames, setOrgNames] = useState<OrgName[]>([]);
	const [selectedPackages, setSelectedPackages] = useState<CheckListItem[]>([]);
	const [allPackages, setAllPackages] = useState<CheckListItem[]>([]);


	const form = useForm<z.infer<typeof formSchema>>({
		resolver: zodResolver(formSchema),
		defaultValues: {
			name: "",
			expires: 365,
			packageOwner: auth.currentUser?.id,
			pushScope: "3",
			unlistScope: ApiKeyScope.none,
			globPattern: "*",
			packageList: ""
		},
		mode: "onChange"
	})

	const { isValid } = form.formState;

	useEffect(() => {
		form.reset();
		let newSelected = allPackages.map((x: CheckListItem) => {
			return {
				name: x.name,
				checked: false
			}
		});
		setSelectedPackages(newSelected);
	}, [id]);



	useEffect(() => {
		const fetchOrgNames = async () => {
			const result = await fetchOrganisationNames();
			if (result.succeeded) {
				var names = [{ id: auth.currentUser?.id, name: auth.currentUser?.userName }, ...result.data]
				setOrgNames(names);
			}
			else {
				setErrors(result.errors);
			}
		}
		const fetchPackages = async () => {
			const result = await fetchPackageNames();
			if (result.succeeded) {
				var packageList = result.data?.map((x: string) => {
					return {
						name: x,
						checked: false
					}
				});

				setAllPackages(packageList);
				let newSelected = packageList.map((x: CheckListItem) => {
					return {
						name: x.name,
						checked: false
					}
				});
				setSelectedPackages(newSelected);
			}
			else {
				setErrors(result.errors);
			}
		}
		fetchOrgNames();
		fetchPackages();
	}, [])

	const onPackageSelectionChange = (selectedId: number, checked: boolean) => {
		let newSelected = selectedPackages.map((x: CheckListItem) => {
			return {
				name: x.name,
				checked: x.checked
			}
		});
		newSelected[selectedId].checked = checked;
		setSelectedPackages(newSelected);
		let packageList = newSelected.filter(x => x.checked).map(x => x.name).toString();
		//console.log("form.packageList", packageList);
		form.setValue("packageList", packageList, { shouldTouch: true });
		form.trigger("packageList");
		//console.log("isValid", isValid)
	};

	async function onSubmit(values: z.infer<typeof formSchema>) {

		let pushScope: ApiKeyScope = parseInt(values.pushScope);
		let scopes = pushScope | values.unlistScope;
		let packageList = selectedPackages.filter(x => x.checked).map(x => x.name);
		// ✅ Values are validated here.
		let apiKeyCreateModel: ApiKeyCreateModel = {
			name: values.name,
			expiresInDays: values.expires,
			packageOwner: values.packageOwner,
			globPattern: values.globPattern ?? "",
			packages: packageList.toString(),
			scopes: scopes
		};

		//console.log(apiKeyCreateModel);
		const result = await createApiKey(apiKeyCreateModel);
		if (result.succeeded)
			onSuccess(result.data);
		else
			setErrors(result.errors);

	}

	const errorDescription = "Error occurred while creating a new API key:";
	function clearErrors() {
		setErrors([]);
	}

	// timeout reference to be used in onChange event of the username field
	// to trigger the validation after 1 second of inactivity
	const timeOutRef = useRef<ReturnType<typeof setTimeout>>();

	return (
		<DialogContent className="max-w-fit">
			<DialogHeader>
				<DialogTitle>Create API key</DialogTitle>
				<DialogDescription>Enter details of new API key and click Save.</DialogDescription>
			</DialogHeader>
			<div className="rounded-md border border-gray-100 p-4 pt-1 text-base font-normal dark:border-gray-900">
				<Form {...form}>
					<form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col" autoComplete="off">
						<div className="grid grid-cols-2 gap-2">
							<div>
								<FormField control={form.control} name="name" render={({ field }) => (
									<FormItem className="">
										<FormLabel >Name</FormLabel>
										<FormControl>
											<Input type="text" id="name" size={50} {...field} autoFocus />
										</FormControl>
										<FormDescription />
										<FormMessage />
									</FormItem>
								)} />
							</div>
							<div>
								<FormField control={form.control} name="expires" render={({ field }) => (
									<FormItem>
										<FormLabel >Expires in</FormLabel>
										<FormControl>
											<Select name="expires" onValueChange={(value) => field.onChange(parseInt(value))} defaultValue={field.value.toString()}>
												<SelectTrigger className="w-40">
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

								)} />

							</div>
							<div>
								<FormField control={form.control} name="packageOwner" render={({ field }) => (
									<FormItem className="">
										<FormLabel>Package Owner</FormLabel>
										<FormControl>
											<div className="my-2 flex flex-row gap-2">
												<div className="">
													<Select name="packageOwner" onValueChange={field.onChange} value={field.value.toString()}>
														<SelectTrigger className="w-60">
															<SelectValue placeholder="Select Package Owner" />
														</SelectTrigger>
														<SelectContent>
															{orgNames.map((x: OrgName, index) => {
																return (
																	<SelectItem key={index} value={x.id.toString()}>{x.name}</SelectItem>
																)
															})}
														</SelectContent>

													</Select>
												</div>
											</div>
										</FormControl>
									</FormItem>
								)} />

							</div>
							<div>
								<div className="flex flex-col items-start">
									<Label className="">Scopes</Label>
									<div className="flex items-center space-x-2">
										<Checkbox id="push" checked={canPush}
											onCheckedChange={(e) => {
												setCanPush(e == true);
												form.setValue("pushScope", "1", { shouldTouch: true })
											}} />
										<Label htmlFor="push" size="sm" variant="form_checkbox">Push</Label>
									</div>

									<FormField control={form.control} name="pushScope" render={({ field }) => (
										<FormItem>
											<div className="ml-2">

												<RadioGroup disabled={!canPush} onValueChange={field.onChange} value={field.value} >
													<FormControl>
														<div className="flex items-center space-x-2">
															<RadioGroupItem value="3" id="pushNewPackage" />
															<Label htmlFor="pushNewPackage" variant="form_checkbox">Push new packages and package versions</Label>
														</div>
													</FormControl>
													<FormControl>
														<div className="flex items-center space-x-2">
															<RadioGroupItem value="1" id="pushPackageVersion" />
															<Label htmlFor="pushPackageVersion" variant="form_checkbox">Push new package versions</Label>
														</div>
													</FormControl>
												</RadioGroup>
											</div>
										</FormItem>

									)} />
									<FormField control={form.control} name="unlistScope" render={({ field }) => (
										<FormItem>
											<FormControl>
												<div className="flex items-center space-x-2">
													<Checkbox id="unlist" className="" checked={(field.value && ApiKeyScope.unlistPackage) == ApiKeyScope.unlistPackage}
														onCheckedChange={(e: CheckedState) => {
															let value = field.value;
															if (e == true) {
																value = ApiKeyScope.unlistPackage;
															} else {
																value = ApiKeyScope.none;
															}
															field.onChange(value);
														}} />
													<Label htmlFor="unlist" variant="form_checkbox" >
														Unlist or relist package versions
													</Label>
												</div>
											</FormControl>
										</FormItem>
									)} />
								</div>

							</div>

						</div>
						<Label className="text-base">Select Packages</Label>
						<div className="flex gap-2">
							<div className="flex flex-col gap-1">
								<FormField control={form.control} name="globPattern" render={({ field }) => (
									<FormItem className="mt-0">
										<FormLabel >Glob Pattern</FormLabel>
										<FormMessage />
										<FormControl>
											<Input type="text" id="name" size={50} {...field} />
										</FormControl>
									</FormItem>

								)} />
								<div className="max-w-full border border-gray-200 p-2 dark:border-gray-700">
									<CheckList height="h-56" items={selectedPackages} onItemChanged={onPackageSelectionChange} />
								</div>
							</div>
							<Card className="mt-4" >
								<CardContent className="max-w-[470px] pt-2 text-muted-foreground">
									<p>A glob pattern allows you to replace any sequence of characters with '*'.</p>
									<p className="mt-1">Example glob patterns:</p>
									<div className="grid grid-cols-2 gap-2">
										<div className="font-medium">Pattern</div>
										<div className="font-medium">Result</div>
										<div >*</div>
										<div >Select all Packages</div>
										<div >VSoft.*</div>
										<div >Select any package that has an ID beginning with <span className="italic">VSoft</span></div>
									</div>
								</CardContent>
							</Card>

						</div>
						<DialogFooter className="pt-4 sm:justify-end">
							<ErrorDisplayCard errors={errors} errorDescription={errorDescription} clearErrors={clearErrors} >
							</ErrorDisplayCard>
							<Button size="default" variant="default" disabled={!isValid} type="submit" className="mr-2">Save</Button>
						</DialogFooter>
					</form>

				</Form >

			</div >

		</DialogContent >

	);


}