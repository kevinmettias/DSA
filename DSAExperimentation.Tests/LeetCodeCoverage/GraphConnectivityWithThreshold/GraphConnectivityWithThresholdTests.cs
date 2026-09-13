using DSAExperimentation.LeetCode.GraphConnectivityWithThreshold;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GraphConnectivityWithThreshold;

// Harness only. Both strategies are GraphConnectivityWithThresholdSolution's - the
// divisor-sieve sweep run over a naive uncompressed parent array (previously untested
// scaffolding inlined in the benchmark) and over this repo's own DisjointSet - so a
// failure names the strategy that broke rather than reporting a disagreement between
// two anonymous arms. Cases below are LeetCode's own published examples plus the
// boundary ones the pre-migration test had already added.
public sealed class GraphConnectivityWithThresholdTests
{
    public static TheoryData<int, int, int[][], bool[]> Examples =>
        new()
        {
            {
                // LeetCode example 1: only 3 and 6 share a divisor above 2.
                6, 2, [[1, 4], [2, 5], [3, 6]], [false, false, true]
            },
            {
                // LeetCode example 2: threshold 0 admits divisor 1, so every city is
                // connected to every other.
                6, 0, [[4, 5], [3, 4], [3, 2], [2, 6], [1, 3]], [true, true, true, true, true]
            },
            {
                // LeetCode example 3: only 2 and 4 are ever unioned, and no query asks
                // about that pair.
                5, 1, [[4, 5], [4, 5], [3, 2], [2, 3], [3, 4]], [false, false, false, false, false]
            },
            {
                // Every eligible divisor exceeds n/2, so no divisor has a second
                // multiple inside [1, n] and nothing is ever unioned.
                5, 3, [[1, 2], [2, 3], [1, 5]], [false, false, false]
            },
            {
                // A city is always connected to itself, even when the sieve performs
                // no unions at all.
                6, 5, [[3, 3], [1, 6]], [true, false]
            },
            {
                // Threshold 1 with a larger n: {2,4,6,8}, {3,6,9} and {5,10} merge
                // through their shared multiples into one component of every composite,
                // while the primes 7 and 1 stay isolated.
                10, 1, [[4, 9], [2, 5], [7, 10], [1, 1]], [true, true, false, true]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AreConnectedByNaiveUnionFind_LeetCodeExamples_ReportsSharedDivisorConnectivity(
        int n, int threshold, int[][] queries, bool[] expected) =>
        Assert.Equal(expected, GraphConnectivityWithThresholdSolution.AreConnectedByNaiveUnionFind(n, threshold, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AreConnectedByDisjointSet_LeetCodeExamples_ReportsSharedDivisorConnectivity(
        int n, int threshold, int[][] queries, bool[] expected) =>
        Assert.Equal(expected, GraphConnectivityWithThresholdSolution.AreConnectedByDisjointSet(n, threshold, queries));
}
