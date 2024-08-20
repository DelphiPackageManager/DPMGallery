import { Edit } from "lucide-react";

import { Link } from "react-router-dom";



export default function PackageRowActions(props: { packageId: string, version: string }) {

	return <Link to={`/packages/${props.packageId}/manage/${props.version}`}><Edit /></Link>
}