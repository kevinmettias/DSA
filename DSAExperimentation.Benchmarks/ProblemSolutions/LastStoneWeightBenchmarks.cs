using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Last Stone Weight (LC 1046): repeatedly smashing the two heaviest stones. The
// brute-force baseline rescans the full remaining list every smash to find both
// the largest and second-largest (O(stones) per smash, O(stones^2) total) against
// this repo's own Heap<int,MaxHeapOrder<int>>, which always offers up the current
// heaviest stone in O(log stones) (O(stones log stones) total).
[MemoryDiagnoser]
public class LastStoneWeightBenchmarks
{
    private const int RandomSeed = 1046; // LC problem number
    private const int MaxStoneWeightExclusive = 1_000;

    [Params(200, 5_000)]
    public int StoneCount;

    private int[] _stones = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _stones = Enumerable.Range(0, StoneCount).Select(_ => random.Next(1, MaxStoneWeightExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LinearRescanEachSmash()
    {
        var remaining = new List<int>(_stones);

        while (remaining.Count > 1)
        {
            var firstIndex = IndexOfLargest(remaining, -1);
            var secondIndex = IndexOfLargest(remaining, firstIndex);
            var difference = remaining[firstIndex] - remaining[secondIndex];

            var higherIndex = Math.Max(firstIndex, secondIndex);
            remaining.RemoveAt(higherIndex);

            var lowerIndex = Math.Min(firstIndex, secondIndex);
            remaining.RemoveAt(lowerIndex);

            if (difference != 0)
            {
                remaining.Add(difference);
            }
        }

        return remaining.Count == 0 ? 0 : remaining[0];
    }

    private static int IndexOfLargest(List<int> values, int excludeIndex)
    {
        var best = -1;

        for (var i = 0; i < values.Count; i++)
        {
            if (i != excludeIndex && (best == -1 || values[i] > values[best]))
            {
                best = i;
            }
        }

        return best;
    }

    [Benchmark]
    public int MaxHeapSmash()
    {
        var heap = new Heap<int, MaxHeapOrder<int>>();

        foreach (var stone in _stones)
        {
            heap.Push(stone);
        }

        while (heap.Count > 1)
        {
            heap.TryPop(out var heaviest);
            heap.TryPop(out var second);

            if (heaviest != second)
            {
                heap.Push(heaviest - second);
            }
        }

        heap.TryPeek(out var remaining);
        return heap.Count == 0 ? 0 : remaining;
    }
}
