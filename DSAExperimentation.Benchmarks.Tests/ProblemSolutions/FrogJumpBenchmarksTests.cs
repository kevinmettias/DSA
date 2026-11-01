using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FrogJumpBenchmarks (ARCHITECTURE 17.9): the generic arms check only compares
// the arms with each other, so this pins the baseline's verdict to the one the fixture guarantees.
// The stones are the consecutive run 0..StoneCount - 2 with one unreachable stone appended a fixed
// gap beyond it, so nothing can ever land on the last stone and the answer is false.
public sealed partial class FrogJumpBenchmarksTests
{
    private const int SmallestStoneCount = 10;
    private const bool ExpectedCrossingVerdict = false;

    [Fact]
    public void CanCrossByRecursiveBruteForce_UnreachableLastStone_CannotCross() =>
        Assert.Equal(ExpectedCrossingVerdict, BuildHarness().CanCrossByRecursiveBruteForce(SmallestStoneCount));

    private static FrogJumpBenchmarks BuildHarness()
    {
        var harness = new FrogJumpBenchmarks();
        harness.Setup();

        return harness;
    }
}
