using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BuildAMatrixWithConditionsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - repeatedly rescanning for a value whose conditions are
// met against a Kahn topological sort of both condition graphs - so a harness whose arms disagree is
// timing two different problems. Both arms report only the built matrix's row count, which the class
// comment ties to the value count whenever the conditions are satisfiable; agreement therefore
// witnesses that both strategies found the same (satisfiable, k x k) matrix and neither bailed out
// on a cycle, but not that they placed the same values - see the row-count narrowing reported with
// this batch. Setup builds the two condition graphs, so the same ValueCount must rebuild both.
public sealed partial class BuildAMatrixWithConditionsBenchmarksTests
{
    private const int SmallestValueCount = 50;

    [Fact]
    public void Setup_SameValueCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveRescanBothOrders(), BuildHarness().NaiveRescanBothOrders());

    [Fact]
    public void NaiveRescanBothOrders_AcyclicConditionDag_AgreesWithKahnsTopologicalSortBothOrders()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.KahnsTopologicalSortBothOrders(), harness.NaiveRescanBothOrders());
    }

    [Fact]
    public void KahnsTopologicalSortBothOrders_AcyclicConditionDag_AgreesWithNaiveRescanBothOrders()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRescanBothOrders(), harness.KahnsTopologicalSortBothOrders());
    }

    private static BuildAMatrixWithConditionsBenchmarks BuildHarness()
    {
        var harness = new BuildAMatrixWithConditionsBenchmarks { ValueCount = SmallestValueCount };
        harness.Setup();

        return harness;
    }
}
