using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - can index 0 reach the last index - so a harness whose arms
// disagree is timing two different problems.
//
// Setup draws every non-final jump length from [Length / MinJumpLengthDivisor, Length), so one
// hop from any index below the last advances by at least a quarter of the array. Four hops
// therefore reach the end from index 0 no matter which indices the seed picks, and the verdict
// the arms have to agree on is decisively true: neither arm can short-circuit its way out of
// the full scan, which is exactly the workload the class comment describes.
public sealed partial class JumpGameBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheDeepJumpingArray()
    {
        Assert.Equal(ExpectedVerdict, BuildHarness().CanJumpByForwardReachabilityDp());
        Assert.Equal(ExpectedVerdict, BuildHarness().CanJumpByGreedyFarthestReach());
    }

    [Fact]
    public void CanJumpByForwardReachabilityDp_DeepJumpingArray_AgreesWithGreedyFarthestReach()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanJumpByGreedyFarthestReach(), harness.CanJumpByForwardReachabilityDp());
    }

    [Fact]
    public void CanJumpByGreedyFarthestReach_DeepJumpingArray_AgreesWithForwardReachabilityDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanJumpByForwardReachabilityDp(), harness.CanJumpByGreedyFarthestReach());
    }

    private static JumpGameBenchmarks BuildHarness()
    {
        var harness = new JumpGameBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
