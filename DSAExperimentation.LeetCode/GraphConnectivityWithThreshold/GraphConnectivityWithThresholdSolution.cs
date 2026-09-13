using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.GraphConnectivityWithThreshold;

// LeetCode 1627. Graph Connectivity With Threshold: cities x and y are directly
// connected whenever they share a common divisor strictly greater than threshold.
//
// Nobody needs to enumerate those edges. For every divisor z above the threshold,
// every multiple of z (2z, 3z, ...) shares z with z itself, so unioning z with each
// of its multiples in one sieve sweep yields exactly the same components as the full
// pairwise edge set - O(n log n) unions instead of O(n^2) pair tests. Each query is
// then a single connectivity lookup.
//
// Both strategies run that identical sieve; what differs is the union-find backing
// it, which is the whole point of the comparison.
internal static class GraphConnectivityWithThresholdSolution
{
    // A divisor's first multiple that is not the divisor itself.
    private const int FirstMultipleFactor = 2;

    // The textbook baseline: a bare parent array with no path compression and no
    // union-by-rank, so Find walks the chain every time. Deliberately written without
    // this repo's primitives - it is the arm the composed solution below has to
    // justify itself against, and a dense sieve is exactly the pattern that lets an
    // uncompressed parent chain degrade toward O(n) per Find.
    public static bool[] AreConnectedByNaiveUnionFind(int n, int threshold, int[][] queries)
    {
        var parent = new int[n + 1];

        for (var city = 0; city <= n; city++)
        {
            parent[city] = city;
        }

        for (var divisor = threshold + 1; divisor <= n; divisor++)
        {
            for (var multiple = FirstMultipleFactor * divisor; multiple <= n; multiple += divisor)
            {
                Union(parent, divisor, multiple);
            }
        }

        var results = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = Find(parent, queries[i][0]) == Find(parent, queries[i][1]);
        }

        return results;
    }

    private static int Find(int[] parent, int city)
    {
        while (parent[city] != city)
        {
            city = parent[city];
        }

        return city;
    }

    private static void Union(int[] parent, int first, int second)
    {
        var firstRoot = Find(parent, first);
        var secondRoot = Find(parent, second);

        if (firstRoot != secondRoot)
        {
            parent[firstRoot] = secondRoot;
        }
    }

    // The same sieve over this repo's own DisjointSet, whose path compression and
    // union-by-rank keep Find/Union at O(alpha(n)) amortized - the same
    // sieve-drives-Union composition NumberOfProvinces and
    // NumberOfOperationsToMakeNetworkConnected use, just with a divisor sieve
    // producing the pairs to Union instead of an explicit edge list. Cities are
    // already dense integers in [1, n], so they are their own DisjointSet ids; the
    // set is sized n + 1 so id 0 simply goes unused.
    public static bool[] AreConnectedByDisjointSet(int n, int threshold, int[][] queries)
    {
        var components = new DisjointSet(n + 1);

        for (var divisor = threshold + 1; divisor <= n; divisor++)
        {
            for (var multiple = FirstMultipleFactor * divisor; multiple <= n; multiple += divisor)
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
