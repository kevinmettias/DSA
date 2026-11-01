using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SuperEggDropBenchmarks (ARCHITECTURE 17.9): both arms answer with a bare int,
// and the pair the class fixes (two eggs, the smaller of its two Floor counts) has a textbook value:
// with two eggs the worst case is the smallest move count whose triangular number reaches the floor
// count, and 10 * 11 / 2 = 55 covers 50 floors while 9 * 10 / 2 = 45 does not. The baseline is
// pinned to that value; the generic arm check holds the bisection arm to the baseline's answer.
//
// The class carries no [GlobalSetup]: Eggs and Floors are the whole workload.
public sealed partial class SuperEggDropBenchmarksTests
{
    private const int SmallestFloors = 50;
    private const int ExpectedMovesForFiftyFloors = 10;

    [Fact]
    public void LinearScanDp_TwoEggsAndFiftyFloors_NeedsTheTriangularMoveCount() =>
        Assert.Equal(ExpectedMovesForFiftyFloors, BuildHarness().LinearScanDp());

    private static SuperEggDropBenchmarks BuildHarness() => new() { Floors = SmallestFloors };
}
