using DSAExperimentation.LeetCode.Subsets;

namespace DSAExperimentation.LeetCode.Tests.Subsets;

// Harness only. The choose/explore/unchoose enumeration lives in SubsetsSolution -
// this file just pins it to LeetCode's published examples. Subset order is not
// part of LeetCode's contract, so each case is checked as a set of subsets rather
// than an ordered sequence.
public sealed partial class SubsetsSolutionTests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllSubsetsByBitmask_LeetCodeExamples_ReturnsEverySubsetExactlyOnce(
        int[] nums, int[][] expectedSubsets)
    {
        var actual = SubsetsSolution.FindAllSubsetsByBitmask(nums);

        Assert.Equal(expectedSubsets.Length, actual.Count);

        var expectedSet = expectedSubsets.Select(subset => string.Join(",", subset)).ToHashSet();
        var actualSet = actual.Select(subset => string.Join(",", subset)).ToHashSet();
        Assert.Equal(expectedSet, actualSet);
    }

    // The two arms are competing strategies for one question, so the property worth pinning is
    // that they enumerate the same family of subsets rather than merely the same count.
    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllSubsets_AgreeOnEveryExample(int[] nums, int[][] expectedSubsets)
    {
        var byBacktrack = SubsetsSolution.FindAllSubsetsByBacktrack(nums)
            .Select(subset => string.Join(",", subset))
            .ToHashSet();
        var byBitmask = SubsetsSolution.FindAllSubsetsByBitmask(nums)
            .Select(subset => string.Join(",", subset))
            .ToHashSet();

        Assert.Equal(byBacktrack, byBitmask);
    }
}
