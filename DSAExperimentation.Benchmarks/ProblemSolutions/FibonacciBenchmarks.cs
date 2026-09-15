using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Fibonacci Number (LC 509) / Climbing Stairs (LC 70) shape: three valid strategies
// for the exact same recurrence. NaiveRecursive is O(2^n) - N is kept modest (<=30)
// specifically because that blowup is real, not because the other two need it.
// TopDownMemoized dogfoods this repo's own Memoizer (a DP recurrence expressed as
// natural-looking recursion, cached underneath). IterativeConstantSpace is the
// O(n)-time O(1)-space answer a naive DP table wouldn't even need to beat.
[MemoryDiagnoser]
public class FibonacciBenchmarks
{
    private const int RecurrenceOrder = 2; // Fibonacci depends on the previous two terms

    [Params(20, 30)]
    public int N { get; set; }

    [Benchmark(Baseline = true)]
    public int NaiveRecursive() => Fib(N);

    [Benchmark]
    public int TopDownMemoized() => Memoizer.Memoize<int, int>(N, new MemoizedSumOfPreviousTerms());

    [Benchmark]
    public int IterativeConstantSpace()
    {
        if (N <= 1)
        {
            return N;
        }

        var (previous, current) = (0, 1);

        for (var i = RecurrenceOrder; i <= N; i++)
        {
            (previous, current) = (current, previous + current);
        }

        return current;
    }

    private static int Fib(int n) => n <= 1 ? n : SumOfPreviousTerms(n);

    // NaiveRecursive's two previous terms, added. It calls this only once its base
    // case no longer answers, so the recursion stays lazy.
    private static int SumOfPreviousTerms(int n) => Fib(n - 1) + Fib(n - RecurrenceOrder);

    // TopDownMemoized's recurrence, named: n is its two previous terms added, and the
    // two base cases are the whole of the rule. The memo run passes this
    // implementation back to itself, so no delegate is handed around.
    private sealed class MemoizedSumOfPreviousTerms : IRecurrence<int, int>
    {
        public int Replay(int state, IRecurrence<int, int> rest)
        {
            if (state <= 1)
            {
                return state;
            }

            return rest.Replay(state - 1, rest) + rest.Replay(state - RecurrenceOrder, rest);
        }
    }
}
