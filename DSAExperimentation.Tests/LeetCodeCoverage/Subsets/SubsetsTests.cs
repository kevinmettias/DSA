using DSAExperimentation.LeetCode.Subsets;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Subsets;

// Harness only. The choose/explore/unchoose enumeration lives in SubsetsSolution -
// this file just pins it to LeetCode's published examples. Subset order is not
// part of LeetCode's contract, so each case is checked as a set of subsets rather
// than an ordered sequence.
public sealed class SubsetsTests
{
    public static TheoryData<int[], int[][]> Examples =>
        new()
        {
            { [1, 2, 3], [[], [1], [2], [1, 2], [3], [1, 3], [2, 3], [1, 2, 3]] },
            { [0], [[], [0]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllSubsetsByBacktrack_LeetCodeExamples_ReturnsEverySubsetExactlyOnce(
        int[] nums, int[][] expectedSubsets)
    {
        var actual = SubsetsSolution.FindAllSubsetsByBacktrack(nums);

        Assert.Equal(expectedSubsets.Length, actual.Count);

        var expectedSet = expectedSubsets.Select(subset => string.Join(",", subset)).ToHashSet();
        var actualSet = actual.Select(subset => string.Join(",", subset)).ToHashSet();
        Assert.Equal(expectedSet, actualSet);
    }
}
