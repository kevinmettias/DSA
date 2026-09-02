using DSAExperimentation.LeetCode.TwoSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoSum;

// Harness only: the algorithms live in TwoSumSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed class TwoSumTests
{
    public static TheoryData<int[], int, bool, int, int> Examples =>
        new()
        {
            { [2, 7, 11, 15], 9, true, 0, 1 },
            { [3, 2, 4], 6, true, 1, 2 },
            { [3, 3], 6, true, 0, 1 },
            { [1, 2, 3], 100, false, 0, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindIndicesByBruteForce_LeetCodeExamples_ReturnsMatchingPairIndices(
        int[] nums, int target, bool expectedFound, int expectedFirst, int expectedSecond)
    {
        var found = TwoSumSolution.TryFindIndicesByBruteForce(nums, target, out var first, out var second);

        AssertResult(expectedFound, expectedFirst, expectedSecond, found, first, second);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindIndicesByHashMap_LeetCodeExamples_ReturnsMatchingPairIndices(
        int[] nums, int target, bool expectedFound, int expectedFirst, int expectedSecond)
    {
        var found = TwoSumSolution.TryFindIndicesByHashMap(nums, target, out var first, out var second);

        AssertResult(expectedFound, expectedFirst, expectedSecond, found, first, second);
    }

    private static void AssertResult(
        bool expectedFound, int expectedFirst, int expectedSecond, bool found, int first, int second)
    {
        Assert.Equal(expectedFound, found);
        Assert.Equal(expectedFirst, first);
        Assert.Equal(expectedSecond, second);
    }
}
