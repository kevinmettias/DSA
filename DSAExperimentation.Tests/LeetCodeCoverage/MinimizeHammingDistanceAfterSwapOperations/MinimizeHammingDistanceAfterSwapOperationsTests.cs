using DSAExperimentation.LeetCode.MinimizeHammingDistanceAfterSwapOperations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimizeHammingDistanceAfterSwapOperations;

// Harness only. Both component-discovery strategies are
// MinimizeHammingDistanceAfterSwapOperationsSolution's - this file pins them to
// LeetCode's three published examples plus the cases those do not reach: a
// component whose source multiset repeats a value (so the count, not a set, is what
// decides the shortfall), a self-swap pair that must leave every index in its own
// component, and arrays that already match.
public sealed class MinimizeHammingDistanceAfterSwapOperationsTests
{
    public static TheoryData<int[], int[], int[][], int> Examples =>
        new()
        {
            { [1, 2, 3, 4], [2, 1, 4, 5], [[0, 1], [2, 3]], 1 },
            { [1, 2, 3, 4], [1, 3, 2, 4], [], 2 },
            { [5, 1, 2, 4, 3], [1, 5, 4, 2, 3], [[0, 4], [4, 2], [1, 3], [1, 4]], 0 },
            { [1, 1, 2], [2, 2, 1], [[0, 1]], 3 },
            { [1, 2], [2, 1], [[0, 0]], 2 },
            { [1, 2, 3], [1, 2, 3], [], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumHammingDistanceByAdjacencyListBfs_LeetCodeExamples_SumsEachComponentsShortfall(
        int[] source, int[] target, int[][] allowedSwaps, int expected)
    {
        var actual =
            MinimizeHammingDistanceAfterSwapOperationsSolution.MinimumHammingDistanceByAdjacencyListBfs(
                source, target, allowedSwaps);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumHammingDistanceByDisjointSet_LeetCodeExamples_SumsEachComponentsShortfall(
        int[] source, int[] target, int[][] allowedSwaps, int expected)
    {
        var actual =
            MinimizeHammingDistanceAfterSwapOperationsSolution.MinimumHammingDistanceByDisjointSet(
                source, target, allowedSwaps);

        Assert.Equal(expected, actual);
    }
}
