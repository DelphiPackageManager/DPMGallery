import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import useAuth from "@/hooks/useAuth";
import { OrgName } from "@/types/organisations";
import { Tabs } from "@radix-ui/react-tabs";
import { PlusIcon } from "lucide-react";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import PageContainer from "../../pageContainer";
import PageHeader from "../../pageHeader";
import { TabsContent, TabsList, TabsTrigger } from "../../ui/tabs";
import { fetchOrganisationNames } from "../organisations/organisationApi";
import PublishedPackagesTable from "./publishedTable";

const MyPackagesPage = () => {
	const [currentTab, setCurrentTab] = useState("published");
	const [orgNames, setOrgNames] = useState<OrgName[]>([{ id: -1, name: "All Packages" }]);
	const [errors, setErrors] = useState<string[]>([]);
	const [filter, setFilter] = useState("-1");

	const auth = useAuth();


	useEffect(() => {
		const fetchOrgNames = async () => {
			const result = await fetchOrganisationNames();
			if (result.succeeded) {

				var names = [{ id: -1, name: "All Packages" }, { id: auth.currentUser?.id, name: auth.currentUser?.userName }, ...result.data]
				setOrgNames(names);
			}
			else {
				setErrors(result.errors);
			}
		}
		fetchOrgNames();
	}, [])
	return (
		<PageContainer>
			<PageHeader title="My Packages" />
			<div className="my-4 flex items-baseline justify-start gap-1" >
				<Label className="min-w-fit"><span>Filter by Owner :&nbsp;</span></Label>
				<Select value={filter} onValueChange={setFilter}  >
					<SelectTrigger className="min-w-56 max-w-fit">
						<SelectValue />
					</SelectTrigger>
					<SelectContent className="">
						{orgNames.map((x: OrgName, index) => {
							return (
								<SelectItem key={index} value={x.id.toString()}>{x.name}</SelectItem>
							)
						})}
					</SelectContent>
				</Select>
				<Link to="/account/packages/upload" >
					<Button variant="create" className="ml-2"><span className="mr-2">Add New Package</span> <PlusIcon /></Button>
				</Link>
			</div>
			<div>
				<PublishedPackagesTable filter={filter} currentUser={auth.currentUser?.userName || ""} />
			</div>
		</PageContainer>
	);
};

export default MyPackagesPage;
