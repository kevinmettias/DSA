using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.Set;

// Backed by HashMap<Element,bool> the same way java.util.HashSet is backed by
// HashMap<E,Object> - membership is exactly "is this key present," so the value is
// an unused placeholder.
/// <summary>
/// An unordered set of distinct elements, with average constant-time adds, removals and membership tests.
/// </summary>
public sealed class Set<Element>
{
    private readonly HashMap<Element, bool> _items;

    /// <summary>The number of elements in the set.</summary>
    public int Count => _items.Count;

    public Set()
        : this(EqualityComparer<Element>.Default)
    {
    }

    public Set(IEqualityComparer<Element> comparer) => _items = new HashMap<Element, bool>(comparer);

    // Bulk-seed from an existing sequence, the way a caller holding a plain list
    // of candidates gets a membership set without writing the loop themselves.
    public Set(IEnumerable<Element> items)
        : this()
    {
        foreach (var item in items)
        {
            TryAdd(item);
        }
    }

    /// <summary>Tests whether the set contains an element, judged by the set's equality comparer.</summary>
    /// <param name="item">The element to look for.</param>
    /// <returns>True when the set contains <paramref name="item"/>; false otherwise.</returns>
    public bool Has(Element item) => _items.HasKey(item);

    /// <summary>Adds an element unless the set already contains an equal one.</summary>
    /// <param name="item">The element to add.</param>
    /// <returns>True when the element was added; false when the set already contained it.</returns>
    public bool TryAdd(Element item)
    {
        if (_items.HasKey(item))
        {
            return false;
        }

        _items.Set(item, true);
        return true;
    }

    /// <summary>Removes an element if the set contains it.</summary>
    /// <param name="item">The element to remove.</param>
    /// <returns>True when the element was removed; false when the set did not contain it.</returns>
    public bool TryRemove(Element item) => _items.TryRemove(item);
}
