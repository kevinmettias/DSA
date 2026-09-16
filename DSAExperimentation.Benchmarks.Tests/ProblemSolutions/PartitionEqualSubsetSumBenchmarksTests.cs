using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionEqualSubsetSumBenchmarks (ARCHITECTURE 17.9): its two arms are
// PartitionEqualSubsetSumSolution's, competing searches for the same yes/no question - bottom-up
// tabulation against top-down memoization - so a harness whose arms disagree is timing two
// different problems. Both arms read the same half-sum private to the harness, so the comparison
// also pins that Setup handed the two strategies the same precomputed target. nums is drawn from
// one fixed seed, so the same Length must rebuild the same array and the same half.
public sealed partial class PartitionEqualSubsetSumBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().CanPartitionByTabulation(), BuildHarness().CanPartitionByTabulation());

    [Fact]
    public void CanPartitionByTabulation_SeededHalfSum_AgreesWithCanPartitionByMemoization()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanPartitionByMemoization(), harness.CanPartitionByTabulation());
    }

    [Fact]
    public void CanPartitionByMemoization_SeededHalfSum_AgreesWithCanPartitionByTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanPartitionByTabulation(), harness.CanPartitionByMemoization());
    }

    private static PartitionEqualSubsetSumBenchmarks BuildHarness()
    {
        var harness = new PartitionEqualSubsetSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
