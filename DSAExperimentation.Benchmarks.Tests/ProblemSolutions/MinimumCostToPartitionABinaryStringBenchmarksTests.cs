using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToPartitionABinaryStringBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a split-in-half recursion that rescans the range it
// is standing on for its sensitive count against the same recursion reading that count off a prepared
// FenwickTree - so a harness whose arms disagree is timing two different strings. Both arms answer with
// a long minimum cost, which they compare directly. Setup draws the sensitive positions from one seeded
// stream, so the same Length must rebuild the same string and the same Fenwick tree.
public sealed partial class MinimumCostToPartitionABinaryStringBenchmarksTests
{
    private const int SmallestLength = 1024;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameString() =>
        Assert.Equal(BuildHarness().LinearScanRecursion(), BuildHarness().LinearScanRecursion());

    [Fact]
    public void LinearScanRecursion_SeededSensitivePositions_AgreesWithFenwickRangeSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickRangeSum(), harness.LinearScanRecursion());
    }

    [Fact]
    public void FenwickRangeSum_SeededSensitivePositions_AgreesWithLinearScanRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanRecursion(), harness.FenwickRangeSum());
    }

    private static MinimumCostToPartitionABinaryStringBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToPartitionABinaryStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
