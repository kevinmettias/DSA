using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks (ARCHITECTURE 17.9): both
// arms count the restricted paths of the same graph - the unmemoized DFS against the memoized DagFold
// - so a harness whose arms disagree is timing two different questions. The count modulo 1e9+7 is the
// problem's whole answer rather than a proxy. Setup builds the graph and its Dijkstra labelling, so
// the same StepCount must rebuild the same labels; both arms then only read the labelled graph, which
// is why one harness can be called twice in either order.
public sealed partial class NumberOfRestrictedPathsFromFirstToLastNodeBenchmarksTests
{
    private const int SmallestStepCount = 20;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveDfs(), BuildHarness().NaiveDfs());

    [Fact]
    public void NaiveDfs_AgreesWithDagFoldMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DagFoldMemoized(), harness.NaiveDfs());
    }

    [Fact]
    public void DagFoldMemoized_AgreesWithNaiveDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveDfs(), harness.DagFoldMemoized());
    }

    private static NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks BuildHarness()
    {
        var harness = new NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks { StepCount = SmallestStepCount };
        harness.Setup();

        return harness;
    }
}
