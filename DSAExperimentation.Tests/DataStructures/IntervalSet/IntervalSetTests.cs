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

    // IntervalEndsView is private to IntervalSet: it is the sequence Add and HasOverlap
    // bisect to find the first interval that could touch a query, so both are driven
    // through those two operations. Every case below gives a different answer if the
    // view exposed the wrong value or the wrong length.
    public sealed partial class IntervalEndsViewTests
    {
        [Fact]
        public void Get_ReadsEachIntervalsEnd_SoAQueryInsideALongIntervalFindsIt()
        {
            // Bisecting the starts [1, 20] for 5 would land on [20, 30] and miss [1, 10]
            // entirely; bisecting the ends [10, 30] lands on [1, 10], which holds 5..6.
            var set = new IntervalSet<int>();
            set.Add(1, 10);
            set.Add(20, 30);

            Assert.True(set.HasOverlap(5, 6));

            set.Add(5, 6);

            Assert.Equal(2, set.Count);
            Assert.Equal((1, 10), set.Get(0));
        }

        [Fact]
        public void Length_CoversEveryInterval_SoAQueryPastTheLastOneFindsNothing()
        {
            // A query beyond every end has to bisect to one past the last interval. A
            // view shorter than the set would stop the search at an earlier interval,
            // whose start precedes the query's end, and report a false overlap.
            var set = new IntervalSet<int>();
            set.Add(1, 2);
            set.Add(4, 5);
            set.Add(7, 8);
            set.Add(10, 11);

            Assert.False(set.HasOverlap(12, 13));

            set.Add(12, 13);

            Assert.Equal(5, set.Count);
            Assert.Equal((10, 11), set.Get(3));
            Assert.Equal((12, 13), set.Get(4));
        }
    }
}
