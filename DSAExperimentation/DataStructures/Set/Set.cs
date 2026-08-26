using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.Set;

// Backed by HashMap<Element,bool> the same way java.util.HashSet is backed by
// HashMap<E,Object> - membership is exactly "is this key present," so the value is
// an unused placeholder.
internal sealed class Set<Element>
{
    private readonly HashMap<Element, bool> _items;

    public int Count => _items.Count;

    public Set()
        : this(EqualityComparer<Element>.Default)
    {
    }

    public Set(IEqualityComparer<Element> comparer) => _items = new HashMap<Element, bool>(comparer);

    public bool Has(Element item) => _items.HasKey(item);

    public bool TryAdd(Element item)
    {
        if (_items.HasKey(item))
        {
            return false;
        }

        _items.Set(item, true);
        return true;
    }

    public bool TryRemove(Element item) => _items.TryRemove(item);
}
