using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Best Time to Buy and Sell Stock with Cooldown (LC 309): plain un-memoized recursion
// over (day, holding) - exponential, since the same state recurs along many different
// buy/sell/rest paths - vs. this repo's own Memoizer<TState,TResult> caching that
// exact pair. Length is kept modest (<=28) specifically because the un-memoized
// baseline's blowup is real, the same reasoning FibonacciBenchmarks.cs's NaiveRecursive
// already documents.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockWithCooldownBenchmarks
{
    [Params(20, 28)]
    public int Length;

    private int[] _prices = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _prices = Enumerable.Range(0, Length).Select(_ => random.Next(0, 100)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => ProfitFrom(0, false);

    private int ProfitFrom(int day, bool holding)
    {
        if (day >= _prices.Length)
        {
            return 0;
        }

        if (holding)
        {
            var sell = _prices[day] + ProfitFrom(day + 2, false);
            var hold = ProfitFrom(day + 1, true);
            return Math.Max(sell, hold);
        }

        var buy = -_prices[day] + ProfitFrom(day + 1, true);
        var rest = ProfitFrom(day + 1, false);
        return Math.Max(buy, rest);
    }

    [Benchmark]
    public int MemoizedRecursion()
    {
        return Memoizer.Memoize<(int Day, bool Holding), int>((0, false), ProfitFromMemoized);

        int ProfitFromMemoized((int Day, bool Holding) state, Func<(int Day, bool Holding), int> profit)
        {
            var (day, holding) = state;
            if (day >= _prices.Length)
            {
                return 0;
            }

            if (holding)
            {
                var sell = _prices[day] + profit((day + 2, false));
                var hold = profit((day + 1, true));
                return Math.Max(sell, hold);
            }

            var buy = -_prices[day] + profit((day + 1, true));
            var rest = profit((day + 1, false));
            return Math.Max(buy, rest);
        }
    }
}
