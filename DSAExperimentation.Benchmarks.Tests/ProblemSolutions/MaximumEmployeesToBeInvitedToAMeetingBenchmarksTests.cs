using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumEmployeesToBeInvitedToAMeetingBenchmarks (ARCHITECTURE 17.9): both arms
// are competing strategies for one question - the largest group that can be seated such that everyone
// sits beside their favourite - so a harness whose arms disagree is timing two different problems.
// Setup builds the favourite array deterministically (mutual pairs, one long cycle, then seeded
// chains) and folds it into the prepared EmployeeGraph the composed arm is handed, so the same node
// count must rebuild the same graph; neither arm mutates either structure.
public sealed partial class MaximumEmployeesToBeInvitedToAMeetingBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ManualPeelAndCycleWalk(), BuildHarness().ManualPeelAndCycleWalk());

    [Fact]
    public void ManualPeelAndCycleWalk_AgreesWithGraphPrimitiveComposition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualPeelAndCycleWalk(), harness.GraphPrimitiveComposition());
    }

    [Fact]
    public void GraphPrimitiveComposition_AgreesWithManualPeelAndCycleWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GraphPrimitiveComposition(), harness.ManualPeelAndCycleWalk());
    }

    private static MaximumEmployeesToBeInvitedToAMeetingBenchmarks BuildHarness()
    {
        var harness = new MaximumEmployeesToBeInvitedToAMeetingBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
