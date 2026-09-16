using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignParkingSystemBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - three raw fields with an if/else dispatch against
// this repo's own HashMap keyed by car type - so a harness whose arms disagree is timing two
// different problems. Setup draws the request script from one fixed seed, so the same Calls must
// rebuild the same script, and that script has to be accepted in full for both arms to do the
// identical per-call work the pairing is measuring.
public sealed partial class DesignParkingSystemBenchmarksTests
{
    private const int SmallestCalls = 1_000;

    [Fact]
    public void Setup_SameCallCount_RebuildsTheSameRequestScript()
    {
        // Each lot is seeded with Calls spaces and the script makes exactly Calls requests, all of
        // them for one of the three car types - so every AddCar has room and the replay accepts
        // all of them. A total short of Calls is what a script that ran out of spaces, or an arm
        // that refused a type it had room for, would leave behind.
        Assert.Equal(SmallestCalls, BuildHarness().ThreeFieldDispatch());
        Assert.Equal(BuildHarness().ThreeFieldDispatch(), BuildHarness().ThreeFieldDispatch());
    }

    [Fact]
    public void ThreeFieldDispatch_SeededRequestScript_AgreesWithHashMapDispatch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapDispatch(), harness.ThreeFieldDispatch());
    }

    [Fact]
    public void HashMapDispatch_SeededRequestScript_AgreesWithThreeFieldDispatch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ThreeFieldDispatch(), harness.HashMapDispatch());
    }

    private static DesignParkingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignParkingSystemBenchmarks { Calls = SmallestCalls };
        harness.Setup();

        return harness;
    }
}
