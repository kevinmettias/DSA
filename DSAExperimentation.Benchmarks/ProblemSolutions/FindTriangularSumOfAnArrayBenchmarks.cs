using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Triangular Sum of an Array (LC 2221): no closed-form shortcut (a Lucas'
// theorem walk over Pascal's triangle mod 10) is composable from this repo's
// existing primitives - nothing here provides modular-binomial-coefficient
// machinery - so both strategies below run the identical O(n^2) pairwise-reduction
// simulation: a raw in-place int[] buffer vs. this repo's own DynamicArray<int>
// rebuilt fresh each round, isolating the primitive's own overhead rather than
// comparing two different algorithms.
[MemoryDiagnoser]
public class FindTriangularSumOfAnArrayBenchmarks
{
    private const int RandomSeed = 2221; // LC problem number
    private const int ValueBound = 10;

    [Params(200, 2_000)]
    public int Length;

    private int[] _initial = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _initial = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InPlaceArrayReduction()
    {
        var current = (int[])_initial.Clone();
        var size = current.Length;

        while (size > 1)
        {
            for (var i = 0; i < size - 1; i++)
            {
                current[i] = (current[i] + current[i + 1]) % ValueBound;
            }

            size--;
        }

        return current[0];
    }

    [Benchmark]
    public int DynamicArrayReduction()
    {
        var current = new DynamicArray<int>();

        foreach (var value in _initial)
        {
            current.Add(value);
        }

        while (current.Count > 1)
        {
            current = ReduceOnce(current);
        }

        return current.Get(0);
    }

    private static DynamicArray<int> ReduceOnce(DynamicArray<int> array)
    {
        var next = new DynamicArray<int>();

        for (var i = 0; i < array.Count - 1; i++)
        {
            next.Add((array.Get(i) + array.Get(i + 1)) % ValueBound);
        }

        return next;
    }
}
