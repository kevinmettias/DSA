using DSAExperimentation.Algorithms.HashMap;

namespace DSAExperimentation.Algorithms.Set;

// Backed by HashMap<T,bool> the same way java.util.HashSet is backed by
// HashMap<E,Object> - membership is exactly "is this key present," so the value is
// an unused placeholder.
internal sealed class Set<T>
{
    private readonly HashMap<T, bool> _items;

    public Set()
        : this(EqualityComparer<T>.Default)
    {
    }

    public Set(IEqualityComparer<T> comparer)
    {
        _items = new HashMap<T, bool>(comparer);
    }

    public int Count => _items.Count;

    public bool Contains(T item) => _items.ContainsKey(item);

    public bool Add(T item)
    {
        if (_items.ContainsKey(item))
        {
            return false;
        }

        _items.Set(item, true);
        return true;
    }

    public bool Remove(T item) => _items.Remove(item);
}
