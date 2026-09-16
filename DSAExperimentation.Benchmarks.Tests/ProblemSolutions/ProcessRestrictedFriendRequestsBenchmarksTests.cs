using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ProcessRestrictedFriendRequestsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - which requests in the stream get approved - so a harness
// whose arms disagree is timing two different problems. NodeCount is the only [Params] axis and Setup
// derives both the restrictions and the request stream from it, so the same NodeCount must rebuild
// the same workload.
public sealed partial class ProcessRestrictedFriendRequestsBenchmarksTests
{
    private const int SmallestNodeCount = 300;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameRequests() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().GraphReachabilityCheck()),
            AnswerText.Of(BuildHarness().GraphReachabilityCheck()));

    [Fact]
    public void GraphReachabilityCheck_AgreesWithDisjointSetUnionFind()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DisjointSetUnionFind()),
            AnswerText.Of(harness.GraphReachabilityCheck()));
    }

    [Fact]
    public void DisjointSetUnionFind_AgreesWithGraphReachabilityCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.GraphReachabilityCheck()),
            AnswerText.Of(harness.DisjointSetUnionFind()));
    }

    private static ProcessRestrictedFriendRequestsBenchmarks BuildHarness()
    {
        var harness = new ProcessRestrictedFriendRequestsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
