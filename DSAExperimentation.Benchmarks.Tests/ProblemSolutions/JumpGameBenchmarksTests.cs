using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on the smallest workload, so this asserts only the verdict the fixture fixes
// independently of either arm.
//
// Setup draws every non-final jump length from [Length / MinJumpLengthDivisor, Length), so one
// hop from any index below the last advances by at least a quarter of the array. Four hops
// therefore reach the end from index 0 no matter which indices the seed picks, and the verdict is
// decisively true: neither arm can short-circuit its way out of the full scan, which is exactly
// the workload the class comment describes.
public sealed partial class JumpGameBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedVerdict = true;

    [Fact]
    public void CanJumpByForwardReachabilityDp_DeepJumpingArray_ReachesTheLastIndex() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().CanJumpByForwardReachabilityDp());

    private static JumpGameBenchmarks BuildHarness()
    {
        var harness = new JumpGameBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
