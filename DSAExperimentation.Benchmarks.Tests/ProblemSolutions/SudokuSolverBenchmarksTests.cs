using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SudokuSolverBenchmarks (ARCHITECTURE 17.9): both arms are
// SudokuSolverSolution's, so a harness whose arms disagree is timing two different questions. The
// class carries no [Params] at all - the board is always the same 9x9 puzzle, so there is nothing to
// tune and the harness is a bare initializer plus Setup. Both arms answer with a bare bool and both
// mutate the board they are handed, but each arm clones the puzzle it works on inside the call, so
// one harness is safe to call twice in either order and the single-harness rule holds.
//
// The puzzle is LeetCode 37's own published example, which is solvable, so the verdict each arm is
// pinned to alongside the agreement is true - two arms that both failed to solve it would otherwise
// agree on false.
public sealed partial class SudokuSolverBenchmarksTests
{
    [Fact]
    public void Setup_FixedPuzzle_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().TrySolveBySpecializedRecursion(),
            BuildHarness().TrySolveBySpecializedRecursion());

    [Fact]
    public void TrySolveBySpecializedRecursion_LeetCodeExamplePuzzle_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrySolveByBacktrackEngine(), harness.TrySolveBySpecializedRecursion());
        Assert.True(harness.TrySolveBySpecializedRecursion());
    }

    [Fact]
    public void TrySolveByBacktrackEngine_LeetCodeExamplePuzzle_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TrySolveBySpecializedRecursion(), harness.TrySolveByBacktrackEngine());
        Assert.True(harness.TrySolveByBacktrackEngine());
    }

    private static SudokuSolverBenchmarks BuildHarness()
    {
        var harness = new SudokuSolverBenchmarks();
        harness.Setup();

        return harness;
    }
}
