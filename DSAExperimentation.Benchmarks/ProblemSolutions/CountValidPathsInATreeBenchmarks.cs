using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Valid Paths in a Tree (LC 2867): the naive baseline runs one BFS per
// unordered node pair to walk that pair's own path and count primes on it directly -
// O(n) per pair, O(n^2) pairs, so O(n^3) total. The primitive-based arm unions every
// non-prime-non-prime edge with DisjointSet (splitting the tree into non-prime
// "blobs" in O(n a(n))), the same DisjointSet-for-connectivity idiom
// RedundantConnectionTests/NumberOfProvincesTests already establish, then does one
// O(n) sweep pairing each prime node's own blob-sized arms - O(n) total instead of
// O(n^3), a genuine complexity split like TwoSumBenchmarks'/LongestCycleInAGraph
// Benchmarks' own naive-vs-primitive arms. Both arms share the same DynamicArray<bool>
// sieve CountPrimesBenchmarks already establishes, so the comparison isolates the
// path-counting strategy, not the primality check.
[MemoryDiagnoser]
public class CountValidPathsInATreeBenchmarks
{
    [Params(50, 200)]
    public int NodeCount;

    private int[][] _edges = null!;

    // A random recursive tree (parent[i] uniform in [1, i)), the same shape
    // MinimumHeightTreesBenchmarks/LongestPathWithDifferentAdjacentCharactersBenchmarks
    // already build their own random test trees from.
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _edges = new int[NodeCount - 1][];
        for (var i = 2; i <= NodeCount; i++)
        {
            _edges[i - 2] = [random.Next(1, i), i];
        }
    }

    [Benchmark(Baseline = true)]
    public long BruteForce()
    {
        var isPrime = Sieve(NodeCount);
        var adjacency = BuildAdjacency(NodeCount, _edges);
        long total = 0;

        for (var a = 1; a <= NodeCount; a++)
        {
            for (var b = a + 1; b <= NodeCount; b++)
            {
                if (CountPrimesOnPath(a, b, adjacency, isPrime) == 1)
                {
                    total++;
                }
            }
        }

        return total;
    }

    private static int CountPrimesOnPath(int a, int b, List<int>[] adjacency, DynamicArray<bool> isPrime)
    {
        var parent = new int[adjacency.Length];
        Array.Fill(parent, -1);

        var queue = new Queue<int>();
        queue.Enqueue(a);
        parent[a] = a;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node == b)
            {
                break;
            }

            foreach (var neighbor in adjacency[node])
            {
                if (parent[neighbor] == -1)
                {
                    parent[neighbor] = node;
                    queue.Enqueue(neighbor);
                }
            }
        }

        var primes = 0;
        for (var current = b; ; current = parent[current])
        {
            if (isPrime.Get(current))
            {
                primes++;
            }

            if (current == a)
            {
                return primes;
            }
        }
    }

    [Benchmark]
    public long DisjointSetBlobCounting()
    {
        var isPrime = Sieve(NodeCount);
        var adjacency = BuildAdjacency(NodeCount, _edges);
        var components = new DisjointSet(NodeCount + 1);

        foreach (var edge in _edges)
        {
            if (!isPrime.Get(edge[0]) && !isPrime.Get(edge[1]))
            {
                components.Union(edge[0], edge[1]);
            }
        }

        var blobSize = new int[NodeCount + 1];
        for (var node = 1; node <= NodeCount; node++)
        {
            if (!isPrime.Get(node))
            {
                blobSize[components.Find(node)]++;
            }
        }

        long total = 0;
        for (var label = 1; label <= NodeCount; label++)
        {
            if (!isPrime.Get(label))
            {
                continue;
            }

            long armsSoFar = 0;
            foreach (var neighbor in adjacency[label])
            {
                if (isPrime.Get(neighbor))
                {
                    continue;
                }

                var arm = blobSize[components.Find(neighbor)];
                total += (armsSoFar * arm) + arm;
                armsSoFar += arm;
            }
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
