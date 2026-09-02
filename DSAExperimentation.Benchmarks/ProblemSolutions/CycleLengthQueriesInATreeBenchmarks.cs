using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Cycle Length Queries in a Tree (LC 2509): a naive per-query ancestor-Dictionary
// approach (walk a up to the root recording every ancestor's distance from a in a
// freshly-allocated Dictionary<int,int>, then walk b up until it lands on one of
// those ancestors) vs. this repo's HeapArrayIndex.Parent two-pointer walk (walk
// the deeper node up to the shallower one's depth, then both up together to the
// LCA), which touches only two ints and allocates nothing per query. Both are
// O(log id) per query - the tree's depth is bounded by n <= 30 regardless of how
// many queries run - so the gap this benchmark demonstrates is allocation, not
// asymptotic complexity: MemoryDiagnoser should show the Dictionary-per-query
// baseline's Gen0/bytes grow with QueriesCount while the Parent-arithmetic
// approach stays at zero.
[MemoryDiagnoser]
public class CycleLengthQueriesInATreeBenchmarks
{
    private const int RandomSeed = 2509;
    private const int TreeLevels = 20; // ids range over [1, 2^20 - 1]

    [Params(1_000, 20_000)]
    public int QueriesCount;

    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var maxId = (1 << TreeLevels) - 1;

        _queries = Enumerable.Range(0, QueriesCount)
            .Select(_ => BuildDistinctPair(random, maxId))
            .ToArray();
    }

    private static int[] BuildDistinctPair(Random random, int maxId)
    {
        var a = random.Next(1, maxId + 1);
        int b;

        do
        {
            b = random.Next(1, maxId + 1);
        }
        while (b == a);

        return [a, b];
    }

    [Benchmark(Baseline = true)]
    public int AncestorDictionaryWalk()
    {
        var total = 0;

        foreach (var query in _queries)
        {
            total += CycleLengthByAncestorDictionary(query[0], query[1]);
        }

        return total;
    }

    private static int CycleLengthByAncestorDictionary(int a, int b)
    {
        var distanceFromA = new Dictionary<int, int>();
        var depth = 0;
        var current = a;

        while (current >= 1)
        {
            distanceFromA[current] = depth;
            current /= 2;
            depth++;
        }

        var distanceFromB = 0;
        current = b;

        while (!distanceFromA.ContainsKey(current))
        {
            current /= 2;
            distanceFromB++;
        }

        return distanceFromA[current] + distanceFromB + 1;
    }

    [Benchmark]
    public int ParentIndexTwoPointerWalk()
    {
        var total = 0;

        foreach (var query in _queries)
        {
            total += CycleLength(query[0], query[1]);
        }

        return total;
    }

    private static int CycleLength(int a, int b)
    {
        var indexA = a - 1;
        var indexB = b - 1;
        var depthA = Depth(indexA);
        var depthB = Depth(indexB);
        var distance = 0;

        while (depthA > depthB)
        {
            indexA = HeapArrayIndex.Parent(indexA);
            depthA--;
            distance++;
        }

        while (depthB > depthA)
        {
            indexB = HeapArrayIndex.Parent(indexB);
            depthB--;
            distance++;
        }

        while (indexA != indexB)
        {
            indexA = HeapArrayIndex.Parent(indexA);
            indexB = HeapArrayIndex.Parent(indexB);
            distance += 2;
        }

        return distance + 1;
    }

    private static int Depth(int index)
    {
        var depth = 0;

        while (index > 0)
        {
            index = HeapArrayIndex.Parent(index);
            depth++;
        }

        return depth;
    }
}
