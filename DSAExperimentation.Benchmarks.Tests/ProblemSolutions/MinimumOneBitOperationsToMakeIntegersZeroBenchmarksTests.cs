using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumOneBitOperationsToMakeIntegersZeroBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the fewest bit operations that turn the
// target into zero - so a harness whose arms disagree is timing two different problems. The input is
// LeetCode's own single integer and the class has no [GlobalSetup] to prepare, so the tuned Target is
// the whole workload: the search arm walks a number of states proportional to the answer the closed
// form computes directly, and agreement is what ties those two figures together.
public sealed partial class MinimumOneBitOperationsToMakeIntegersZeroBenchmarksTests
{
    private const int SmallestTarget = 2_000;

    [Fact]
    public void BreadthFirstSearch_SameTarget_AgreesWithInverseGrayCodeFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InverseGrayCodeFormula(), harness.BreadthFirstSearch());
    }

    [Fact]
    public void InverseGrayCodeFormula_SameTarget_AgreesWithBreadthFirstSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BreadthFirstSearch(), harness.InverseGrayCodeFormula());
    }

    private static MinimumOneBitOperationsToMakeIntegersZeroBenchmarks BuildHarness() =>
        new() { Target = SmallestTarget };
}
