using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountValidPathsInATree;

// LeetCode 2867. Count Valid Paths in a Tree: a path (a, b), a != b, is valid when
// exactly one node LABEL on it is prime - there is no separate values array here,
// the node's own 1..n number is what gets primality-checked (verified by hand
// against both of LeetCode's own examples below: labels 2, 3, 5 prime, 1, 4, 6 not).
//
// Deleting every prime node splits the tree into "blobs" of non-prime nodes -
// DisjointSet.Union over every edge whose both endpoints are non-prime finds those
// blobs directly, the same union-same-component idiom RedundantConnectionTests/
// NumberOfProvincesTests already establish. Each prime node p then only needs the
// blob size hanging off each of its neighbors (0 for a neighbor that's itself prime,
// since crossing straight into another prime would make the path invalid before it
// ever reaches a second non-prime node) to count every valid path with p as its
// unique prime: a running pairwise sweep over p's arms adds, one arm at a time,
// (nodes paired with every earlier arm) + (nodes paired with p itself).
//
// Sieve of Eratosthenes reuses DynamicArray<bool> as its composite-tracking array,
// the same CountPrimesTests (LC 204) precedent every other "prime" coverage test in
// this repo already follows - no dedicated sieve/prime primitive exists.
public sealed partial class CountValidPathsInATreeTests
{
    public static TheoryData<int, int[][], long> Examples =>
        new()
        {
            // Primes among labels 1..5: 2, 3, 5. Valid pairs (hand-verified against
            // LeetCode's own explanation): (1,2), (1,3), (1,4), (2,4).
            { 5, [[1, 2], [1, 3], [2, 4], [2, 5]], 4 },
            // Primes among labels 1..6: 2, 3, 5. Valid pairs: (1,2), (1,3), (1,4),
            // (1,6), (2,4), (3,6).
            { 6, [[1, 2], [1, 3], [2, 4], [3, 5], [3, 6]], 6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountValidPaths_LeetCodeExamples_ReturnsExpectedCount(int n, int[][] edges, long expected)
    {
        var count = CountValidPaths(n, edges);

        Assert.Equal(expected, count);
    }

    private static long CountValidPaths(int n, int[][] edges)
    {
        var isPrime = Sieve(n);
        var adjacency = BuildAdjacency(n, edges);
        var components = new DisjointSet(n + 1);

        foreach (var edge in edges)
        {
            if (!isPrime.Get(edge[0]) && !isPrime.Get(edge[1]))
            {
                components.Union(edge[0], edge[1]);
            }
        }

        var blobSize = ComputeBlobSizes(n, isPrime, components);

        long total = 0;
        for (var label = 1; label <= n; label++)
        {
            if (isPrime.Get(label))
            {
                total += CountPairsThroughPrime(label, adjacency, isPrime, components, blobSize);
            }
        }

        return total;
    }

    // Sweeps p's non-prime arms left to right, accumulating armsSoFar so each new arm
    // pairs against every earlier one in O(1) instead of a full pairwise double loop.
    private static long CountPairsThroughPrime(
        int prime, List<int>[] adjacency, DynamicArray<bool> isPrime, DisjointSet components, int[] blobSize)
    {
        long total = 0;
        long armsSoFar = 0;

        foreach (var neighbor in adjacency[prime])
        {
            if (isPrime.Get(neighbor))
            {
                continue;
            }

            var arm = blobSize[components.Find(neighbor)];
            total += (armsSoFar * arm) + arm;
            armsSoFar += arm;
        }

        return total;
    }

    private static int[] ComputeBlobSizes(int n, DynamicArray<bool> isPrime, DisjointSet components)
    {
        var blobSize = new int[n + 1];
        for (var node = 1; node <= n; node++)
        {
            if (!isPrime.Get(node))
            {
                blobSize[components.Find(node)]++;
            }
        }

        return blobSize;
    }

    private static List<int>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<int>[n + 1];
        for (var i = 0; i <= n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            adjacency[edge[0]].Add(edge[1]);
            adjacency[edge[1]].Add(edge[0]);
        }

        return adjacency;
    }

    private static DynamicArray<bool> Sieve(int n)
    {
        var isPrime = new DynamicArray<bool>();
        for (var value = 0; value <= n; value++)
        {
            isPrime.Add(value >= 2);
        }

        for (var factor = 2; (long)factor * factor <= n; factor++)
        {
            if (!isPrime.Get(factor))
            {
                continue;
            }

            for (var multiple = factor * factor; multiple <= n; multiple += factor)
            {
                isPrime.Set(multiple, false);
            }
        }

        return isPrime;
    }
}
