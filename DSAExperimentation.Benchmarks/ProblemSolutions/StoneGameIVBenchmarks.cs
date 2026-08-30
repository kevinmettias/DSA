using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stone Game IV (LC 1510): plain un-memoized minimax recursion over the
// remaining stone count - exponential, since the same remaining count recurs
// through many different perfect-square-removal sequences reaching it - vs.
// this repo's own Memoizer<TState,TResult> caching that count, the identical
// shape DivisorGameBenchmarks/StoneGameIIIBenchmarks already use. StoneCount
// is kept modest for the same reason those benchmarks document: the
// un-memoized baseline's blowup is real (branching factor sqrt(StoneCount),
// wider than DivisorGame's or StoneGameIII's own branching).
[MemoryDiagnoser]
public class StoneGameIVBenchmarks
{
    [Params(16, 20)]
    public int StoneCount;

    [Benchmark(Baseline = true)]
    public bool UnmemoizedRecursion() => AliceWins(StoneCount);

    private static bool AliceWins(int remaining)
    {
        for (var square = 1; square * square <= remaining; square++)
        {
            if (!AliceWins(remaining - square * square))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool MemoizedRecursion()
        => Memoizer.Memoize<int, bool>(StoneCount, (current, aliceWins) =>
        {
            for (var square = 1; square * square <= current; square++)
            {
                if (!aliceWins(current - square * square))
                {
                    return true;
                }
            }

            return false;
        });
}
