using DSAExperimentation.LeetCode.KthSmallestInstructions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthSmallestInstructions;

// Harness only. Both strategies are KthSmallestInstructionsSolution's - the
// enumerate-and-sort baseline and the memoized greedy walk - pinned to LeetCode's
// three published examples for destination [2, 3] plus the two ranks that bound
// the ordering (k = 1 is all 'H' first, k = 10 is the last of the ten routes) and
// a one-by-one destination where every step is forced.
public sealed class KthSmallestInstructionsTests
{
    public static TheoryData<int[], long, string> Examples =>
        new()
        {
            { [2, 3], 1, "HHHVV" },
            { [2, 3], 2, "HHVHV" },
            { [2, 3], 3, "HHVVH" },
            { [2, 3], 6, "HVVHH" },
            { [2, 3], 10, "VVHHH" },
            { [1, 1], 2, "VH" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestPathByEnumerateAndSort_LeetCodeExamples_ReturnsKthLexicographicRoute(
        int[] destination, long k, string expected)
    {
        var actual = KthSmallestInstructionsSolution.KthSmallestPathByEnumerateAndSort(destination, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthSmallestPathByMemoizedGreedy_LeetCodeExamples_ReturnsKthLexicographicRoute(
        int[] destination, long k, string expected)
    {
        var actual = KthSmallestInstructionsSolution.KthSmallestPathByMemoizedGreedy(destination, k);

        Assert.Equal(expected, actual);
    }
}
