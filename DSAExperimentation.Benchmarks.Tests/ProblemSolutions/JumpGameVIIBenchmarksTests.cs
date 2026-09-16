using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameVIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - can index 0 reach the last index of the binary string - so a
// harness whose arms disagree is timing two different problems.
//
// Setup fills the string with '0' except for a blocked '1' in the final position, and LC 1871
// only allows landing on a '0'. The last index is therefore unreachable and the verdict the arms
// have to agree on is decisively false - which is exactly the class comment's point: neither arm
// can short-circuit on an early success, so both must exhaust every reachable index before
// answering, and the unmemoized recursion's call-count blowup is real work rather than a bailout.
public sealed partial class JumpGameVIIBenchmarksTests
{
    private const int SmallestStringLength = 28;
    private const bool ExpectedVerdict = false;

    [Fact]
    public void Setup_SameStringLength_RebuildsTheBlockedFinalCharacter()
    {
        Assert.Equal(ExpectedVerdict, BuildHarness().CanReachByUnmemoizedRecursion());
        Assert.Equal(ExpectedVerdict, BuildHarness().CanReachByVisitedTrackingTraversal());
    }

    [Fact]
    public void CanReachByUnmemoizedRecursion_BlockedFinalCharacter_AgreesWithVisitedTrackingTraversal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanReachByVisitedTrackingTraversal(), harness.CanReachByUnmemoizedRecursion());
    }

    [Fact]
    public void CanReachByVisitedTrackingTraversal_BlockedFinalCharacter_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanReachByUnmemoizedRecursion(), harness.CanReachByVisitedTrackingTraversal());
    }

    private static JumpGameVIIBenchmarks BuildHarness()
    {
        var harness = new JumpGameVIIBenchmarks { StringLength = SmallestStringLength };
        harness.Setup();

        return harness;
    }
}
