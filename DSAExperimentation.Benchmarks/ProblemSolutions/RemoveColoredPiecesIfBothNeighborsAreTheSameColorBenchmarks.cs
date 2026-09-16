using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RemoveColoredPiecesIfBothNeighborsAreTheSameColor;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution's, the same methods
// RemoveColoredPiecesIfBothNeighborsAreTheSameColorTests proves correct - playing
// the game out move by move against recognizing that a run of length L always yields
// exactly max(L - 2, 0) moves and counting both budgets in one grouping pass.
[MemoryDiagnoser]
public class RemoveColoredPiecesIfBothNeighborsAreTheSameColorBenchmarks
{
    private const int ColorGroupCount = 2; private string _colors = "";

    // two color runs: Alice's then Bob's

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // One long run of Alice's color followed by one long run of Bob's maximizes
        // the total number of moves available to both players, forcing the
        // simulation through its full O(n^2) worst case instead of stopping after a
        // handful of turns.
        var half = Length / ColorGroupCount;

        _colors = new string('A', half) + new string('B', Length - half);
    }

    [Benchmark(Baseline = true)]
    public bool CanAliceWinByGameSimulation() =>
        RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution.CanAliceWinByGameSimulation(_colors);

    [Benchmark]
    public bool CanAliceWinByRunLengthCounting() =>
        RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution.CanAliceWinByRunLengthCounting(_colors);
}
