using DSAExperimentation.LeetCode.HIndex;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HIndex;

// Harness only. Both strategies are HIndexSolution's - this file just pins them
// to LeetCode's published examples.
public sealed class HIndexTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 0, 6, 1, 5], 3 },
            { [1, 3, 1], 1 },
            { [0, 0], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HIndexByBruteForce_LeetCodeExamples_ReturnsLargestQualifyingH(int[] citations, int expected) =>
        Assert.Equal(expected, HIndexSolution.HIndexByBruteForce(citations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HIndexByMergeSortScan_LeetCodeExamples_ReturnsLargestQualifyingH(int[] citations, int expected) =>
        Assert.Equal(expected, HIndexSolution.HIndexByMergeSortScan(citations));
}
