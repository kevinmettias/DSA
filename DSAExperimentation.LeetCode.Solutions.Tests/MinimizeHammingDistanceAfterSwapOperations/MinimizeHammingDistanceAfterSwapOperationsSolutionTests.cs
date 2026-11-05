using DSAExperimentation.LeetCode.Solutions.MinimizeHammingDistanceAfterSwapOperations;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimizeHammingDistanceAfterSwapOperations;

// Harness only. Both component-discovery strategies are
// MinimizeHammingDistanceAfterSwapOperationsSolution's - this file pins them to
// the cases those do not reach: a
// component whose source multiset repeats a value (so the count, not a set, is what
// decides the shortfall), a self-swap pair that must leave every index in its own
// component, and arrays that already match.
public sealed partial class MinimizeHammingDistanceAfterSwapOperationsSolutionTests
{
    public static TheoryData<int[], int[], int[][], int> Cases =>
        new()
        {
            { [1, 1, 2], [2, 2, 1], [[0, 1]], 3 },
            { [1, 2], [2, 1], [[0, 0]], 2 },
            { [1, 2, 3], [1, 2, 3], [], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumHammingDistanceByAdjacencyListBfs_Cases_SumsEachComponentsShortfall(
        int[] source, int[] target, int[][] allowedSwaps, int expected)
    {
        var actual =
            MinimizeHammingDistanceAfterSwapOperationsSolution.MinimumHammingDistanceByAdjacencyListBfs(
                source, target, allowedSwaps);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumHammingDistanceByDisjointSet_Cases_SumsEachComponentsShortfall(
        int[] source, int[] target, int[][] allowedSwaps, int expected)
    {
        var actual =
            MinimizeHammingDistanceAfterSwapOperationsSolution.MinimumHammingDistanceByDisjointSet(
                source, target, allowedSwaps);

        Assert.Equal(expected, actual);
    }
}
