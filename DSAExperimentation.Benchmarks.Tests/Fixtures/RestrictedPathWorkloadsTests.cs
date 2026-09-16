using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RestrictedPathWorkloads (ARCHITECTURE 17.7). The LC 1786 reading depends on
// the two routes being genuinely equal-cost: node u takes a one-step edge to u + 1 and a two-step
// edge to u + 2, each weighing exactly its own step distance, so the same downstream node is
// reached two ways at every layer and the un-memoized arm's recount really is the Fibonacci
// recurrence. NodeCount is what the harness sizes its Dijkstra labelling with, so it has to agree
// with the node ids BuildTwoStepEdges emits.
public sealed partial class RestrictedPathWorkloadsTests
{
    private const int StepCount = 20;
    private const int OneStepWidth = 1;
    private const int TwoStepWidth = 2;
    private const int FirstNode = 1; // this chain is 1-based, unlike most fixtures in this folder

    [Fact]
    public void NodeCount_StepCount_ReturnsOneMoreNodeThanSteps() =>
        Assert.Equal(StepCount + OneStepWidth, RestrictedPathWorkloads.NodeCount(StepCount));

    [Fact]
    public void BuildTwoStepEdges_EveryEdge_WeighsExactlyItsOwnStepDistance() =>
        Assert.All(
            RestrictedPathWorkloads.BuildTwoStepEdges(StepCount),
            edge => Assert.Equal(edge[1] - edge[0], edge[2]));

    // Every layer needs both routes: without the one-step edge the chain breaks, and without the
    // two-step edge there is no second route for the recount to find.
    [Fact]
    public void BuildTwoStepEdges_EveryNode_OffersBothItsOneStepAndItsTwoStepSuccessor()
    {
        var edges = RestrictedPathWorkloads.BuildTwoStepEdges(StepCount);

        for (var node = FirstNode; node <= StepCount; node++)
        {
            Assert.Contains(
                edges,
                edge => edge[0] == node && edge[1] == node + OneStepWidth && edge[2] == OneStepWidth);
        }

        for (var node = FirstNode; node <= StepCount - OneStepWidth; node++)
        {
            Assert.Contains(
                edges,
                edge => edge[0] == node && edge[1] == node + TwoStepWidth && edge[2] == TwoStepWidth);
        }
    }

    [Fact]
    public void BuildTwoStepEdges_EveryEdge_StaysInsideTheNodeCountTheSameStepCountReports() =>
        Assert.All(
            RestrictedPathWorkloads.BuildTwoStepEdges(StepCount),
            edge => Assert.InRange(edge[1], FirstNode, RestrictedPathWorkloads.NodeCount(StepCount)));

    [Fact]
    public void BuildTwoStepEdges_SameStepCount_ReturnsTheSameEdges() =>
        Assert.Equal(
            AnswerText.Of(RestrictedPathWorkloads.BuildTwoStepEdges(StepCount)),
            AnswerText.Of(RestrictedPathWorkloads.BuildTwoStepEdges(StepCount)));
}
