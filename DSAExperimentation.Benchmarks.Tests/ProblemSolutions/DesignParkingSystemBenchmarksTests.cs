using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignParkingSystemBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is a count derived from the script
// rather than from either arm: the script has to be accepted in full for both arms to do the
// identical per-call work the pairing is measuring.
public sealed partial class DesignParkingSystemBenchmarksTests
{
    private const int SmallestCalls = 100;

    // Each lot is seeded with Calls spaces and the script makes exactly Calls requests, all of them
    // for one of the three car types - so every AddCar has room and the replay accepts all of them. A
    // total short of Calls is what a script that ran out of spaces, or an arm that refused a type it
    // had room for, would leave behind.
    [Fact]
    public void ThreeFieldDispatch_SeededRequestScript_AcceptsEveryRequest() =>
        Assert.Equal(SmallestCalls, BuildHarness().ThreeFieldDispatch());

    private static DesignParkingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignParkingSystemBenchmarks { Calls = SmallestCalls };
        harness.Setup();

        return harness;
    }
}
