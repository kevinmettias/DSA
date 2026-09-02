using DSAExperimentation.LeetCode.PermutationsII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PermutationsII;

// Harness only: both strategies live in PermutationsIISolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed class PermutationsIITests
{
    public static TheoryData<int[], int[][]> Examples =>
        new()
        {
            { [1, 1, 2], [[1, 1, 2], [1, 2, 1], [2, 1, 1]] },
            { [1, 2, 3], [[1, 2, 3], [1, 3, 2], [2, 1, 3], [2, 3, 1], [3, 1, 2], [3, 2, 1]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PermuteUniqueBySpecializedRecursion_LeetCodeExamples_ReturnsDistinctPermutations(
        int[] nums, int[][] expected) =>
        AssertSamePermutations(expected, PermutationsIISolution.PermuteUniqueBySpecializedRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PermuteUniqueByBacktracking_LeetCodeExamples_ReturnsDistinctPermutations(
        int[] nums, int[][] expected) =>
        AssertSamePermutations(expected, PermutationsIISolution.PermuteUniqueByBacktracking(nums));

    private static void AssertSamePermutations(int[][] expected, List<List<int>> actual)
    {
        var actualArrays = actual.Select(x => x.ToArray()).ToArray();
        Assert.Equal(expected.Length, actualArrays.Length);

        foreach (var permutation in expected)
        {
            Assert.Contains(actualArrays, x => x.SequenceEqual(permutation));
        }
    }
}
