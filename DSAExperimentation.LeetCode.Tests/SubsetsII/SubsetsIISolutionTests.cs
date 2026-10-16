using DSAExperimentation.LeetCode.SubsetsII;

namespace DSAExperimentation.LeetCode.Tests.SubsetsII;

// Harness only. Both choose/explore/unchoose enumerations live in
// SubsetsIISolution - this file just pins them to LeetCode's published examples.
// Subset order is not part of LeetCode's contract, so each case is checked as a
// set of subsets rather than an ordered sequence.
public sealed partial class SubsetsIISolutionTests
{
    public static TheoryData<int[], int[][]> Examples =>
        new()
        {
            { [1, 2, 2], [[], [1], [1, 2], [1, 2, 2], [2], [2, 2]] },
            { [0], [[], [0]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllSubsetsBySkipDuplicatesRecursion_LeetCodeExamples_ReturnsEveryUniqueSubsetExactlyOnce(
        int[] nums, int[][] expectedSubsets) =>
        AssertEveryUniqueSubsetOnce(expectedSubsets, SubsetsIISolution.FindAllSubsetsBySkipDuplicatesRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindAllSubsetsByBacktrackSkipDuplicates_LeetCodeExamples_ReturnsEveryUniqueSubsetExactlyOnce(
        int[] nums, int[][] expectedSubsets) =>
        AssertEveryUniqueSubsetOnce(expectedSubsets, SubsetsIISolution.FindAllSubsetsByBacktrackSkipDuplicates(nums));

    private static void AssertEveryUniqueSubsetOnce(int[][] expectedSubsets, List<List<int>> actual)
    {
        Assert.Equal(expectedSubsets.Length, actual.Count);

        var expectedSet = expectedSubsets.Select(subset => string.Join(",", subset)).ToHashSet();
        var actualSet = actual.Select(subset => string.Join(",", subset)).ToHashSet();
        Assert.Equal(expectedSet, actualSet);
    }
}
