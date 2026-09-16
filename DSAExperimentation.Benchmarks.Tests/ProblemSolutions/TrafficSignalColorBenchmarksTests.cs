using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TrafficSignalColorBenchmarks (ARCHITECTURE 17.9): the class has a single
// arm - a direct three-way classification over LC 3894's own disjoint timer ranges - so there is no
// second strategy to reconcile it against and no [GlobalSetup] to rebuild. The assertion therefore
// has to come from what the class comment makes decisive: [Params] sweeps one representative timer
// value from each of LC's ranges (green, orange, red and invalid), and the state each of those
// ranges maps to is LeetCode's own published classification rather than a restatement of whatever
// the arm returns. The arm is handed each representative value here, so every one of the four
// ranges is checked rather than only the initializer's default.
public sealed partial class TrafficSignalColorBenchmarksTests
{
    private const int GreenTimer = 0;
    private const int OrangeTimer = 30;
    private const int RedTimer = 60;
    private const int InvalidTimer = 1_000;

    private const string ExpectedGreenState = "Green";
    private const string ExpectedOrangeState = "Orange";
    private const string ExpectedRedState = "Red";
    private const string ExpectedInvalidState = "Invalid";

    [Fact]
    public void RangeCheck_EachLeetCodeRange_ReturnsItsSignalState()
    {
        Assert.Equal(ExpectedGreenState, BuildHarness(GreenTimer).RangeCheck());
        Assert.Equal(ExpectedOrangeState, BuildHarness(OrangeTimer).RangeCheck());
        Assert.Equal(ExpectedRedState, BuildHarness(RedTimer).RangeCheck());
        Assert.Equal(ExpectedInvalidState, BuildHarness(InvalidTimer).RangeCheck());
    }

    private static TrafficSignalColorBenchmarks BuildHarness(int timer) => new() { Timer = timer };
}
