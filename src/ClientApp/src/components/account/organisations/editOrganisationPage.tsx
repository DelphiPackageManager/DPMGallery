import PageContainer from "@/components/pageContainer";
import PageHeader from "@/components/pageHeader";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { Checkbox, CheckedState } from "@/components/ui/checkbox";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import useAuth from "@/hooks/useAuth";
import { MemberRole, memberRoleToString, OrganisationMember, UserOrganisation } from "@/types/organisations";
import { ChangeEvent, useEffect, useState } from "react";
import { Link, useLocation, useNavigate, useParams } from "react-router-dom";
import { fetchOrganisationByName, UpdateOrganisationEmail } from "./organisationApi";

import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { UpdateEmailSchema } from "@/schemas";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import EditOrganisation from "./editOrganisation";
import MemberRow from "./orgMemberRow";





const EditOrganisationPage = () => {
	const { orgName } = useParams();
	const [currentOrg, setCurrentOrg] = useState<UserOrganisation | null>(null);
	const [errors, setErrors] = useState<string[]>([]);

	//	const location = useLocation();
	//const navigate = useNavigate();



	useEffect(() => {
		const fetchOrg = async () => {
			if (orgName) {
				const result = await fetchOrganisationByName(orgName);
				if (result.succeeded) {
					setCurrentOrg(result.data.orgModel);
				}
				else {
					setErrors(result.errors);
				}
			}
		}
		fetchOrg();
	}, [])



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
		<PageContainer>
			{!currentOrg &&
				<div>Loading...</div>
			}
			{currentOrg &&
				<EditOrganisation organisation={currentOrg} updateOrganisation={setCurrentOrg} />
			}
		</PageContainer >
	)


}

export default EditOrganisationPage;