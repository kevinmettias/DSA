using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfPointIsReachableBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - searching the reachable state space against the one
// closed-form gcd check - so a harness whose arms disagree is timing two different problems. Both
// arms answer with a bare bool, so agreement between them says the search and the closed form
// reached the same verdict on the same target.
public sealed partial class CheckIfPointIsReachableBenchmarksTests
{
    private const int SmallestLength = 50;

    // Setup targets (Length, Length + 1), two consecutive integers, whose gcd is 1 - a power of two
    // and therefore reachable, since the moves can strip every factor of two but never change the
    // odd part. The harness picks consecutive integers precisely so the target is always reachable
    // and neither arm may short-circuit on an early false; that is what makes a rebuilt target's
    // verdict decisive.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheConsecutiveIntegerTarget()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedVerdict, first.IsReachableByBruteForceBfs());
        Assert.Equal(ExpectedVerdict, second.IsReachableByGcd());
    }

    [Fact]
    public void IsReachableByBruteForceBfs_ConsecutiveTarget_AgreesWithGcd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsReachableByGcd(), harness.IsReachableByBruteForceBfs());
    }

    [Fact]
    public void IsReachableByGcd_ConsecutiveTarget_AgreesWithBruteForceBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsReachableByBruteForceBfs(), harness.IsReachableByGcd());
    }

    private static CheckIfPointIsReachableBenchmarks BuildHarness()
    {
        var harness = new CheckIfPointIsReachableBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
