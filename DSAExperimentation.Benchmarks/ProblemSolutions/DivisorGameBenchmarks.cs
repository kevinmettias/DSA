using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Divisor Game (LC 1025): the O(n^2) memoized game-theory recursion scanning every
// divisor x of n - via this repo's own Memoizer, no hand-rolled cache, the same
// pairing NimGameBenchmarks already uses - vs. the closed-form O(1) "n is even"
// formula the recursion itself reduces to.
[MemoryDiagnoser]
public class DivisorGameBenchmarks
{
    [Params(100, 1_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public bool MemoizedRecursion()
        => Memoizer.Memoize<int, bool>(N, (current, aliceWins) =>
        {
            for (var x = 1; x < current; x++)
            {
                if (current % x == 0 && !aliceWins(current - x))
                {
                    return true;
                }
            }

            return false;
        });

    [Benchmark]
    public bool ParityFormula() => N % 2 == 0;
}
