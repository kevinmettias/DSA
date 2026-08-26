namespace DSAExperimentation.DataStructures.Buffers;

// Raw storage only - no precondition checks, no throwing. ContiguousGroupBuffer owns
// the "no current key" guard and its exception; this type just holds the fields.
internal sealed class ContiguousGroupBufferStorage<TItem, TKey>
    where TKey : notnull
{
    private readonly List<TItem> _currentGroup = [];

    public bool HasCurrentKey { get; private set; }

    public TKey? CurrentKey { get; private set; }

    public int CurrentGroupCount => _currentGroup.Count;

    public void AppendToCurrentGroup(TItem item) => _currentGroup.Add(item);

    public IReadOnlyList<TItem> SnapshotCurrentGroup() => [.. _currentGroup];

    public void ClearCurrentGroup() => _currentGroup.Clear();

    public void SetCurrentKey(TKey key)
    {
        CurrentKey = key;
        HasCurrentKey = true;
    }

    // Deliberately clears only the flag, not CurrentKey itself - the stale key value
    // is left in place, matching the original bundled type's Flush behavior exactly.
    // Do not "complete" this by also clearing CurrentKey; nothing reads it while
    // HasCurrentKey is false, so doing so would only look more thorough.
    public void MarkCurrentKeyConsumed() => HasCurrentKey = false;

    public void ResetAll()
    {
        _currentGroup.Clear();
        CurrentKey = default;
        HasCurrentKey = false;
    }
}
