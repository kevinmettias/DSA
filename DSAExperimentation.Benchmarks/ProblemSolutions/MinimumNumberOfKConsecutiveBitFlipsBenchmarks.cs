using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of K Consecutive Bit Flips (LC 995): the textbook approach that
// actually flips each k-length window in place (O(n*k) worst case) vs. a single
// left-to-right sweep tracking active-flip parity through this repo's own
// Queue<TElement> holding flip start indices - O(n), with the queue never growing
// past k entries.
[MemoryDiagnoser]
public class MinimumNumberOfKConsecutiveBitFlipsBenchmarks
{
    private const int K = 300;

    [Params(3_000, 30_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(2)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InPlaceWindowFlip()
    {
        var array = (int[])_nums.Clone();
        var n = array.Length;
        var flipCount = 0;

        for (var i = 0; i <= n - K; i++)
        {
            if (array[i] != 0)
            {
                continue;
            }

            for (var j = i; j < i + K; j++)
            {
                array[j] ^= 1;
            }

            flipCount++;
        }

        for (var i = 0; i < n; i++)
        {
            if (array[i] == 0)
            {
                return -1;
            }
        }

        return flipCount;
    }

    [Benchmark]
    public int QueueTrackedParity()
    {
        var activeFlips = new DSAExperimentation.DataStructures.Queue.Queue<int>();
        var flipCount = 0;

        for (var i = 0; i < _nums.Length; i++)
        {
            if (activeFlips.TryPeek(out var earliestStart) && earliestStart + K == i)
            {
                activeFlips.TryDequeue(out _);
            }

            var effectiveBit = _nums[i] ^ (activeFlips.Count % 2);

            if (effectiveBit == 0)
            {
                if (i + K > _nums.Length)
                {
                    return -1;
                }

                activeFlips.Enqueue(i);
                flipCount++;
            }
        }

        return flipCount;
    }
}
