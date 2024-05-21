using lab10;
using System.ComponentModel;

public class CustomBindingList<T> : BindingList<T>
{
    private bool isSorted;
    private ListSortDirection sortDirection;
    private PropertyDescriptor sortProperty;
    private List<T> originalList;

    public CustomBindingList() : base()
    {
        originalList = new List<T>();
    }

    public CustomBindingList(IList<T> list) : base(list)
    {
        originalList = list.ToList();
    }

    protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
    {
        var items = this.Items as List<T>;

        if (items == null) return;

        Type propertyType = prop.PropertyType;

        if (propertyType.GetInterface("IComparable") != null)
        {
            items.Sort((x, y) =>
            {
                var xValue = prop.GetValue(x) as IComparable;
                var yValue = prop.GetValue(y) as IComparable;
                return direction == ListSortDirection.Ascending ? xValue.CompareTo(yValue) : yValue.CompareTo(xValue);
            });

            isSorted = true;
            sortDirection = direction;
            sortProperty = prop;
        }
        //else if (prop.Name == "Motor")
        //{
        //    items.Sort((x, y) =>
        //    {
        //        var xEngine = prop.GetValue(x) as Engine;
        //        var yEngine = prop.GetValue(y) as Engine;
        //        return direction == ListSortDirection.Ascending ? xEngine.HorsePower.CompareTo(yEngine.HorsePower) : yEngine.HorsePower.CompareTo(xEngine.HorsePower);
        //    });
        //}
        else
        {
            isSorted = false;
        }

        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }



    protected override void RemoveSortCore()
    {
        isSorted = false;
    }

    protected override bool SupportsSearchingCore => true;

    protected override int FindCore(PropertyDescriptor prop, object key)
    {
        for (int i = 0; i < Count; i++)
        {
            if (prop.GetValue(this[i]).Equals(key))
                return i;
        }
        return -1;
    }

    public void Sort(string propertyName, ListSortDirection direction)
    {
        PropertyDescriptor prop = TypeDescriptor.GetProperties(typeof(T)).Find(propertyName, true);
        ApplySortCore(prop, direction);
    }

    private Predicate<T> filter;
    public void Filter(Predicate<T> filter)
    {
        this.filter = filter;
        var filteredList = Items.Where(x => filter(x)).ToList();
        ClearItems();
        foreach (var item in filteredList)
        {
            Add(item);
        }
    }

    public void ResetFilter()
    {
        var items = this.Items as List<T>;
        if (items == null) return;

        this.RaiseListChangedEvents = false;

        items.Clear();
        items.AddRange(originalList);

        this.RaiseListChangedEvents = true;
        this.ResetBindings();
    }

}
