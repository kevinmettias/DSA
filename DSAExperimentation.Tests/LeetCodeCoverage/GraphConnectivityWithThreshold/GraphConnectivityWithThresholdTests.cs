using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GraphConnectivityWithThreshold;

// LeetCode 1627. Graph Connectivity With Threshold: cities x and y are directly
// connected whenever they share a common divisor strictly greater than threshold,
// so for every divisor z above the threshold, every multiple of z (2z, 3z, ...)
// belongs in z's component - the same sieve-drives-Union composition
// NumberOfOperationsToMakeNetworkConnectedTests/NumberOfProvincesTests already use
// this repo's own DisjointSet for, just with a divisor sieve producing the pairs to
// Union instead of an explicit edge list. Each query then reduces to one
// DisjointSet.IsConnected lookup. Verified against LeetCode's own published
// examples (n=6, threshold=2 and n=6, threshold=0).
public sealed partial class GraphConnectivityWithThresholdTests
{
    [Fact]
    public void AreConnected_ThresholdTwo_OnlyCitiesSharingADivisorAboveTwoAreConnected()
    {
        int[][] queries = [[1, 4], [2, 5], [3, 6]];

        var connected = AreConnected(6, 2, queries);

        Assert.Equal([false, false, true], connected);
    }

    [Fact]
    public void AreConnected_ThresholdZero_EveryCityShareDivisorOneSoAllAreConnected()
    {
        int[][] queries = [[4, 5], [3, 4], [3, 2], [2, 6], [1, 3]];

        var connected = AreConnected(6, 0, queries);

        Assert.Equal([true, true, true, true, true], connected);
    }

    [Fact]
    public void AreConnected_ThresholdLeavesNoEligibleDivisorBelowN_NoCitiesAreConnected()
    {
        int[][] queries = [[1, 2], [2, 3], [1, 5]];

        var connected = AreConnected(5, 3, queries);

        Assert.Equal([false, false, false], connected);
    }

    private static bool[] AreConnected(int n, int threshold, int[][] queries)
    {
        var components = new DisjointSet(n + 1);

        for (var divisor = threshold + 1; divisor <= n; divisor++)
        {
            for (var multiple = 2 * divisor; multiple <= n; multiple += divisor)
            {
                components.Union(divisor, multiple);
            }
        }

        var results = new bool[queries.Length];
        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = components.IsConnected(queries[i][0], queries[i][1]);
        }

        return results;
    }
}
