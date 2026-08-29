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
    [Params(20, 30)]
    public int N;

    [Benchmark(Baseline = true)]
    public int NaiveRecursive() => Fib(N);

    private static int Fib(int n) => n <= 1 ? n : Fib(n - 1) + Fib(n - 2);

    [Benchmark]
    public int TopDownMemoized()
        => Memoizer.Memoize<int, int>(N, (n, fib) => n <= 1 ? n : fib(n - 1) + fib(n - 2));

    [Benchmark]
    public int IterativeConstantSpace()
    {
        if (N <= 1)
        {
            return N;
        }

        var (previous, current) = (0, 1);

        for (var i = 2; i <= N; i++)
        {
            (previous, current) = (current, previous + current);
        }

        return current;
    }
}
