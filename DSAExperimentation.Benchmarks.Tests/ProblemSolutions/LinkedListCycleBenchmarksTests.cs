using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListCycleBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - remembering every visited node against this repo's own Floyd
// detector - so a harness whose arms disagree is timing two different problems. Setup builds a
// Length-node chain whose tail rejoins its head, so every arm must walk the full cycle and answer
// true; the same Length must rebuild that same list, otherwise two published numbers were never
// comparable in the first place.
//
// A bool is the whole observable here, and this workload pins the verdict rather than merely leaving
// the two arms free to agree on false, so each test asserts the documented verdict as well as the
// agreement. The weaker half is reported with the batch: agreement alone would not catch both arms
// returning false for the same wrong reason - only the pinned verdict does that.
public sealed partial class LinkedListCycleBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedCycleDetected = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameCyclicList() =>
        Assert.Equal(BuildHarness().HasCycleByVisitedSet(), BuildHarness().HasCycleByVisitedSet());

    [Fact]
    public void HasCycleByFloydCycleDetection_TailRejoinsHead_AgreesWithVisitedSet()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedCycleDetected, harness.HasCycleByFloydCycleDetection());
        Assert.Equal(harness.HasCycleByVisitedSet(), harness.HasCycleByFloydCycleDetection());
    }

    [Fact]
    public void HasCycleByVisitedSet_TailRejoinsHead_AgreesWithFloydCycleDetection()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedCycleDetected, harness.HasCycleByVisitedSet());
        Assert.Equal(harness.HasCycleByFloydCycleDetection(), harness.HasCycleByVisitedSet());
    }

    private static LinkedListCycleBenchmarks BuildHarness()
    {
        var harness = new LinkedListCycleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
