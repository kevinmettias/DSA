using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.Tests.DataStructures.IntervalSet;

public sealed partial class IntervalSetTests
{
    [Fact]
    public void Count_ReflectsNumberOfDisjointIntervalsAfterMerging()
    {
        var set = new IntervalSet<int>();
        set.Add(1, 2);
        set.Add(2, 3);
        set.Add(10, 11);

        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void Get_ReturnsIntervalAtGivenIndex()
    {
        var set = new IntervalSet<int>();
        set.Add(5, 6);

        Assert.Equal((5, 6), set.Get(0));
    }

    [Fact]
    public void Add_NoOverlap_KeepsIntervalsSeparate()
    {
        var set = new IntervalSet<int>();

        set.Add(1, 2);
        set.Add(4, 5);

        Assert.Equal(2, set.Count);
        Assert.Equal((1, 2), set.Get(0));
        Assert.Equal((4, 5), set.Get(1));
    }

    [Fact]
    public void Add_OverlappingInterval_MergesIntoOne()
    {
        var set = new IntervalSet<int>();

        set.Add(1, 3);
        set.Add(2, 5);

        Assert.Equal(1, set.Count);
        Assert.Equal((1, 5), set.Get(0));
    }

    [Fact]
    public void Add_TouchingIntervals_Merges()
    {
        var set = new IntervalSet<int>();

        set.Add(1, 2);
        set.Add(2, 3);

        Assert.Equal(1, set.Count);
        Assert.Equal((1, 3), set.Get(0));
    }

    [Fact]
    public void Add_NewIntervalSpansMultipleExisting_MergesAllOfThem()
    {
        var set = new IntervalSet<int>();

        set.Add(1, 2);
        set.Add(4, 5);
        set.Add(7, 8);
        set.Add(0, 10);

        Assert.Equal(1, set.Count);
        Assert.Equal((0, 10), set.Get(0));
    }

    [Fact]
    public void Add_IntervalContainedWithinExisting_NoChange()
    {
        var set = new IntervalSet<int>();

        set.Add(1, 10);
        set.Add(3, 5);

        Assert.Equal(1, set.Count);
        Assert.Equal((1, 10), set.Get(0));
    }

    [Fact]
    public void Add_IntervalBetweenTwoExisting_InsertsAtCorrectSortedPosition()
    {
        var set = new IntervalSet<int>();

        set.Add(1, 2);
        set.Add(10, 11);
        set.Add(5, 6);

        Assert.Equal(3, set.Count);
        Assert.Equal((1, 2), set.Get(0));
        Assert.Equal((5, 6), set.Get(1));
        Assert.Equal((10, 11), set.Get(2));
    }

    [Fact]
    public void HasOverlap_NoExistingIntervals_ReturnsFalse()
    {
        var hasOverlap = new IntervalSet<int>().HasOverlap(1, 2);

        Assert.False(hasOverlap);
    }

    [Fact]
    public void HasOverlap_NonOverlappingQuery_ReturnsFalse()
    {
        var set = new IntervalSet<int>();
        set.Add(1, 2);
        set.Add(8, 9);

        var hasOverlap = set.HasOverlap(4, 6);

        Assert.False(hasOverlap);
    }

    [Fact]
    public void HasOverlap_OverlappingQuery_ReturnsTrue()
    {
        var set = new IntervalSet<int>();
        set.Add(1, 5);

        var hasOverlap = set.HasOverlap(3, 7);

        Assert.True(hasOverlap);
    }

    [Fact]
    public void HasOverlap_TouchingQuery_ReturnsTrue()
    {
        var set = new IntervalSet<int>();
        set.Add(1, 2);

        var hasOverlap = set.HasOverlap(2, 3);

        Assert.True(hasOverlap);
    }

    [Fact]
    public void Add_WithCustomComparer_MergesInReverseOrder()
    {
        var set = new IntervalSet<int>(Comparer<int>.Create((a, b) => b.CompareTo(a)));

        set.Add(10, 8);
        set.Add(9, 7);

        Assert.Equal(1, set.Count);
        Assert.Equal((10, 7), set.Get(0));
    }
}
