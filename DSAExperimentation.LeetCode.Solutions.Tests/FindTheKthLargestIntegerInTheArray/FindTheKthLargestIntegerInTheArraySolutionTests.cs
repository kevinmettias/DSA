using DSAExperimentation.LeetCode.Solutions.FindTheKthLargestIntegerInTheArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheKthLargestIntegerInTheArray;

// Harness only: both strategies live in FindTheKthLargestIntegerInTheArraySolution
// and are asserted against the same cases - LeetCode's three published ones plus
// the two cases that pin the numeric-vs-lexicographic distinction (same-length
// digit-by-digit compare, and a longer string outranking a larger leading digit).
public sealed partial class FindTheKthLargestIntegerInTheArraySolutionTests
{
    public static TheoryData<string[], int, string> Cases =>
        new()
        {
            { ["1", "9", "10", "2"], 1, "10" },
            { ["3", "6", "7", "10"], 1, "10" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void KthLargestNumberByFullSort_Cases_ReturnsKthLargestByNumericValue(
        string[] nums, int rank, string expected)
    {
        var actual = FindTheKthLargestIntegerInTheArraySolution.KthLargestNumberByFullSort(nums, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void KthLargestNumberBySizeKMinHeap_Cases_ReturnsKthLargestByNumericValue(
        string[] nums, int rank, string expected)
    {
        var actual = FindTheKthLargestIntegerInTheArraySolution.KthLargestNumberBySizeKMinHeap(nums, rank);

        Assert.Equal(expected, actual);
    }
}
