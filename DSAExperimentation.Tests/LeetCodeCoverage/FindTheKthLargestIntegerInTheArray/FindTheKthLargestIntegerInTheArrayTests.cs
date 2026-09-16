using DSAExperimentation.LeetCode.FindTheKthLargestIntegerInTheArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheKthLargestIntegerInTheArray;

// Harness only: both strategies live in FindTheKthLargestIntegerInTheArraySolution
// and are asserted against the same examples - LeetCode's three published ones plus
// the two cases that pin the numeric-vs-lexicographic distinction (same-length
// digit-by-digit compare, and a longer string outranking a larger leading digit).
public sealed class FindTheKthLargestIntegerInTheArrayTests
{
    public static TheoryData<string[], int, string> Examples =>
        new()
        {
            { ["3", "6", "7", "10"], 4, "3" },
            { ["2", "21", "12", "1"], 3, "2" },
            { ["0", "0"], 2, "0" },
            { ["1", "9", "10", "2"], 1, "10" },
            { ["3", "6", "7", "10"], 1, "10" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthLargestNumberByFullSort_LeetCodeExamples_ReturnsKthLargestByNumericValue(
        string[] nums, int rank, string expected)
    {
        var actual = FindTheKthLargestIntegerInTheArraySolution.KthLargestNumberByFullSort(nums, rank);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthLargestNumberBySizeKMinHeap_LeetCodeExamples_ReturnsKthLargestByNumericValue(
        string[] nums, int rank, string expected)
    {
        var actual = FindTheKthLargestIntegerInTheArraySolution.KthLargestNumberBySizeKMinHeap(nums, rank);

        Assert.Equal(expected, actual);
    }
}
