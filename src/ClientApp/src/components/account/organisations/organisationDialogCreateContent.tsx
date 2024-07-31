import { DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form } from "@/components/ui/form";
import { Constants } from "@/types/constants";
import { UserOrganisationCreateModel } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";


export default function OrganisationCreateDialogContent(props: { id: number, onSuccess: (data: UserOrganisationCreateModel) => void }) {

	const { onSuccess, id } = props;

	const formSchema = z.object({
		name: z.string().trim().min(2).max(Constants.FieldLength.Medium).regex(Constants.RegExPatterns.UserName, { message: "Name must be alphanumeric and may contain hyphens, dots, ampersands and underscores" }),

	})


	const form = useForm<z.infer<typeof formSchema>>({
		resolver: zodResolver(formSchema),
		defaultValues: {
			name: "",
		},
		mode: "onChange"
	})


	useEffect(() => {
		form.reset();
	}, [id]);


	const title = "Create new Organisation";

	return (
		<DialogContent>
			<DialogHeader>
				<DialogTitle>{title}</DialogTitle>
			</DialogHeader>
			<div>
				<Form {...form}>
					<div>


					</div>
				</Form>
			</div>
		</DialogContent>


	)
}