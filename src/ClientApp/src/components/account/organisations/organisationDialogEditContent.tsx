import { DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Form } from "@/components/ui/form";
import { Constants } from "@/types/constants";
import { UserOrganisation } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";


export default function OrganisationEditDialogContent(props: { id: number, editOrg: UserOrganisation, onSuccess: (data: UserOrganisation) => void }) {

	const { onSuccess, id, editOrg } = props;

	const formSchema = z.object({
		name: z.string().trim().min(2).max(Constants.FieldLength.Medium).regex(Constants.RegExPatterns.UserName, { message: "Name must be alphanumeric and may contain hyphens, dots, ampersands and underscores" }),

	})


	const form = useForm<z.infer<typeof formSchema>>({
		resolver: zodResolver(formSchema),
		defaultValues: {
			name: editOrg.name,
		},
		mode: "onChange"
	})


	useEffect(() => {
		form.reset();
	}, [id]);



	return (
		<DialogContent>
			<DialogHeader>
				<DialogTitle>Edit Organisation</DialogTitle>
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