namespace DSAExperimentation;

public sealed class ContiguousGroupBuffer<TItem, TKey>(EqualityComparer<TKey> keyComparer)
{
    private readonly List<TItem> currentGroup = [];

    private TKey? currentKey;
    private bool hasCurrentKey;

    public ContiguousGroupBuffer()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    public void Add(
        TItem item,
        TKey key,
        Action<IReadOnlyList<TItem>, TKey> visitGroup)
    {
        if (hasCurrentKey && !keyComparer.Equals(currentKey!, key))
        {
            Flush(visitGroup);
        }

        currentKey = key;
        hasCurrentKey = true;
        currentGroup.Add(item);
    }

    public void Flush(Action<IReadOnlyList<TItem>, TKey> visitGroup)
    {
        if (!hasCurrentKey || currentGroup.Count == 0)
        {
            return;
        }

        visitGroup([.. currentGroup], currentKey!);
        currentGroup.Clear();
        hasCurrentKey = false;
    }

    public void Reset()
    {
        currentGroup.Clear();
        currentKey = default;
        hasCurrentKey = false;
    }
}
