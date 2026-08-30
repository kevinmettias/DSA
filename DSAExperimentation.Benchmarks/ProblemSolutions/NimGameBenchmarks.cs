using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Nim Game (LC 292): the textbook O(n) memoized game-theory recursion - via this
// repo's own Memoizer, no hand-rolled cache - vs. the closed-form O(1)
// n % 4 != 0 formula the recursion itself reduces to.
[MemoryDiagnoser]
public class NimGameBenchmarks
{
    [Params(20, 1_000)]
    public int Stones;

    [Benchmark(Baseline = true)]
    public bool MemoizedRecursion()
        => Memoizer.Memoize<int, bool>(Stones, (stones, canWin) => stones switch
        {
            <= 0 => false,
            _ => !canWin(stones - 1)
                || (stones >= 2 && !canWin(stones - 2))
                || (stones >= 3 && !canWin(stones - 3)),
        });

    [Benchmark]
    public bool ModuloFormula() => Stones % 4 != 0;
}
