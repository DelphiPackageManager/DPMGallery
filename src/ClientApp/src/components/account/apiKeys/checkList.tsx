import { Checkbox } from "@/components/ui/checkbox";

export type CheckListItem = {
  value: string;
  checked: boolean;
};
export type CheckListProps = {
  items: CheckListItem[];
  height: string;
  onItemChanged?: (selectedId: number, value: boolean) => void;
};

const CheckList = (props: CheckListProps) => {
  const itemChanged = (index: number, value: boolean) => {
    if (props?.onItemChanged) {
      props.onItemChanged(index, value);
    }
  };

  const renderItems = props.items.map((item, index) => (
    <div className="items-center flex" key={index}>
      <Checkbox
        key={index}
        id={item.value}
        itemID={item.value}
        onCheckedChange={(e) => {
          let value = e !== "indeterminate" ? e : false;
          itemChanged(index, value);
        }}
        className="mr-2"
        defaultValue={item.value}
        checked={props.items[index].checked}></Checkbox>
      <label htmlFor={item.value} className="cursor-pointer">
        {item.value}
      </label>
    </div>
  ));

  return <div className={`overflow-y-auto ${props.height}`}>{renderItems}</div>;
};

export default CheckList;
