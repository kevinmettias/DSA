using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.DataStructures.Graph.Contracts.Ordering;

public sealed class SparseArrayChildrenTests
{
    private static readonly TestNode A = new("A");
    private static readonly TestNode B = new("B");

    [Fact]
    public void Count_CountsOnlyOccupiedSlots()
    {
        Assert.Equal(2, new SparseArrayChildren<TestNode>([null, A, null, B, null]).Count);
    }

    [Fact]
    public void Count_AllSlotsEmpty_IsZero()
    {
        Assert.Equal(0, new SparseArrayChildren<TestNode>([null, null]).Count);
    }

    [Fact]
    public void Get_SkipsEmptySlotsAndCompactsTheIndexSpace()
    {
        var children = new SparseArrayChildren<TestNode>([null, A, null, B, null]);

        Assert.Equal("A", children.Get(0).Name);
        Assert.Equal("B", children.Get(1).Name);
    }

    [Fact]
    public void Get_PreservesSlotOrder()
    {
        var children = new SparseArrayChildren<TestNode>([B, null, A]);

        Assert.Equal(["B", "A"], Enumerable.Range(0, children.Count).Select(i => children.Get(i).Name));
    }

    [Fact]
    public void Get_PastTheLastOccupiedSlot_Throws()
    {
        var children = new SparseArrayChildren<TestNode>([null, A, null]);

        Assert.Throws<IndexOutOfRangeException>(() => children.Get(1));
    }

    [Fact]
    public void Get_EmptySlots_Throws()
    {
        var children = new SparseArrayChildren<TestNode>([null, null]);

        Assert.Throws<IndexOutOfRangeException>(() => children.Get(0));
    }
}
