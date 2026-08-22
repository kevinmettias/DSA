namespace DSAExperimentation;

public sealed class ContiguousGroupBuffer<TItem, TKey>(EqualityComparer<TKey> keyComparer)
    where TKey : notnull
{
    private const string MissingCurrentKeyMessage = "No current group key is available.";

    private readonly List<TItem> _currentGroup = [];

    private TKey? _currentKey;
    private bool _hasCurrentKey;

    public ContiguousGroupBuffer()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    public void Add(
        TItem item,
        TKey key,
        Action<IReadOnlyList<TItem>, TKey> visitGroup)
    {
        if (_hasCurrentKey && !keyComparer.Equals(GetCurrentKey(), key))
        {
            Flush(visitGroup);
        }

        _currentKey = key;
        _hasCurrentKey = true;
        _currentGroup.Add(item);
    }

    public void Flush(Action<IReadOnlyList<TItem>, TKey> visitGroup)
    {
        if (!_hasCurrentKey || _currentGroup.Count == 0)
        {
            return;
        }

        visitGroup([.. _currentGroup], GetCurrentKey());
        _currentGroup.Clear();
        _hasCurrentKey = false;
    }

    public void Reset()
    {
        _currentGroup.Clear();
        _currentKey = default;
        _hasCurrentKey = false;
    }

    private TKey GetCurrentKey()
        => _currentKey is null
            ? throw new InvalidOperationException(MissingCurrentKeyMessage)
            : _currentKey;
}
