using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumNumberOfDaysToEatNOranges;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumNumberOfDaysToEatNOrangesSolution's, the same
// methods MinimumNumberOfDaysToEatNOrangesTests proves correct. The two subproblems
// (n/2, n/3) reconverge heavily across levels (e.g. n/2/3 and n/3/2 frequently land
// on the same value once floors are applied), so the unmemoized arm revisits the
// same states many times over while the memoized arm only ever computes each
// distinct state once - this recurrence's actual reconvergence-driven gap. N is int,
// not long: the problem's own stated upper bound (2*10^9) stays below int.MaxValue.
[MemoryDiagnoser]
public class MinimumNumberOfDaysToEatNOrangesBenchmarks
{
    [Params(100_000, 2_000_000_000)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => MinimumNumberOfDaysToEatNOrangesSolution.MinDaysByUnmemoizedRecursion(N);

    [Benchmark]
    public int MemoizedRecurrence() => MinimumNumberOfDaysToEatNOrangesSolution.MinDaysByMemoizedRecurrence(N);
}
