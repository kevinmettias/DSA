using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Nim Game (LC 292): the textbook O(n) memoized game-theory recursion - via this
// repo's own Memoizer, no hand-rolled cache - vs. the closed-form O(1)
// n % 4 != 0 formula the recursion itself reduces to.
[MemoryDiagnoser]
public class NimGameBenchmarks
{
    // LC 292 lets a turn remove 1, 2, or 3 stones; the losing positions are
    // exactly the multiples of (MaxStonesPerTurn + 1).
    private const int TwoStoneRemoval = 2;
    private const int ThreeStoneRemoval = 3;
    private const int LosingPositionModulus = 4;

    [Params(20, 1_000)]
    public int Stones;

    [Benchmark(Baseline = true)]
    public bool MemoizedRecursion()
        => Memoizer.Memoize<int, bool>(Stones, (stones, canWin) => stones switch
        {
            <= 0 => false,
            _ => !canWin(stones - 1)
                || (stones >= TwoStoneRemoval && !canWin(stones - TwoStoneRemoval))
                || (stones >= ThreeStoneRemoval && !canWin(stones - ThreeStoneRemoval)),
        });

    [Benchmark]
    public bool ModuloFormula() => Stones % LosingPositionModulus != 0;
}
