using DSAExperimentation.LeetCode.Solutions.CountSubmatricesWithAllOnes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountSubmatricesWithAllOnes;

// Harness only. Both the O(rows * cols^2) running-minimum scan and the monotonic-stack
// reduction to LC 907 live in CountSubmatricesWithAllOnesSolution; this file pins both to
// the degenerate all-zero/all-one matrices, where the
// closed form (a size-n run contributes n(n+1)/2 ranges per row) makes the expected count
// checkable by hand.
public sealed partial class CountSubmatricesWithAllOnesSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[1]], 1 },
            { [[0]], 0 },
            { [[1, 1, 1]], 6 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByRunningMinScan_Cases_ReturnsAllOnesSubmatrixCount(int[][] mat, int expected) =>
        Assert.Equal(expected, CountSubmatricesWithAllOnesSolution.CountByRunningMinScan(mat));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountByMonotonicStackDp_Cases_ReturnsAllOnesSubmatrixCount(int[][] mat, int expected) =>
        Assert.Equal(expected, CountSubmatricesWithAllOnesSolution.CountByMonotonicStackDp(mat));
}
