using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Burst Balloons (LC 312): plain un-memoized interval recursion over (left, right)
// boundary pairs - exponential, since the same (left, right) sub-range recurs across
// many different choices of which balloon bursts last outside it - vs. this repo's
// own Memoizer<TState,TResult> caching that exact pair. BalloonCount is kept modest
// (<=14) specifically because the un-memoized baseline's blowup is real, the same
// reasoning FibonacciBenchmarks.cs's NaiveRecursive already documents.
[MemoryDiagnoser]
public class BurstBalloonsBenchmarks
{
    [Params(10, 14)]
    public int BalloonCount;

    private int[] _padded = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _padded = new int[BalloonCount + 2];
        _padded[0] = 1;
        _padded[^1] = 1;
        for (var i = 1; i <= BalloonCount; i++)
        {
            _padded[i] = random.Next(1, 100);
        }
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => CoinsBetween(0, _padded.Length - 1);

    private int CoinsBetween(int left, int right)
    {
        if (right - left <= 1)
        {
            return 0;
        }

        var best = 0;
        for (var last = left + 1; last < right; last++)
        {
            var gained = (_padded[left] * _padded[last] * _padded[right])
                + CoinsBetween(left, last) + CoinsBetween(last, right);
            best = Math.Max(best, gained);
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion()
    {
        return Memoizer.Memoize<(int Left, int Right), int>((0, _padded.Length - 1), CoinsBetweenMemoized);

        int CoinsBetweenMemoized((int Left, int Right) range, Func<(int Left, int Right), int> coins)
        {
            var (left, right) = range;
            if (right - left <= 1)
            {
                return 0;
            }

            var best = 0;
            for (var last = left + 1; last < right; last++)
            {
                var gained = (_padded[left] * _padded[last] * _padded[right])
                    + coins((left, last)) + coins((last, right));
                best = Math.Max(best, gained);
            }

            return best;
        }
    }
}
