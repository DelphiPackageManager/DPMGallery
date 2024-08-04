import { checkEmailUnique, checkOrgNameUnique } from "@/components/account/organisations/organisationApi";
import { Constants } from "@/types/constants";
import AwesomeDebouncePromise from "awesome-debounce-promise";
import { z } from "zod";

// NOTE : putting schema's in same file so it's easier to learn from

export const checkUniqueUserDebounced =
	AwesomeDebouncePromise(
		checkOrgNameUnique,
		500, {
		leading: true
	});

export const checkUniqueEmailDebounced =
	AwesomeDebouncePromise(
		checkEmailUnique,
		500, {
		leading: true
	});



export const CreateOrganisationSchema = z.object({
	name: z.string().trim().min(4).max(Constants.FieldLength.Medium).regex(Constants.RegExPatterns.UserName, { message: "Org Name must be alphanumeric and may contain hyphens and underscores" })
		.refine(async (value: string) => {
			if (!value)
				return false;
			let result = await checkUniqueUserDebounced(value);
			if (result) {
				return !result.data
			} else
				return true;
		}, "Organisation name must be unique"),
	email: z.string().email().min(6).refine(async (value: string) => {
		if (!value)
			return false;
		let result = await checkUniqueEmailDebounced(value);
		if (result) {
			return !result.data
		} else
			return true;
	}, "Email is in use by another account or org"),
})


export const UpdateEmailSchema = z.object({
	email: z.string().email().min(6).refine(async (value: string) => {
		if (!value)
			return false;
		let result = await checkUniqueEmailDebounced(value);
		if (result) {
			return !result.data
		} else
			return true;
	}, "Email is in use by another account or org"),
})
