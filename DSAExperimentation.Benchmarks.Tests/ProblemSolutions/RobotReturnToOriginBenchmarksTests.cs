using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RobotReturnToOriginBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a hand-written switch per move character against a HashMap
// lookup of the same fixed step deltas - so a harness whose arms disagree is timing two different
// problems. Setup draws the move string from one fixed seed, so the same Length must rebuild the
// same string; otherwise two published numbers were never comparable in the first place.
//
// Both arms answer a yes/no question over that string, and the fixture's random {U, D, L, R} draws
// leave the robot away from the origin in general without the class comment fixing that, so
// agreement is the honest assertion here: it catches an arm that ever steps the wrong way, but it
// would also hold if both arms reported the same wrong verdict.
public sealed partial class RobotReturnToOriginBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsAtOriginBySwitch(),
            BuildHarness().IsAtOriginBySwitch());

    [Fact]
    public void IsAtOriginBySwitch_SeededMoves_AgreesWithTheHashMapLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsAtOriginByHashMapLookup(), harness.IsAtOriginBySwitch());
    }

    [Fact]
    public void IsAtOriginByHashMapLookup_SeededMoves_AgreesWithTheSwitch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsAtOriginBySwitch(), harness.IsAtOriginByHashMapLookup());
    }

    private static RobotReturnToOriginBenchmarks BuildHarness()
    {
        var harness = new RobotReturnToOriginBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
