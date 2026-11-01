using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfPointIsReachableBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt target, so this pins the baseline to the verdict
// the target's shape decides, derived independently of either arm.
public sealed partial class CheckIfPointIsReachableBenchmarksTests
{
    private const int SmallestLength = 50;

    // Setup targets (Length, Length + 1), two consecutive integers, whose gcd is 1 - a power of two
    // and therefore reachable, since the moves can strip every factor of two but never change the
    // odd part. The harness picks consecutive integers precisely so the target is always reachable
    // and neither arm may short-circuit on an early false.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void IsReachableByBruteForceBfs_ConsecutiveIntegerTarget_ReturnsTrue() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().IsReachableByBruteForceBfs());

    private static CheckIfPointIsReachableBenchmarks BuildHarness()
    {
        var harness = new CheckIfPointIsReachableBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
