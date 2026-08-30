using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Days to Eat N Oranges (LC 1553): plain unmemoized recursion over
// days(n) = n when n <= 1, else 1 + min(n % 2 + days(n / 2), n % 3 + days(n / 3)),
// vs. this repo's own Memoizer-driven version of the same recurrence - same DP
// composition IntegerReplacementBenchmarks/FibonacciNumberBenchmarks already use.
// The two subproblems (n/2, n/3) reconverge heavily across levels (e.g. n/2/3 and
// n/3/2 frequently land on the same value once floors are applied), so the
// unmemoized tree revisits the same states many times over; the memoized version
// only ever computes each distinct state once. N is int, not long: the problem's
// own stated upper bound (2*10^9) stays below int.MaxValue, so no overflow risk
// exists the way IntegerReplacement's n+1 branch has.
[MemoryDiagnoser]
public class MinimumNumberOfDaysToEatNOrangesBenchmarks
{
    [Params(100_000, 2_000_000_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => MinDays(N);

    private static int MinDays(int n)
        => n <= 1 ? n : 1 + Math.Min((n % 2) + MinDays(n / 2), (n % 3) + MinDays(n / 3));

    [Benchmark]
    public int MemoizedRecurrence()
        => Memoizer.Memoize<int, int>(N, (value, days) => value <= 1
            ? value
            : 1 + Math.Min((value % 2) + days(value / 2), (value % 3) + days(value / 3)));
}
