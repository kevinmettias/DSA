using DSAExperimentation.DataStructures.Buffers;

namespace DSAExperimentation.Algorithms.Buffers;

internal sealed class ContiguousGroupBuffer<TItem, TKey>(EqualityComparer<TKey> keyComparer)
    where TKey : notnull
{
    private const string MissingCurrentKeyMessage = "No current group key is available.";

    private readonly ContiguousGroupBufferStorage<TItem, TKey> _storage = new();

    public ContiguousGroupBuffer()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    public void Add(
        TItem item,
        TKey key,
        Action<IReadOnlyList<TItem>, TKey> visitGroup)
    {
        if (_storage.HasCurrentKey && !keyComparer.Equals(GetCurrentKey(), key))
        {
            Flush(visitGroup);
        }

        _storage.SetCurrentKey(key);
        _storage.AppendToCurrentGroup(item);
    }

    public void Flush(Action<IReadOnlyList<TItem>, TKey> visitGroup)
    {
        if (!_storage.HasCurrentKey || _storage.CurrentGroupCount == 0)
        {
            return;
        }

        visitGroup(_storage.SnapshotCurrentGroup(), GetCurrentKey());
        _storage.ClearCurrentGroup();
        _storage.MarkCurrentKeyConsumed();
    }

    public void Reset() => _storage.ResetAll();

    private TKey GetCurrentKey()
        => _storage.CurrentKey is null
            ? ThrowMissingCurrentKey()
            : _storage.CurrentKey;

    private static TKey ThrowMissingCurrentKey()
        => throw new InvalidOperationException(MissingCurrentKeyMessage);
}
