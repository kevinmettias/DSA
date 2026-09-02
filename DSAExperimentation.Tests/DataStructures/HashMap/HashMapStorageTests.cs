using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.DataStructures.HashMap;

public sealed class HashMapStorageTests
{
    private static HashMapStorage<string, int> Storage() => new();

    [Fact]
    public void Count_NewStorage_IsZero()
    {
        Assert.Equal(0, Storage().Count);
    }

    [Fact]
    public void Insert_IncrementsTheCount()
    {
        var storage = Storage();

        storage.Insert(1, "a", 10);
        storage.Insert(2, "b", 20);

        Assert.Equal(2, storage.Count);
    }

    [Fact]
    public void BucketIndexFor_MapsAHashCodeIntoTheBucketRange()
    {
        var storage = Storage();

        Assert.Equal(0, storage.BucketIndexFor(0));
        Assert.True(storage.BucketIndexFor(12_345) >= 0);
    }

    [Fact]
    public void BucketHead_EmptyBucket_IsMinusOne()
    {
        Assert.Equal(-1, Storage().BucketHead(0));
    }

    [Fact]
    public void BucketHead_AfterInsert_PointsAtTheNewEntry()
    {
        var storage = Storage();

        storage.Insert(0, "a", 10);

        var head = storage.BucketHead(storage.BucketIndexFor(0));

        Assert.True(head >= 0);
        Assert.Equal("a", storage.Entry(head).Key);
    }

    [Fact]
    public void Entry_ReturnsTheStoredHashCodeKeyAndValue()
    {
        var storage = Storage();

        storage.Insert(7, "a", 10);
        var entry = storage.Entry(storage.BucketHead(storage.BucketIndexFor(7)));

        Assert.Equal(7, entry.HashCode);
        Assert.Equal("a", entry.Key);
        Assert.Equal(10, entry.Value);
    }

    [Fact]
    public void SetEntryValue_ReplacesTheValueAndKeepsTheKey()
    {
        var storage = Storage();
        storage.Insert(7, "a", 10);
        var index = storage.BucketHead(storage.BucketIndexFor(7));

        storage.SetEntryValue(index, 99);

        Assert.Equal(99, storage.Entry(index).Value);
        Assert.Equal("a", storage.Entry(index).Key);
    }

    [Fact]
    public void SnapshotEntries_NewStorage_IsEmpty()
    {
        Assert.Empty(Storage().SnapshotEntries());
    }

    [Fact]
    public void SnapshotEntries_ReturnsEveryInsertedEntry()
    {
        var storage = Storage();

        for (var i = 0; i < 10; i++)
        {
            storage.Insert(i, $"k{i}", i);
        }

        Assert.Equal(10, storage.SnapshotEntries().Count);
        Assert.Equal(
            Enumerable.Range(0, 10).Select(i => $"k{i}").OrderBy(k => k),
            storage.SnapshotEntries().Select(e => e.Key).OrderBy(k => k));
    }

    [Fact]
    public void Insert_ChainsCollidingEntriesInTheSameBucket()
    {
        var storage = Storage();

        // Two entries whose hash codes land in the same bucket by construction.
        var bucketCount = 4;
        storage.Insert(0, "a", 1);
        storage.Insert(bucketCount, "b", 2);

        Assert.Equal(2, storage.Count);
        Assert.Contains("a", storage.SnapshotEntries().Select(e => e.Key));
        Assert.Contains("b", storage.SnapshotEntries().Select(e => e.Key));
    }

    [Fact]
    public void Insert_GrowsPastTheLoadFactorWithoutLosingEntries()
    {
        var storage = Storage();

        for (var i = 0; i < 200; i++)
        {
            storage.Insert(i, $"k{i}", i);
        }

        Assert.Equal(200, storage.Count);
        Assert.Equal(200, storage.SnapshotEntries().Count);
    }

    [Fact]
    public void Insert_AfterGrowth_KeepsEveryEntryFindableFromItsBucket()
    {
        var storage = Storage();

        for (var i = 0; i < 100; i++)
        {
            storage.Insert(i, $"k{i}", i);
        }

        for (var i = 0; i < 100; i++)
        {
            var found = false;

            for (var e = storage.BucketHead(storage.BucketIndexFor(i)); e >= 0; e = storage.Entry(e).Next)
            {
                found |= storage.Entry(e).Key == $"k{i}";
            }

            Assert.True(found, $"k{i} was not reachable from its bucket after growth");
        }
    }
}
