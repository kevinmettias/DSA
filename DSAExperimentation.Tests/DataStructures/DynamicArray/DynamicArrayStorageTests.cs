using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.DataStructures.DynamicArray;

public sealed class DynamicArrayStorageTests
{
    private static DynamicArrayStorage<int> Seeded(params int[] values)
    {
        var storage = new DynamicArrayStorage<int>();

        foreach (var value in values)
        {
            storage.Add(value);
        }

        return storage;
    }

    private static IEnumerable<int> Contents(DynamicArrayStorage<int> storage) =>
        Enumerable.Range(0, storage.Count).Select(storage.Get);

    [Fact]
    public void Count_NewStorage_IsZero() => Assert.Equal(0, new DynamicArrayStorage<int>().Count);

    [Fact]
    public void Add_AppendsAtTheEndAndRaisesTheCount()
    {
        var storage = Seeded(1, 2, 3);

        Assert.Equal(3, storage.Count);
        Assert.Equal([1, 2, 3], Contents(storage));
    }

    [Fact]
    public void Add_PastTheInitialCapacity_KeepsEveryElement()
    {
        var storage = Seeded([.. Enumerable.Range(0, 100)]);
        var expected = Enumerable.Range(0, 100);

        Assert.Equal(100, storage.Count);
        Assert.Equal(expected, Contents(storage));
    }

    [Fact]
    public void Get_ReturnsTheElementAtTheIndex() => Assert.Equal(2, Seeded(1, 2, 3).Get(1));

    [Fact]
    public void Set_ReplacesInPlaceWithoutChangingTheCount()
    {
        var storage = Seeded(1, 2, 3);

        storage.Set(1, 99);

        Assert.Equal(3, storage.Count);
        Assert.Equal([1, 99, 3], Contents(storage));
    }

    [Fact]
    public void InsertAt_ShiftsLaterElementsRight()
    {
        var storage = Seeded(1, 2, 3);

        storage.InsertAt(1, 99);

        Assert.Equal([1, 99, 2, 3], Contents(storage));
    }

    [Fact]
    public void InsertAt_Front_PutsTheElementFirst()
    {
        var storage = Seeded(1, 2);

        storage.InsertAt(0, 99);

        Assert.Equal([99, 1, 2], Contents(storage));
    }

    [Fact]
    public void InsertAt_AtTheCount_AppendsLikeAdd()
    {
        var storage = Seeded(1, 2);

        storage.InsertAt(2, 99);

        Assert.Equal([1, 2, 99], Contents(storage));
    }

    [Fact]
    public void InsertAt_GrowsWhenFull()
    {
        var storage = Seeded([.. Enumerable.Range(0, 64)]);

        storage.InsertAt(0, -1);

        Assert.Equal(65, storage.Count);
        Assert.Equal(-1, storage.Get(0));
    }

    [Fact]
    public void RemoveAt_ShiftsLaterElementsLeft()
    {
        var storage = Seeded(1, 2, 3);

        storage.RemoveAt(1);

        Assert.Equal([1, 3], Contents(storage));
    }

    [Fact]
    public void RemoveAt_Last_JustShortensTheStorage()
    {
        var storage = Seeded(1, 2, 3);

        storage.RemoveAt(2);

        Assert.Equal([1, 2], Contents(storage));
    }

    [Fact]
    public void RemoveAt_ClearsTheVacatedSlotSoAReferenceIsNotRetained()
    {
        var storage = new DynamicArrayStorage<string>();
        storage.Add("a");
        storage.Add("b");

        storage.RemoveAt(1);

        Assert.Equal(1, storage.Count);
        Assert.Null(storage.Get(1));
    }
}
