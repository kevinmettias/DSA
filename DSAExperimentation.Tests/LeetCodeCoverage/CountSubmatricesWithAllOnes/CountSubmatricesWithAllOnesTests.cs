using DSAExperimentation.LeetCode.CountSubmatricesWithAllOnes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubmatricesWithAllOnes;

// Harness only. Both the O(rows * cols^2) running-minimum scan and the monotonic-stack
// reduction to LC 907 live in CountSubmatricesWithAllOnesSolution; this file pins both to
// LeetCode's published examples plus the degenerate all-zero/all-one matrices, where the
// closed form (a size-n run contributes n(n+1)/2 ranges per row) makes the expected count
// checkable by hand.
public sealed class CountSubmatricesWithAllOnesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 0, 1], [1, 1, 0], [1, 1, 0]], 13 },
            { [[0, 1, 1, 0], [0, 1, 1, 1], [1, 1, 1, 0]], 24 },
            { [[1]], 1 },
            { [[0]], 0 },
            { [[0, 0], [0, 0]], 0 },
            { [[1, 1], [1, 1]], 9 },
            { [[1, 1, 1]], 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByRunningMinScan_LeetCodeExamples_ReturnsAllOnesSubmatrixCount(int[][] mat, int expected) =>
        Assert.Equal(expected, CountSubmatricesWithAllOnesSolution.CountByRunningMinScan(mat));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByMonotonicStackDp_LeetCodeExamples_ReturnsAllOnesSubmatrixCount(int[][] mat, int expected) =>
        Assert.Equal(expected, CountSubmatricesWithAllOnesSolution.CountByMonotonicStackDp(mat));
}
