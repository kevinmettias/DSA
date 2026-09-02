using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// GCD Sort of an Array (LC 1998): PairwiseGcdUnionFind treats every distinct value as its
// own union-find node and unions any two DISTINCT values actually present in nums whenever
// Gcd(a, b) > 1 - O(distinctValues^2 log(maxValue)) - using a plain int[]-backed union-find
// with no path compression/rank, then sorts the comparison copy via BCL Array.Sort.
// DisjointSetByPrimeFactor instead unions each value directly with its own prime factors
// (never comparing two values against each other) via this repo's own DisjointSet, and
// sorts via this repo's own MergeSort - the same pair of primitives GCDSortOfAnArrayTests
// exercises for correctness. Values are drawn as products of a small shared prime pool so
// real shared-factor chains actually occur, the same generator intent
// LargestComponentSizeByCommonFactorBenchmarks already uses.
[MemoryDiagnoser]
public class GCDSortOfAnArrayBenchmarks
{
    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13];

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1998;

    [Params(50, 400)]
    public int Length;

    private int[] _values = null!;
    private int _maxValue;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length)
            .Select(_ => SharedPrimes[random.Next(SharedPrimes.Length)] * SharedPrimes[random.Next(SharedPrimes.Length)])
            .ToArray();
        _maxValue = _values.Max();
    }

    [Benchmark(Baseline = true)]
    public bool PairwiseGcdUnionFind()
    {
        var distinctValues = _values.Distinct().ToArray();
        var parent = InitializeParent(_maxValue + 1);

        UnionPairsWithSharedFactor(parent, distinctValues);

        var sorted = (int[])_values.Clone();
        Array.Sort(sorted);

        return SameComponentOrder(parent, sorted);
    }

    private static void UnionPairsWithSharedFactor(int[] parent, int[] distinctValues)
    {
        for (var i = 0; i < distinctValues.Length; i++)
        {
            for (var j = i + 1; j < distinctValues.Length; j++)
            {
                if (Gcd(distinctValues[i], distinctValues[j]) > 1)
                {
                    Union(parent, distinctValues[i], distinctValues[j]);
                }
            }
        }
    }

    private bool SameComponentOrder(int[] parent, int[] sorted)
    {
        for (var i = 0; i < _values.Length; i++)
        {
            if (Find(parent, _values[i]) != Find(parent, sorted[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static int[] InitializeParent(int count)
    {
        var parent = new int[count];
        for (var i = 0; i < count; i++)
        {
            parent[i] = i;
        }

        return parent;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    private static int Find(int[] parent, int id)
    {
        while (parent[id] != id)
        {
            id = parent[id];
        }

        return id;
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

    [Benchmark]
    public bool DisjointSetByPrimeFactor()
    {
        var components = new DisjointSet(_maxValue + 1);

        foreach (var value in _values)
        {
            foreach (var factor in PrimeFactors(value))
            {
                components.Union(value, factor);
            }
        }

        var sorted = (int[])_values.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        for (var i = 0; i < _values.Length; i++)
        {
            if (!components.IsConnected(_values[i], sorted[i]))
            {
                return false;
            }
        }

        return true;
    }

    private const int SmallestPrime = 2;

    private static IEnumerable<int> PrimeFactors(int value)
    {
        for (var factor = SmallestPrime; factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            yield return factor;

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }
}
