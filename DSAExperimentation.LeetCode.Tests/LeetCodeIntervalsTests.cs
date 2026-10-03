namespace DSAExperimentation.LeetCode.Tests;

// LeetCode's [start, end] rows, read once for every interval problem. Swapping the two
// coordinates would still type-check everywhere they are used.
public sealed partial class LeetCodeIntervalsTests
{
    [Fact]
    public void AsPairs_LeetCodeRows_ReadsStartThenEndInRowOrder() =>
        Assert.Equal([(1, 3), (8, 10), (2, 6)], LeetCodeIntervals.AsPairs([[1, 3], [8, 10], [2, 6]]));
}
