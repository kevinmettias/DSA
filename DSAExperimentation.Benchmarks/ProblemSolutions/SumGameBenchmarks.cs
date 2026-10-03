using DSAExperimentation.LeetCode.SumGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are SumGameSolution's, the same methods SumGameSolutionTests
// proves correct - the unmemoized minimax that re-explores every digit-fill order
// reaching the same reduced state, that recursion routed through this repo's own
// Memoizer, and the closed form the recursion collapses to. Each arm is handed the
// prepared SumGameState its hoisted overload takes, so reducing a board to
// (leftBlanks, rightBlanks, difference) is charged to [GlobalSetup] rather than to
// the search being measured.
//
// Sizes are per arm, counted in blanks per side. Branching is 10 digits per remaining
// blank, so the unmemoized tree is O(10^(2 * blanks)) and stops at 3 per side, in the
// millions of calls rather than billions. The memoized arm's (blanks, blanks,
// difference) states grow only polynomially and run on to 32 per side. The closed form
// is O(1) whatever the board, so it stays at the sizes every arm shares.
public class SumGameBenchmarks
{
    private Dictionary<int, SumGameState> _boardBySize = [];

    public static IEnumerable<int> BaselineSizes => [2, 3];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 8, 32];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _boardBySize = MemoizedSizes.ToDictionary(
            blanksPerSide => blanksPerSide,
            blanksPerSide => new SumGameState(blanksPerSide, blanksPerSide, SumDifference: 0));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public bool CanAliceWinByBruteForceRecursion(int blanksPerSide) =>
        SumGameSolution.CanAliceWinByBruteForceRecursion(_boardBySize[blanksPerSide]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public bool CanAliceWinByMemoizedRecursion(int blanksPerSide) =>
        SumGameSolution.CanAliceWinByMemoizedRecursion(_boardBySize[blanksPerSide]);

    [Benchmark]
    [ArgumentsSource(nameof(BaselineSizes))]
    public bool CanAliceWinByClosedForm(int blanksPerSide) =>
        SumGameSolution.CanAliceWinByClosedForm(_boardBySize[blanksPerSide]);
}
