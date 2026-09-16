using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are SumGameSolution's, the same methods SumGameTests
// proves correct - the unmemoized minimax that re-explores every digit-fill order
// reaching the same reduced state, that recursion routed through this repo's own
// Memoizer, and the closed form the recursion collapses to. Each arm is handed the
// prepared SumGameState its hoisted overload takes, so reducing a board to
// (leftBlanks, rightBlanks, difference) is charged to [GlobalSetup] rather than to
// the search being measured.
[MemoryDiagnoser]
public class SumGameBenchmarks
{
    private SumGameState _board;

    // Kept modest: branching is 10 digits per remaining blank, so the unmemoized
    // tree is already O(10^(2 * BlanksPerSide)) - 4 and 6 total blanks keep
    // CanAliceWinByBruteForceRecursion in the thousands-to-millions of calls, not
    // billions.
    [Params(2, 3)]
    public int BlanksPerSide { get; set; }

    [GlobalSetup]
    public void Setup() => _board = new SumGameState(BlanksPerSide, BlanksPerSide, SumDifference: 0);

    [Benchmark(Baseline = true)]
    public bool CanAliceWinByBruteForceRecursion() => SumGameSolution.CanAliceWinByBruteForceRecursion(_board);

    [Benchmark]
    public bool CanAliceWinByMemoizedRecursion() => SumGameSolution.CanAliceWinByMemoizedRecursion(_board);

    [Benchmark]
    public bool CanAliceWinByClosedForm() => SumGameSolution.CanAliceWinByClosedForm(_board);
}
