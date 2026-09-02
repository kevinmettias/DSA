using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimize Deviation in Array (LC 1675): repeatedly halve the current largest
// value until it turns odd. The brute-force baseline rescans the full remaining
// array every step to find the current largest (O(n) per step) against this
// repo's own Heap<int,MaxHeapOrder<int>>, which always offers up the current
// largest in O(log n) - the same LastStoneWeightBenchmarks pairing, just for a
// different per-step reduction rule.
[MemoryDiagnoser]
public class MinimizeDeviationInArrayBenchmarks
{
    // LC problem number, used as the deterministic setup seed.
    private const int RandomSeed = 1675;
    private const int RandomValueUpperBound = 1_000_000;
    private const int ParityDivisor = 2;
    private const int EvenizingMultiplier = 2;
    private const int HalvingDivisor = 2;

    [Params(200, 2_000)]
    public int Length;

    private int[] _transformed = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, RandomValueUpperBound)).ToArray();
        _transformed = nums.Select(n => n % ParityDivisor == 1 ? n * EvenizingMultiplier : n).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearRescanEachStep()
    {
        var values = new List<int>(_transformed);
        var min = values.Min();
        var deviation = int.MaxValue;

        while (true)
        {
            var maxIndex = IndexOfMax(values);
            var max = values[maxIndex];
            deviation = Math.Min(deviation, max - min);

            if (max % ParityDivisor != 0)
            {
                break;
            }

            var half = max / HalvingDivisor;
            min = Math.Min(min, half);
            values[maxIndex] = half;
        }

        return deviation;
    }

    private static int IndexOfMax(List<int> values)
    {
        var best = 0;

        for (var i = 1; i < values.Count; i++)
        {
            if (values[i] > values[best])
            {
                best = i;
            }
        }

        return best;
    }

    [Benchmark]
    public int MaxHeapReduce()
    {
        var heap = new Heap<int, MaxHeapOrder<int>>();
        var min = int.MaxValue;

        foreach (var value in _transformed)
        {
            heap.Push(value);
            min = Math.Min(min, value);
        }

        var deviation = int.MaxValue;

        while (TryReduceStep(heap, ref min, ref deviation))
        {
        }

        return deviation;
    }

    private static bool TryReduceStep(Heap<int, MaxHeapOrder<int>> heap, ref int min, ref int deviation)
    {
        heap.TryPop(out var max);
        deviation = Math.Min(deviation, max - min);

        if (max % ParityDivisor != 0)
        {
            return false;
        }

        var half = max / HalvingDivisor;
        min = Math.Min(min, half);
        heap.Push(half);

        return true;
    }
}
