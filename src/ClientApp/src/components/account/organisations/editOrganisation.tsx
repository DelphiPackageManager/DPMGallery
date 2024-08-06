import PageContainer from "@/components/pageContainer";
import { Button } from "@/components/ui/button";
import { Checkbox, CheckedState } from "@/components/ui/checkbox";
import { Form, FormControl, FormDescription, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import useAuth from "@/hooks/useAuth";
import { UpdateEmailSchema, UpdateNotificationSchema } from "@/schemas";
import { EditOrganisationProps, OrganisationMember, UserOrganisation } from "@/types/organisations";
import { zodResolver } from "@hookform/resolvers/zod";
import { ChangeEvent, useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import EditOrganisationEmail from "./editOrganisationEmail";
import EditOrganisationMembers from "./editOrganisationMembers";
import EditOrganisationNotify from "./editOrganisationNotify";
import { UpdateOrganisationEmail, UpdateOrganisationSettings } from "./organisationApi";
import MemberRow from "./orgMemberRow";


const EditOrganisation = ({ organisation }: EditOrganisationProps) => {

	const [errors, setErrors] = useState<string[]>([]);




	if (errors.length > 0) {
		return <>
			{errors.map(function (error: string, index: number) {
				return (
					<div key={index}>
						{error}
					</div>
				)
			})}
		</>
	}




	return (
		<div>

			<div>
				<Tabs defaultValue="email" className="flex flex-col">
					<TabsList className="w-full items-start border-b border-primary bg-white text-base dark:bg-gray-800">
						<TabsTrigger value="email">Email</TabsTrigger>
						<TabsTrigger value="notify">Notifications</TabsTrigger>
						<TabsTrigger value="members">Members</TabsTrigger>
					</TabsList>
					<TabsContent value="email" className="text-sm">
						<EditOrganisationEmail organisation={organisation} setErrors={setErrors} />
					</TabsContent>
					<TabsContent value="notify">
						<EditOrganisationNotify organisation={organisation} setErrors={setErrors} />
					</TabsContent>
					<TabsContent value="members">
						<EditOrganisationMembers organisation={organisation} setErrors={setErrors} />
					</TabsContent>
				</Tabs>
			</div>

		</div >
	)

};

export default EditOrganisation;
