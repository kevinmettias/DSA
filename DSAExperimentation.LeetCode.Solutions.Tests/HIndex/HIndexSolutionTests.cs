using DSAExperimentation.LeetCode.Solutions.HIndex;

namespace DSAExperimentation.LeetCode.Solutions.Tests.HIndex;

// Harness only. Both strategies are HIndexSolution's - this file pins them to a
// researcher whose papers are all uncited, so the h-index is zero. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class HIndexSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [0, 0], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HIndexByBruteForce_Cases_ReturnsLargestQualifyingH(int[] citations, int expected) =>
        Assert.Equal(expected, HIndexSolution.HIndexByBruteForce(citations));

    [Theory]
    [MemberData(nameof(Cases))]
    public void HIndexByMergeSortScan_Cases_ReturnsLargestQualifyingH(int[] citations, int expected) =>
        Assert.Equal(expected, HIndexSolution.HIndexByMergeSortScan(citations));
}
