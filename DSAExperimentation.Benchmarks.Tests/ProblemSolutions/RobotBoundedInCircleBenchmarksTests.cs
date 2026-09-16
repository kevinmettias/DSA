using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RobotBoundedInCircleBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a hand-written switch per facing against this repo's own
// HashMap-backed step-delta table - so a harness whose arms disagree is timing two different
// problems. Setup draws the instruction string from one fixed seed, so the same Length must rebuild
// the same instructions; otherwise two published numbers were never comparable in the first place.
//
// Both arms answer a yes/no question over that string, so agreement is weaker than for a sequence
// answer: it would also hold if both arms always said the same thing for the wrong reason. The
// instruction stream is random over {G, L, R} and neither the class comment nor the fixture fixes
// which side of the question it lands on, so no decisive literal is available and the agreement is
// stated for what it is rather than dressed up.
public sealed partial class RobotBoundedInCircleBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsRobotBoundedByDirectionSwitch(),
            BuildHarness().IsRobotBoundedByDirectionSwitch());

    [Fact]
    public void IsRobotBoundedByDirectionSwitch_SeededInstructions_AgreesWithTheStepDeltaMap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsRobotBoundedByStepDeltaMap(), harness.IsRobotBoundedByDirectionSwitch());
    }

    [Fact]
    public void IsRobotBoundedByStepDeltaMap_SeededInstructions_AgreesWithTheDirectionSwitch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsRobotBoundedByDirectionSwitch(), harness.IsRobotBoundedByStepDeltaMap());
    }

    private static RobotBoundedInCircleBenchmarks BuildHarness()
    {
        var harness = new RobotBoundedInCircleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
