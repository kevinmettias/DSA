using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.CountValidPathsInATree;

// LeetCode 2867. Count Valid Paths in a Tree: a path (a, b) with a != b is valid
// when exactly one node LABEL on it is prime. There is no separate values array -
// the node's own 1..n number is what gets primality-checked (verified by hand
// against both of LeetCode's examples: labels 2, 3, 5 prime; 1, 4, 6 not).
//
// Both strategies decide primality the same way, so the comparison isolates the
// path counting rather than the sieve.
//
// CountValidPathsByPerPairPathWalk is the textbook answer: one BFS per unordered
// pair to walk that pair's own path and count the primes on it - O(n) per pair
// over O(n^2) pairs, so O(n^3) overall.
//
// CountValidPathsByDisjointSetBlobs turns the rule inside out. Deleting every
// prime node splits the tree into "blobs" of non-prime nodes, and DisjointSet.Union
// over every edge whose endpoints are both non-prime finds those blobs directly -
// the same union-for-connectivity idiom Redundant Connection and Number of
// Provinces already use. Each prime p then only needs the blob size hanging off
// each of its neighbours (0 for a neighbour that is itself prime, because crossing
// straight into a second prime invalidates the path before it can reach another
// non-prime node) to count every valid path whose unique prime is p: a running
// pairwise sweep over p's arms adds, one arm at a time, the nodes paired with every
// earlier arm plus the nodes paired with p itself. O(n a(n)) overall.
internal static class CountValidPathsInATreeSolution
{
    // The validity rule itself: one prime on the path, no more and no fewer.
    private const int ExactlyOnePrime = 1;

    // The smallest prime, and therefore the first label a sieve can mark.
    private const int FirstPrime = 2;

    // Labels run 1..n, so this can never collide with a real parent.
    private const int Unvisited = -1;

    // The textbook answer: for every unordered pair, BFS from one end and walk the
    // parent chain back from the other, counting the primes it passes. Deliberately
    // written with a BCL bool[] sieve, BCL adjacency lists and a BCL Queue and
    // nothing else (ARCHITECTURE.md #17.5) - it is the arm the composed strategy
    // below has to justify itself against.
    public static long CountValidPathsByPerPairPathWalk(int n, int[][] edges)
    {
        var isPrime = SieveWithBclArray(n);
        var adjacency = BuildAdjacency(n, edges);
        long total = 0;

        for (var a = 1; a <= n; a++)
        {
            for (var b = a + 1; b <= n; b++)
            {
                if (CountPrimesOnPath(a, b, adjacency, isPrime) == ExactlyOnePrime)
                {
                    total++;
                }
            }
        }

        return total;
    }

    // Sieve of Eratosthenes over a BCL array, for the baseline arm only.
    private static bool[] SieveWithBclArray(int n)
    {
        var isPrime = new bool[n + 1];

        for (var value = FirstPrime; value <= n; value++)
        {
            isPrime[value] = true;
        }

        for (var factor = FirstPrime; (long)factor * factor <= n; factor++)
        {
            if (!isPrime[factor])
            {
                continue;
            }

            for (var multiple = factor * factor; multiple <= n; multiple += factor)
            {
                isPrime[multiple] = false;
            }
        }

        return isPrime;
    }

    // A tree has exactly one path between any two nodes, so the BFS parent chain
    // from b back to a IS that path - no shortest-path argument is needed.
    private static int CountPrimesOnPath(int a, int b, List<int>[] adjacency, bool[] isPrime)
    {
        var parent = BuildParentChain(a, b, adjacency);
        var primes = 0;

        for (var current = b; ; current = parent[current])
        {
            if (isPrime[current])
            {
                primes++;
            }

            if (current == a)
            {
                return primes;
            }
        }
    }

    private static int[] BuildParentChain(int a, int b, List<int>[] adjacency)
    {
        var parent = new int[adjacency.Length];
        Array.Fill(parent, Unvisited);

        var queue = new Queue<int>();
        queue.Enqueue(a);
        parent[a] = a;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (node == b)
            {
                return parent;
            }

            EnqueueUnvisitedNeighbors(node, adjacency, parent, queue);
        }

        return parent;
    }

    private static void EnqueueUnvisitedNeighbors(int node, List<int>[] adjacency, int[] parent, Queue<int> queue)
    {
        foreach (var neighbor in adjacency[node])
        {
            if (parent[neighbor] == Unvisited)
            {
                parent[neighbor] = node;
                queue.Enqueue(neighbor);
            }
        }
    }

    // This repo's own answer: DisjointSet collapses each non-prime blob onto a
    // single representative, then one sweep over the primes pairs their arms.
    public static long CountValidPathsByDisjointSetBlobs(int n, int[][] edges)
    {
        var isPrime = SieveWithDynamicArray(n);
        var adjacency = BuildAdjacency(n, edges);
        var components = UnionNonPrimeEdges(n, edges, isPrime);
        var blobSize = ComputeBlobSizes(n, isPrime, components);
        var blobs = new PrimeBlobs(isPrime, components, blobSize);

        long total = 0;

        for (var label = 1; label <= n; label++)
        {
            if (isPrime.Get(label))
            {
                total += CountPairsThroughPrime(label, adjacency, blobs);
            }
        }

        return total;
    }

    // The same sieve over DynamicArray<bool>, the composite-tracking array Count
    // Primes (LC 204) already establishes for this repo - no dedicated sieve or
    // prime primitive exists.
    private static DynamicArray<bool> SieveWithDynamicArray(int n)
    {
        var isPrime = new DynamicArray<bool>();

        for (var value = 0; value <= n; value++)
        {
            isPrime.Add(value >= FirstPrime);
        }

        for (var factor = FirstPrime; (long)factor * factor <= n; factor++)
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

    private static DisjointSet UnionNonPrimeEdges(int n, int[][] edges, DynamicArray<bool> isPrime)
    {
        // Ids are the labels themselves, so the forest carries one unused slot 0.
        var components = new DisjointSet(n + 1);

        foreach (var edge in edges)
        {
            if (!isPrime.Get(edge[0]) && !isPrime.Get(edge[1]))
            {
                components.Union(edge[0], edge[1]);
            }
        }

        return components;
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

    // Sweeps p's non-prime arms left to right, accumulating armsSoFar so each new
    // arm pairs against every earlier one in O(1) instead of a full double loop.
    private static long CountPairsThroughPrime(int prime, List<int>[] adjacency, PrimeBlobs blobs)
    {
        long total = 0;
        long armsSoFar = 0;

        foreach (var neighbor in adjacency[prime])
        {
            if (blobs.IsPrime.Get(neighbor))
            {
                continue;
            }

            var arm = blobs.BlobSize[blobs.Components.Find(neighbor)];
            total += (armsSoFar * arm) + arm;
            armsSoFar += arm;
        }

        return total;
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

    // The three things a prime's arm sweep reads: which labels are prime, which
    // blob each non-prime label fell into, and how large each blob is.
    private readonly record struct PrimeBlobs(
        DynamicArray<bool> IsPrime,
        DisjointSet Components,
        int[] BlobSize);
}
