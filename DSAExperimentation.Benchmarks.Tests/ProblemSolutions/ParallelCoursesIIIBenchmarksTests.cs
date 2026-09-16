using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ParallelCoursesIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// ParallelCoursesIIISolution's, competing orderings for the same critical-path DP - Kahn's
// topological order, relaxing every edge once, against repeated whole-graph relaxation rounds -
// so a harness whose arms disagree is timing two different problems. Setup builds the seeded
// DAG, so the same NodeCount must rebuild the same graph, and both arms are handed that same
// prepared node list rather than each building its own.
public sealed partial class ParallelCoursesIIIBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RepeatedRelaxation(), BuildHarness().RepeatedRelaxation());

    [Fact]
    public void KahnsTopologicalSortDp_SeededDag_AgreesWithRepeatedRelaxation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedRelaxation(), harness.KahnsTopologicalSortDp());
    }

    [Fact]
    public void RepeatedRelaxation_SeededDag_AgreesWithKahnsTopologicalSortDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.KahnsTopologicalSortDp(), harness.RepeatedRelaxation());
    }

    private static ParallelCoursesIIIBenchmarks BuildHarness()
    {
        var harness = new ParallelCoursesIIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
