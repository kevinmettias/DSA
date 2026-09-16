using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EvaluateDivisionBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a plain dictionary-backed DFS against this repo's
// HashMap/Stack composition - so a harness whose arms disagree is evaluating two different chains.
// Setup builds one chain of equal-ratio equations, so the query across it multiplies every edge;
// the same VariableCount must therefore rebuild both endpoints and the ratio between them. Both
// arms return a double and the answer is that ratio raised to a large power, so the arms are
// compared under a named relative tolerance rather than by exact equality.
public sealed partial class EvaluateDivisionBenchmarksTests
{
    private const int SmallestVariableCount = 200;
    private const double EdgeWeight = 2.0;
    private const int ChainEdgeCount = SmallestVariableCount - 1;

    // Every equation in Setup's chain carries the same ratio, so the query from the first variable
    // to the last is that ratio raised to the number of edges between them.
    private static readonly double ExpectedChainValue = Math.Pow(EdgeWeight, ChainEdgeCount);

    // Both arms multiply the same exactly-representable powers of two, so agreement is expected to
    // the bit; the tolerance exists so a last-bit difference in the two accumulations cannot fail
    // the harness for the wrong reason.
    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void Setup_SameVariableCount_RebuildsTheSameChain()
    {
        Assert.Equal(ExpectedChainValue, BuildHarness().DictionaryBased(), RelativeTolerance);

        Assert.Equal(BuildHarness().DictionaryBased(), BuildHarness().DictionaryBased(), RelativeTolerance);
    }

    [Fact]
    public void DictionaryBased_TwoHundredVariableChain_AgreesWithHashMapStackComposed()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapStackComposed(), harness.DictionaryBased(), RelativeTolerance);
    }

    [Fact]
    public void HashMapStackComposed_TwoHundredVariableChain_AgreesWithDictionaryBased()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryBased(), harness.HashMapStackComposed(), RelativeTolerance);
    }

    private static EvaluateDivisionBenchmarks BuildHarness()
    {
        var harness = new EvaluateDivisionBenchmarks { VariableCount = SmallestVariableCount };
        harness.Setup();

        return harness;
    }
}
