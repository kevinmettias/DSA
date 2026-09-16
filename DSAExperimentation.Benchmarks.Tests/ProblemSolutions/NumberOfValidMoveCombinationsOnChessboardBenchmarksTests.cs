using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfValidMoveCombinationsOnChessboardBenchmarks (ARCHITECTURE 17.9): both
// arms count the arrangements of final positions in which no two pieces attack each other - the
// Cartesian product of every piece's move set validated afterwards against the pruned backtracking -
// so a harness whose arms disagree is timing two different questions. The arrangement count is the
// problem's whole answer rather than a proxy. Setup slices the fixed four-corner board down to the
// measured piece count, so the same PieceCount must select the same pieces and corners.
public sealed partial class NumberOfValidMoveCombinationsOnChessboardBenchmarksTests
{
    private const int SmallestPieceCount = 2;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().CartesianProductThenValidate(), BuildHarness().CartesianProductThenValidate());

    [Fact]
    public void CartesianProductThenValidate_AgreesWithPrunedBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrunedBacktracking(), harness.CartesianProductThenValidate());
    }

    [Fact]
    public void PrunedBacktracking_AgreesWithCartesianProductThenValidate()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CartesianProductThenValidate(), harness.PrunedBacktracking());
    }

    private static NumberOfValidMoveCombinationsOnChessboardBenchmarks BuildHarness()
    {
        var harness = new NumberOfValidMoveCombinationsOnChessboardBenchmarks { PieceCount = SmallestPieceCount };
        harness.Setup();

        return harness;
    }
}
