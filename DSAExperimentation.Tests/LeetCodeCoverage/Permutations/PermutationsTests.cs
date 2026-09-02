using DSAExperimentation.LeetCode.Permutations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Permutations;

// Harness only: both strategies live in PermutationsSolution and are asserted
// against the same examples, so a failure names the strategy that broke.
public sealed class PermutationsTests
{
    public static TheoryData<int[], int[][]> Examples =>
        new()
        {
            { [1, 2, 3], [[1, 2, 3], [1, 3, 2], [2, 1, 3], [2, 3, 1], [3, 1, 2], [3, 2, 1]] },
            { [0, 1], [[0, 1], [1, 0]] },
            { [1], [[1]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PermuteBySpecializedRecursion_LeetCodeExamples_ReturnsAllPermutations(
        int[] nums, int[][] expected) =>
        AssertSamePermutations(expected, PermutationsSolution.PermuteBySpecializedRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PermuteByBacktracking_LeetCodeExamples_ReturnsAllPermutations(
        int[] nums, int[][] expected) =>
        AssertSamePermutations(expected, PermutationsSolution.PermuteByBacktracking(nums));

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
