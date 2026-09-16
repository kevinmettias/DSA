using DSAExperimentation.LeetCode.CountSequencesToK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSequencesToK;

// Harness only: both strategies live in CountSequencesToKSolution and are
// asserted against the same examples, so a failure names the strategy that
// broke.
public sealed partial class CountSequencesToKTests
{
    public static TheoryData<int[], long, long> Examples =>
        new()
        {
            { [2, 3, 2], 6, 2 },
            { [4, 6, 3], 2, 2 },
            { [1, 5], 1, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSequencesByBruteForceSearch_LeetCodeExamples_ReturnsSequenceCount(
        int[] nums, long target, long expected)
    {
        var actual = CountSequencesToKSolution.CountSequencesByBruteForceSearch(nums, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountSequencesByPrimeExponentMemo_LeetCodeExamples_ReturnsSequenceCount(
        int[] nums, long target, long expected)
    {
        var actual = CountSequencesToKSolution.CountSequencesByPrimeExponentMemo(nums, target);
        Assert.Equal(expected, actual);
    }
}
