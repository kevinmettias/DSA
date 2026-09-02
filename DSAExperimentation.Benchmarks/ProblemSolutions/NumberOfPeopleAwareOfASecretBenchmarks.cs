using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of People Aware of a Secret (LC 2327): both benchmarks run the exact same
// dp[day] = "who first learns the secret on this day" recurrence, where dp[day]
// needs the sum of every dp[j] still actively sharing on that day (a window of
// roughly Forget - Delay earlier days). SlidingWindowSum re-derives that window sum
// from scratch on every day - O(n * windowSize), effectively O(n^2) since the
// window here is a fixed fraction of n. FenwickRangeSum instead runs the same
// recurrence against this repo's own FenwickTree<long,SumOperation<long>>, turning
// each day's window sum into one O(log n) Add/Query pair - O(n log n) overall.
[MemoryDiagnoser]
public class NumberOfPeopleAwareOfASecretBenchmarks
{
    private const long Mod = 1_000_000_007L;
    private const int DelayDivisor = 20;
    private const int ForgetDivisor = 2;

    [Params(2_000, 6_000)]
    public int Length;

    private int _delay;
    private int _forget;

    [GlobalSetup]
    public void Setup()
    {
        _delay = Math.Max(1, Length / DelayDivisor);
        _forget = Math.Max(_delay + 1, Length / ForgetDivisor);
    }

    [Benchmark(Baseline = true)]
    public long SlidingWindowSum()
    {
        var n = Length;
        var dp = new long[n];
        dp[0] = 1;

        for (var day = 2; day <= n; day++)
        {
            var low = Math.Max(1, day - _forget + 1);
            var high = day - _delay;

            if (high < low)
            {
                continue;
            }

            var sum = 0L;
            for (var j = low; j <= high; j++)
            {
                sum += dp[j - 1];
            }

            dp[day - 1] = sum % Mod;
        }

        var finalLow = Math.Max(1, n - _forget + 1);
        var total = 0L;
        for (var i = finalLow; i <= n; i++)
        {
            total += dp[i - 1];
        }

        return total % Mod;
    }

    [Benchmark]
    public long FenwickRangeSum()
    {
        var n = Length;
        var dp = new FenwickTree<long, SumOperation<long>>(n);
        dp.Add(0, 1);

        for (var day = 2; day <= n; day++)
        {
            var low = Math.Max(1, day - _forget + 1);
            var high = day - _delay;

            if (high >= low)
            {
                dp.Add(day - 1, dp.Query(low - 1, high - 1) % Mod);
            }
        }

        var finalLow = Math.Max(1, n - _forget + 1);
        return dp.Query(finalLow - 1, n - 1) % Mod;
    }
}
