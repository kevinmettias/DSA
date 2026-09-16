using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SuperEggDropBenchmarks (ARCHITECTURE 17.9): both arms are SuperEggDropSolution's
// - the exhaustive scan of every candidate trial floor against the bisection on the same monotonic
// worst-case curve - so a harness whose arms disagree is timing two different questions. Both answer
// with a bare int, and the pair the class fixes (two eggs, the smaller of its two Floor counts) also
// has a textbook value: with two eggs the worst case is the smallest move count whose triangular
// number reaches the floor count, and 10 * 11 / 2 = 55 covers 50 floors while 9 * 10 / 2 = 45 does
// not. Each arm is pinned to that value next to the agreement.
//
// The class carries no [GlobalSetup]: Eggs and Floors are the whole workload.
public sealed partial class SuperEggDropBenchmarksTests
{
    private const int SmallestFloors = 50;
    private const int ExpectedMovesForFiftyFloors = 10;

    [Fact]
    public void LinearScanDp_TwoEggsAndFiftyFloors_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchDp(), harness.LinearScanDp());
        Assert.Equal(ExpectedMovesForFiftyFloors, harness.LinearScanDp());
    }

    [Fact]
    public void BinarySearchDp_TwoEggsAndFiftyFloors_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanDp(), harness.BinarySearchDp());
        Assert.Equal(ExpectedMovesForFiftyFloors, harness.BinarySearchDp());
    }

    private static SuperEggDropBenchmarks BuildHarness() => new() { Floors = SmallestFloors };
}
