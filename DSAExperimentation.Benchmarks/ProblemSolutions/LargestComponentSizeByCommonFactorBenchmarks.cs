using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Largest Component Size by Common Factor (LC 952): the naive pairwise approach that
// decides "do these two values belong together" by computing Gcd(nums[i], nums[j]) for
// every pair - O(n^2 log(maxValue)) - plus a plain int[]-backed union-find with no path
// compression or rank (O(n) worst-case Find/relink), vs. this repo's DisjointSet unioned
// by first-seen owner PER PRIME FACTOR (tracked in a HashMap<factor,index>), the same
// AccountsMergeBenchmarks union-on-shared-key shape applied to factors instead of
// emails. Never compares two values directly at all: sharing a factor is discovered in
// O(1) via the map the moment the second value reaches that same factor -
// O(n*sqrt(maxValue)*alpha(n)) total. Values are drawn as products of a small shared
// prime pool so real overlaps - and therefore real merge work - actually occur, the same
// "force genuine matches, not coincidental ones" intent AccountsMergeBenchmarks' own
// generator already uses.
[MemoryDiagnoser]
public class LargestComponentSizeByCommonFactorBenchmarks
{
    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13];

    [Params(50, 400)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(952);
        _values = Enumerable.Range(0, Length)
            .Select(_ => SharedPrimes[random.Next(SharedPrimes.Length)] * SharedPrimes[random.Next(SharedPrimes.Length)])
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseGcdScan()
    {
        var parent = new int[_values.Length];

        for (var i = 0; i < parent.Length; i++)
        {
            parent[i] = i;
        }

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

        var counts = new int[_values.Length];
        var largest = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            var root = Find(parent, i);
            counts[root]++;
            largest = Math.Max(largest, counts[root]);
        }

        return largest;
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
    public int DisjointSetByPrimeFactor()
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

        var sizeByRoot = new HashMap<int, int>();
        var largest = 1;

        for (var i = 0; i < _values.Length; i++)
        {
            var root = components.Find(i);
            sizeByRoot.TryGetValue(root, out var count);
            count++;
            sizeByRoot.Set(root, count);
            largest = Math.Max(largest, count);
        }

        return largest;
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
