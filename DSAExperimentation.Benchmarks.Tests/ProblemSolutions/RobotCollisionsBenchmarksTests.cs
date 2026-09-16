using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RobotCollisionsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - resolving one adjacent colliding pair per full rescan from the
// front against a single left-to-right sweep over this repo's own Stack<int> - so a harness whose
// arms disagree is timing two different problems. Setup draws all three arrays from one fixed seed,
// so the same Length must rebuild the same workload; otherwise two published numbers were never
// comparable in the first place.
//
// Both arms sort their own copy of the positions and read the hoisted arrays without writing to
// them, so one harness is safe to call twice in either order and the single-harness rule holds. The
// survivor set is a genuine function of the random positions, healths and directions, so it is
// reconciled between the arms rather than against a re-derivation of which robots survive.
public sealed partial class RobotCollisionsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RepeatedScan()),
            AnswerText.Of(BuildHarness().RepeatedScan()));

    [Fact]
    public void RepeatedScan_SeededRobots_AgreesWithStackSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StackSimulation()), AnswerText.Of(harness.RepeatedScan()));
    }

    [Fact]
    public void StackSimulation_SeededRobots_AgreesWithRepeatedScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.RepeatedScan()), AnswerText.Of(harness.StackSimulation()));
    }

    private static RobotCollisionsBenchmarks BuildHarness()
    {
        var harness = new RobotCollisionsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
