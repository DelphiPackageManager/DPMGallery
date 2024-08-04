import ErrorDisplayCard from "@/components/errorDisplayDialog";
import { Button } from "@/components/ui/button";
import { CheckedState } from "@/components/ui/checkbox";
import { DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { CreateOrganisationSchema } from "@/schemas";
import { UserOrganisation, UserOrganisationCreateModel } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { ChangeEvent, useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { createOrganisation } from "./organisationApi";


export default function OrganisationCreateDialogContent(props: { id: number, onSuccess: (data: UserOrganisation) => void }) {

	const { onSuccess, id } = props;
	const [errors, setErrors] = useState([] as string[]);




	const form = useForm<z.infer<typeof CreateOrganisationSchema>>({
		resolver: zodResolver(CreateOrganisationSchema),
		defaultValues: {
			name: "",
			email: ""
		},
		mode: "onChange"
	})



	useEffect(() => {
		form.reset();
	}, [id]);

	const [changeEnabled, setChangeEnabled] = useState(false);

	function inputOnChange(event: ChangeEvent<HTMLInputElement> | CheckedState, field: any): void {

		field.onChange(event).then(() => {
			const dirtyFields = form.formState.dirtyFields;
			let isDirty = (dirtyFields.name ?? false);
			setChangeEnabled(isDirty);
		});
	}


	async function onSubmit(values: z.infer<typeof CreateOrganisationSchema>) {

		// ✅ Values are validated here.
		let model: UserOrganisationCreateModel = {
			name: values.name,
			email: values.email
		}

		const result = await createOrganisation(model);
		if (result.succeeded) {
			form.reset();
			onSuccess(result.data);

		}
		else
			setErrors(result.errors);

	}


	const errorDescription = "Error occurred while creating a new Organisation:";
	function clearErrors() {
		setErrors([]);
	}

	const watchOrgName = form.watch("name");

	return (
		<DialogContent className="">
			<DialogHeader>
				<DialogTitle>Create new Organisation</DialogTitle>
			</DialogHeader>
			<div>
				<Form {...form}>
					<form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col space-y-4" autoComplete="off">
						< FormField control={form.control} name="name" render={({ field }) => (
							<FormItem>
								<div className="inline">
									<FormLabel>Name</FormLabel>
									<FormMessage className="float-end" />
								</div>
								<FormControl>
									<Input type="text" id="name" size={64} required {...field} onChange={(event) => inputOnChange(event, field)} />
								</FormControl>
								<FormDescription className="italic">This will be your organization account on <br /> https://delphi.dev/profiles/{watchOrgName}</FormDescription>

							</FormItem>
						)
						}
						/>
						< FormField control={form.control} name="email" render={({ field }) => (
							<FormItem>
								<div className="inline">
									<FormLabel>Email</FormLabel>
									<FormMessage className="float-end" />
								</div>
								<FormControl>
									<Input type="text" id="email" placeholder="you@domain.com" required size={64} {...field} onChange={(event) => inputOnChange(event, field)} />
								</FormControl>
								<FormDescription className="italic">Users can contact your organization at this email address.</FormDescription>

							</FormItem>

						)}
						/>


						< DialogFooter className="pt-4 sm:justify-end" >
							<ErrorDisplayCard errors={errors} errorDescription={errorDescription} clearErrors={clearErrors} >
							</ErrorDisplayCard>
							<Button size="default" variant="default" disabled={!changeEnabled} type="submit" className="mr-2">Save</Button>

						</DialogFooter >

					</form >
				</Form >
			</div >
		</DialogContent >


	)
}