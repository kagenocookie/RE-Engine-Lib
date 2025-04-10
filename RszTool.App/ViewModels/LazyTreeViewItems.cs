using System.Collections;

namespace RszTool.App.ViewModels
{
    public class LazyTreeViewItems<T> : IEnumerable<T>
    {
        public LazyTreeViewItems(Func<T[]> valueFactory)
        {
            ValueFactory = valueFactory;
        }

        public bool ValueCreated { get; private set; }
        public Func<T[]> ValueFactory { get; }
        public T[]? value;
        public T[] Value
        {
            get
            {
                if (ValueCreated)
                {
                    return value!;
                }
                ValueCreated = true;
                return value ??= ValueFactory();
            }
        }

        public void Refresh()
        {
            value = ValueFactory();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }
    }


    public class LazyTreeViewItems : LazyTreeViewItems<object>
    {
        public LazyTreeViewItems(Func<object[]> valueFactory) : base(valueFactory)
        {
        }
    }
}
