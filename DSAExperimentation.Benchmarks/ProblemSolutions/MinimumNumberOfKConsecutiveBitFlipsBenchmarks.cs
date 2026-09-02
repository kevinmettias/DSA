using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

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

    private const int BitValueUpperBound = 2;

    private const int ParityModulus = 2;

    [Params(3_000, 30_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(BitValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int InPlaceWindowFlip()
    {
        var array = (int[])_nums.Clone();
        var flipCount = FlipWindows(array);

        return AllOnes(array) ? flipCount : -1;
    }

    private static int FlipWindows(int[] array)
    {
        var flipCount = 0;

        for (var i = 0; i <= array.Length - K; i++)
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

        return flipCount;
    }

    private static bool AllOnes(int[] array)
    {
        foreach (var value in array)
        {
            if (value == 0)
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public int QueueTrackedParity()
    {
        var activeFlips = new RepoQueue();

        return ProcessBits(activeFlips);
    }

    private int ProcessBits(RepoQueue activeFlips)
    {
        var flipCount = 0;

        for (var i = 0; i < _nums.Length; i++)
        {
            var outcome = ProcessIndex(activeFlips, i);

            if (outcome.Impossible)
            {
                return -1;
            }

            if (outcome.Flipped)
            {
                flipCount++;
            }
        }

        return flipCount;
    }

    private (bool Flipped, bool Impossible) ProcessIndex(RepoQueue activeFlips, int i)
    {
        if (activeFlips.TryPeek(out var earliestStart) && earliestStart + K == i)
        {
            activeFlips.TryDequeue(out _);
        }

        var effectiveBit = _nums[i] ^ (activeFlips.Count % ParityModulus);

        if (effectiveBit != 0)
        {
            return (Flipped: false, Impossible: false);
        }

        if (i + K > _nums.Length)
        {
            return (Flipped: false, Impossible: true);
        }

        activeFlips.Enqueue(i);
        return (Flipped: true, Impossible: false);
    }
}
