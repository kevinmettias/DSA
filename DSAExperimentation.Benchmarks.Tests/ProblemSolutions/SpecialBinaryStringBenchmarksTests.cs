using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SpecialBinaryStringBenchmarks (ARCHITECTURE 17.9): both arms are
// SpecialBinaryStringSolution's rearrangements of the same generated special binary string, so a
// harness whose arms disagree is timing two different problems. Setup rebuilds the generator from
// PairCount and its own fixed seed, so the same PairCount must rebuild the same string.
//
// Each arm returns the rearranged string itself, and the problem pins one answer rather than a
// set of them, so the whole-string comparison is the whole claim.
public sealed partial class SpecialBinaryStringBenchmarksTests
{
    private const int SmallestPairCount = 50;

    [Fact]
    public void Setup_SamePairCount_RebuildsTheSameString() =>
        Assert.Equal(BuildHarness().ArraySort(), BuildHarness().ArraySort());

    [Fact]
    public void ArraySort_FiftyPairs_AgreesWithMergeSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSort(), harness.ArraySort());
    }

    [Fact]
    public void MergeSort_FiftyPairs_AgreesWithArraySort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArraySort(), harness.MergeSort());
    }

    private static SpecialBinaryStringBenchmarks BuildHarness()
    {
        var harness = new SpecialBinaryStringBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
