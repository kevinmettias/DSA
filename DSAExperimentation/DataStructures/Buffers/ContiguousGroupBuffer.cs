
namespace DSAExperimentation.DataStructures.Buffers;

/// <summary>
/// Gathers items fed one at a time into runs of consecutive items with equal keys, handing each finished run
/// to a callback.
/// </summary>
/// <param name="keyComparer">
/// Decides whether an item's key equals the current run's, and so whether the item extends it.
/// </param>
public sealed class ContiguousGroupBuffer<TItem, TKey>(EqualityComparer<TKey> keyComparer)
    where TKey : notnull
{
    private const string MissingCurrentKeyMessage = "No current group key is available.";

    private readonly ContiguousGroupBufferStorage<TItem, TKey> _storage = new();

    public ContiguousGroupBuffer()
        : this(EqualityComparer<TKey>.Default)
    {
    }

    /// <summary>
    /// Appends an item to the current run, first ending that run and handing it on if the item's key differs
    /// from its key.
    /// </summary>
    /// <param name="item">The item to append.</param>
    /// <param name="key">The item's key, compared with the current run's by the buffer's key comparer.</param>
    /// <param name="visitGroup">
    /// Receives a copy of the ended run's items and its key, when this item ends a run.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current run's key is null, against the notnull constraint.
    /// </exception>
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

    /// <summary>
    /// Ends the current run and hands it on, so the next item starts a new run even if its key is the same.
    /// </summary>
    /// <param name="visitGroup">Receives a copy of the run's items and its key; not called when no run is open.</param>
    /// <exception cref="InvalidOperationException">
    /// The current run's key is null, against the notnull constraint.
    /// </exception>
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

    /// <summary>Discards the current run and its key without handing them to any callback.</summary>
    public void Reset() => _storage.ResetAll();

    private TKey GetCurrentKey()
        => _storage.CurrentKey is null
            ? ThrowMissingCurrentKey()
            : _storage.CurrentKey;

    private static TKey ThrowMissingCurrentKey()
        => throw new InvalidOperationException(MissingCurrentKeyMessage);
}
