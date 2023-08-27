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
      //  props.items[index].checked = value;
      props.onItemChanged(index, value);
    }
  };

  const renderItems = props.items.map((item, index) => (
    <label htmlFor={item.value} className="flex flex-row">
      <input
        type="checkbox"
        key={index}
        id={item.value}
        itemID={item.value}
        onChange={(e) => itemChanged(index, e.target.checked)}
        className="mr-2"
        checked={props.items[index].checked}
      />
      {item.value}
    </label>
  ));

  return <div className={`overflow-y-auto ${props.height}`}>{renderItems}</div>;
};

export default CheckList;
