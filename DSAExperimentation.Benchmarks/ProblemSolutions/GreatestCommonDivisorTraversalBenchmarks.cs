using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Greatest Common Divisor Traversal (LC 2709): the naive pairwise approach computes
// Gcd(nums[i], nums[j]) for every pair - O(n^2 log(maxValue)) - unioning whenever it's
// > 1, using a plain int[]-backed union-find with no path compression or rank, vs. this
// repo's DisjointSet unioned by first-seen owner PER PRIME FACTOR (HashMap<factor,
// index>), the same LargestComponentSizeByCommonFactorBenchmarks composition applied to
// a full-connectivity check instead of a largest-component-size tally. Never compares
// two values directly at all: sharing a factor is discovered in O(1) via the map the
// moment the second value reaches that same factor - O(n*sqrt(maxValue)*alpha(n))
// total. Values are products of a small shared prime pool so real overlaps - and
// therefore real union work - actually occur on both arms.
[MemoryDiagnoser]
public class GreatestCommonDivisorTraversalBenchmarks
{
    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13];

    private const int RandomSeed = 2709; // LC problem number

    [Params(50, 400)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length)
            .Select(_ => SharedPrimes[random.Next(SharedPrimes.Length)] * SharedPrimes[random.Next(SharedPrimes.Length)])
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool PairwiseGcdScan()
    {
        var parent = InitializeParent(_values.Length);
        UnionPairsSharingFactor(parent);
        return AllConnected(parent);
    }

    private static int[] InitializeParent(int length)
    {
        var parent = new int[length];

        for (var i = 0; i < parent.Length; i++)
        {
            parent[i] = i;
        }

        return parent;
    }

    private void UnionPairsSharingFactor(int[] parent)
    {
        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                if (Gcd(_values[i], _values[j]) > 1)
                {
                    Union(parent, i, j);
                }
            }
        }
    }

    private bool AllConnected(int[] parent)
    {
        for (var i = 1; i < _values.Length; i++)
        {
            if (Find(parent, i) != Find(parent, 0))
            {
                return false;
            }
        }

        return true;
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
        var components = new DisjointSet(_values.Length);
        var firstIndexWithFactor = new HashMap<int, int>();

        for (var i = 0; i < _values.Length; i++)
        {
            foreach (var factor in PrimeFactors(_values[i]))
            {
                if (firstIndexWithFactor.TryGetValue(factor, out var owner))
                {
                    components.Union(i, owner);
                }
                else
                {
                    firstIndexWithFactor.Set(factor, i);
                }
            }
        }

        return AllConnectedByDisjointSet(components);
    }

    private bool AllConnectedByDisjointSet(DisjointSet components)
    {
        for (var i = 1; i < _values.Length; i++)
        {
            if (!components.IsConnected(0, i))
            {
                return false;
            }
        }

        return true;
    }

    private static IEnumerable<int> PrimeFactors(int value)
    {
        for (var factor = 2; factor * factor <= value; factor++)
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
