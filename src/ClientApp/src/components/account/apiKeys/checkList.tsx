import { Checkbox } from "@/components/ui/checkbox";

export type CheckListItem = {
	name: string;
	checked: boolean;
};
export type CheckListProps = {
	items: CheckListItem[];
	height: string;
	onItemChanged?: (selectedId: number, checked: boolean) => void;
};

const CheckList = (props: CheckListProps) => {
	const itemChanged = (index: number, value: boolean) => {
		if (props?.onItemChanged) {
			props.onItemChanged(index, value);
		}
	};

	const renderItems = props.items.map((item, index) => (
		<div className="flex items-center" key={index}>
			<Checkbox
				key={index}
				id={item.name}
				itemID={item.name}
				onCheckedChange={(e) => {
					let value = e !== "indeterminate" ? e : false;
					itemChanged(index, value);
				}}
				className="mr-2"
				checked={item.checked}></Checkbox>
			<label htmlFor={item.name} className="cursor-pointer">
				{item.name}
			</label>
		</div>
	));

	return <div className={`overflow-y-auto ${props.height}`}>{renderItems}</div>;
};

export default CheckList;
