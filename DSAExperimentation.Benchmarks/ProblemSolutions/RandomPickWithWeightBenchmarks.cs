using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Random Pick with Weight (LC 528): a linear weighted scan through the cumulative
// -sum prefix array (walk until the running total exceeds the draw, O(n) per
// PickIndex) vs. this repo's own BinarySearch.UpperBound over an ArraySequence<int>
// of the same prefix sums (O(log n) per PickIndex), the same prefix-sum-plus-
// BinarySearch pairing RandomPointInNonOverlappingRectanglesBenchmarks already uses
// for area-weighted sampling. Both draw from the same seeded Random sequence so
// neither benefits from a luckier draw order.
[MemoryDiagnoser]
public class RandomPickWithWeightBenchmarks
{
    private const int PickCalls = 500;
    private const int RandomSeed = 528; // LC problem number
    private const int MaxWeightExclusive = 100;

    [Params(50, 2_000)]
    public int WeightCount;

    private int[] _prefixSums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _prefixSums = new int[WeightCount];
        var running = 0;

        for (var i = 0; i < WeightCount; i++)
        {
            running += random.Next(1, MaxWeightExclusive);
            _prefixSums[i] = running;
        }
    }

    [Benchmark(Baseline = true)]
    public long LinearWeightedScan()
    {
        var random = new Random(1);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            var draw = random.Next(_prefixSums[^1]);
            var index = 0;

            while (_prefixSums[index] <= draw)
            {
                index++;
            }

            total += index;
        }

        return total;
    }

    [Benchmark]
    public long BinarySearchUpperBound()
    {
        var random = new Random(1);
        var sequence = new ArraySequence<int>(_prefixSums);
        long total = 0;

        for (var call = 0; call < PickCalls; call++)
        {
            var draw = random.Next(_prefixSums[^1]);
            total += BinarySearch.UpperBound(sequence, draw);
        }

        return total;
    }
}
