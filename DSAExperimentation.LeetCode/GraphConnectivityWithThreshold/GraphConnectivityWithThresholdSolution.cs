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
    public static bool[] AreConnectedByNaiveUnionFind(int cityCount, int threshold, int[][] queries)
    {
        var parent = BuildNaiveComponents(cityCount, threshold);

        return ConnectivityByNaiveFind(parent, queries);
    }

    // The bare parent array plus the sieve over it: every divisor above the threshold
    // shares itself with each of its multiples, so unioning the two joins exactly the
    // cities that share a divisor.
    private static int[] BuildNaiveComponents(int cityCount, int threshold)
    {
        var parent = new int[cityCount + 1];

        for (var city = 0; city <= cityCount; city++)
        {
            parent[city] = city;
        }

        for (var divisor = threshold + 1; divisor <= cityCount; divisor++)
        {
            for (var multiple = FirstMultipleFactor * divisor; multiple <= cityCount; multiple += divisor)
            {
                Union(parent, divisor, multiple);
            }
        }

        return parent;
    }

    // One connectivity lookup per query, both sides resolved through the naive Find.
    private static bool[] ConnectivityByNaiveFind(int[] parent, int[][] queries)
    {
        var results = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = Find(parent, queries[i][0]) == Find(parent, queries[i][1]);
        }

        return results;
    }

    // The same sieve over this repo's own DisjointSet, whose path compression and
    // union-by-rank keep Find/Union at O(alpha(n)) amortized - the same
    // sieve-drives-Union composition NumberOfProvinces and
    // NumberOfOperationsToMakeNetworkConnected use, just with a divisor sieve
    // producing the pairs to Union instead of an explicit edge list. Cities are
    // already dense integers in [1, cityCount], so they are their own DisjointSet
    // ids; the set is sized cityCount + 1 so id 0 simply goes unused.
    public static bool[] AreConnectedByDisjointSet(int cityCount, int threshold, int[][] queries)
    {
        var components = BuildDisjointSetComponents(cityCount, threshold);

        return ConnectivityByDisjointSet(components, queries);
    }

    // The same divisor sieve, driving this repo's DisjointSet instead of a bare parent
    // array.
    private static DisjointSet BuildDisjointSetComponents(int cityCount, int threshold)
    {
        var components = new DisjointSet(cityCount + 1);

        for (var divisor = threshold + 1; divisor <= cityCount; divisor++)
        {
            for (var multiple = FirstMultipleFactor * divisor; multiple <= cityCount; multiple += divisor)
            {
                components.Union(divisor, multiple);
            }
        }

        return components;
    }

    // One connectivity lookup per query, both sides resolved through DisjointSet's own
    // compressed Find.
    private static bool[] ConnectivityByDisjointSet(DisjointSet components, int[][] queries)
    {
        var results = new bool[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = components.IsConnected(queries[i][0], queries[i][1]);
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
}
