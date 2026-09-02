using DSAExperimentation.DataStructures.Buffers;

namespace DSAExperimentation.Tests.DataStructures.Buffers;

public sealed class ContiguousGroupBufferStorageTests
{
    private static ContiguousGroupBufferStorage<int, string> Storage() => new();

    [Fact]
    public void HasCurrentKey_NewStorage_IsFalse()
    {
        Assert.False(Storage().HasCurrentKey);
    }

    [Fact]
    public void CurrentGroupCount_NewStorage_IsZero()
    {
        Assert.Equal(0, Storage().CurrentGroupCount);
    }

    [Fact]
    public void AppendToCurrentGroup_RaisesTheGroupCount()
    {
        var storage = Storage();

        storage.AppendToCurrentGroup(1);
        storage.AppendToCurrentGroup(2);

        Assert.Equal(2, storage.CurrentGroupCount);
    }

    [Fact]
    public void SnapshotCurrentGroup_ReturnsTheItemsInInsertionOrder()
    {
        var storage = Storage();

        storage.AppendToCurrentGroup(1);
        storage.AppendToCurrentGroup(2);

        Assert.Equal([1, 2], storage.SnapshotCurrentGroup());
    }

    [Fact]
    public void SnapshotCurrentGroup_IsACopyUnaffectedByLaterAppends()
    {
        var storage = Storage();
        storage.AppendToCurrentGroup(1);

        var snapshot = storage.SnapshotCurrentGroup();
        storage.AppendToCurrentGroup(2);

        Assert.Single(snapshot);
    }

    [Fact]
    public void ClearCurrentGroup_EmptiesTheItemsButKeepsTheKey()
    {
        var storage = Storage();
        storage.SetCurrentKey("k");
        storage.AppendToCurrentGroup(1);

        storage.ClearCurrentGroup();

        Assert.Equal(0, storage.CurrentGroupCount);
        Assert.True(storage.HasCurrentKey);
        Assert.Equal("k", storage.CurrentKey);
    }

    [Fact]
    public void SetCurrentKey_RecordsTheKeyAndMarksItPresent()
    {
        var storage = Storage();

        storage.SetCurrentKey("k");

        Assert.True(storage.HasCurrentKey);
        Assert.Equal("k", storage.CurrentKey);
    }

    [Fact]
    public void MarkCurrentKeyConsumed_ClearsThePresenceFlagButNotTheKey()
    {
        var storage = Storage();
        storage.SetCurrentKey("k");

        storage.MarkCurrentKeyConsumed();

        Assert.False(storage.HasCurrentKey);
        Assert.Equal("k", storage.CurrentKey);
    }

    [Fact]
    public void ResetAll_ClearsTheItemsTheKeyAndThePresenceFlag()
    {
        var storage = Storage();
        storage.SetCurrentKey("k");
        storage.AppendToCurrentGroup(1);

        storage.ResetAll();

        Assert.Equal(0, storage.CurrentGroupCount);
        Assert.False(storage.HasCurrentKey);
        Assert.Null(storage.CurrentKey);
    }
}
