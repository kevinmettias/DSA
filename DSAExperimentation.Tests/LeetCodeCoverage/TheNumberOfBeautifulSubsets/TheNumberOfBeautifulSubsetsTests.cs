using DSAExperimentation.LeetCode.TheNumberOfBeautifulSubsets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TheNumberOfBeautifulSubsets;

// Harness only: both counting strategies live in
// TheNumberOfBeautifulSubsetsSolution. One test method per strategy over one shared
// set of examples, so a failure names the strategy that broke - including the
// bitmask baseline, which used to exist only inside the benchmark and was therefore
// asserted by nothing.
public sealed class TheNumberOfBeautifulSubsetsTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [2, 4, 6], 2, 4 },
            { [1], 1, 1 },
            { [1, 1], 1, 3 },
            { [1, 3, 5], 2, 4 },

            // Duplicates never conflict with each other (|x-x| == 0 != k), so every
            // non-empty subset of an all-equal array is beautiful.
            { [4, 4, 4], 1, 7 },

            // No pair differs by k at all, so all 2^n - 1 non-empty subsets count.
            { [2, 4, 6], 1, 7 },

            // A chain of conflicts: {1},{2},{3},{1,3}.
            { [1, 2, 3], 1, 4 },

            // One element conflicting with all three others: any subset of the rest
            // (8, including empty) plus the singleton, less the empty subset.
            { [1, 1, 2, 3], 1, 8 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBeautifulSubsetsByBitmask_LeetCodeExamples_ReturnsExpectedCount(
        int[] nums, int difference, int expected)
    {
        var actual = TheNumberOfBeautifulSubsetsSolution.CountBeautifulSubsetsByBitmask(nums, difference);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountBeautifulSubsetsByPrunedBacktracking_LeetCodeExamples_ReturnsExpectedCount(
        int[] nums, int difference, int expected)
    {
        var actual = TheNumberOfBeautifulSubsetsSolution.CountBeautifulSubsetsByPrunedBacktracking(nums, difference);

        Assert.Equal(expected, actual);
    }
}
