namespace DSAExperimentation.LeetCode.Tests;

// LeetCode's [start, end] rows, read once for every interval problem. Swapping the two
// coordinates would still type-check everywhere they are used.
public sealed partial class LeetCodeIntervalsTests
{
    [Fact]
    public void AsPairs_LeetCodeRows_ReadsStartThenEndInRowOrder() =>
        Assert.Equal([(1, 3), (8, 10), (2, 6)], LeetCodeIntervals.AsPairs([[1, 3], [8, 10], [2, 6]]));

    [Fact]
    public void SortedByEnd_UnsortedPairs_OrdersByEndAlone() =>
        Assert.Equal([(5, 1), (1, 3), (0, 6), (8, 10)], LeetCodeIntervals.SortedByEnd([(8, 10), (1, 3), (0, 6), (5, 1)]));

    // Equal ends keep the order they were given in, whatever their starts.
    [Fact]
    public void SortedByEnd_EqualEnds_KeepTheirInputOrder() =>
        Assert.Equal([(4, 5), (1, 5), (3, 5)], LeetCodeIntervals.SortedByEnd([(4, 5), (1, 5), (3, 5)]));

    // A sorted copy: a workload reused across benchmark iterations must start each one unsorted.
    [Fact]
    public void SortedByEnd_InputPairs_AreLeftAsGiven()
    {
        (int Start, int End)[] intervals = [(2, 9), (1, 3)];

        _ = LeetCodeIntervals.SortedByEnd(intervals);

        Assert.Equal([(2, 9), (1, 3)], intervals);
    }
}
