using DSAExperimentation.LeetCode.SubsetsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubsetsII;

// Harness only. The choose/explore/unchoose enumeration lives in
// SubsetsIISolution - this file just pins it to LeetCode's published examples.
// Subset order is not part of LeetCode's contract, so each case is checked as a
// set of subsets rather than an ordered sequence.
public sealed class SubsetsIITests
{
    public static TheoryData<int[], int[][]> Examples =>
        new()
        {
            { [1, 2, 2], [[], [1], [1, 2], [1, 2, 2], [2], [2, 2]] },
            { [0], [[], [0]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllSubsetsByBacktrackSkipDuplicates_LeetCodeExamples_ReturnsEveryUniqueSubsetExactlyOnce(
        int[] nums, int[][] expectedSubsets)
    {
        var actual = SubsetsIISolution.FindAllSubsetsByBacktrackSkipDuplicates(nums);

        Assert.Equal(expectedSubsets.Length, actual.Count);

        var expectedSet = expectedSubsets.Select(subset => string.Join(",", subset)).ToHashSet();
        var actualSet = actual.Select(subset => string.Join(",", subset)).ToHashSet();
        Assert.Equal(expectedSet, actualSet);
    }
}
