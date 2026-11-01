using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EvaluateDivisionBenchmarks (ARCHITECTURE 17.9): an expected value known from
// the workload's shape. Setup builds one chain of equal-ratio equations, so the query across it
// multiplies every edge, and the answer is that ratio raised to the number of edges. The answer is a
// double, so it is compared under a named tolerance rather than by exact equality.
public sealed partial class EvaluateDivisionBenchmarksTests
{
    private const int SmallestVariableCount = 6;
    private const double EdgeWeight = 2.0;
    private const int ChainEdgeCount = SmallestVariableCount - 1;

    // Every equation in Setup's chain carries the same ratio, so the query from the first variable
    // to the last is that ratio raised to the number of edges between them.
    private static readonly double ExpectedChainValue = Math.Pow(EdgeWeight, ChainEdgeCount);

    // The chain multiplies exactly-representable powers of two, so the value is expected to the
    // bit; the tolerance exists so a last-bit difference in the accumulation cannot fail the test
    // for the wrong reason.
    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void DictionaryBased_SixVariableChain_MultipliesEveryEdgeRatio() =>
        Assert.Equal(ExpectedChainValue, BuildHarness().DictionaryBased(), RelativeTolerance);

    private static EvaluateDivisionBenchmarks BuildHarness()
    {
        var harness = new EvaluateDivisionBenchmarks { VariableCount = SmallestVariableCount };
        harness.Setup();

        return harness;
    }
}
