using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HappyNumberBenchmarks (ARCHITECTURE 17.9): both arms are
// HappyNumberSolution's - the visited-set walk against Floyd tortoise-and-hare over the same
// digit-square chain - so a harness whose arms disagree is timing two different problems. The class
// carries no [GlobalSetup] and no parameters: the single input is the constant the class comment
// names as a member of the canonical non-happy cycle, so there is nothing to rebuild and no setup
// invariant to assert. Both arms answer with a bare bool, and the verdict is decisive from that
// comment rather than from either arm: a number on the non-happy cycle repeats a value before ever
// reaching 1, so both strategies must report false. Agreement between a bool and a bool is weak on
// its own - it witnesses only that neither arm late-exited on the wrong side of the cycle.
public sealed partial class HappyNumberBenchmarksTests
{
    // A number on the canonical non-happy cycle (4 -> 16 -> ... -> 4) never reaches 1.
    private const bool ExpectedVerdict = false;

    [Fact]
    public void IsHappyByVisitedSet_NonHappyCycleMember_AgreesWithFloydCycleDetection()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedVerdict, harness.IsHappyByVisitedSet());
        Assert.Equal(harness.IsHappyByFloydCycleDetection(), harness.IsHappyByVisitedSet());
    }

    [Fact]
    public void IsHappyByFloydCycleDetection_NonHappyCycleMember_AgreesWithVisitedSet()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedVerdict, harness.IsHappyByFloydCycleDetection());
        Assert.Equal(harness.IsHappyByVisitedSet(), harness.IsHappyByFloydCycleDetection());
    }

    private static HappyNumberBenchmarks BuildHarness() => new();
}
