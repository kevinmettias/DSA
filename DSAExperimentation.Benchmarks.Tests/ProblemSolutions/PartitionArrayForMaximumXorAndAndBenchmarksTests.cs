using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PartitionArrayForMaximumXorAndAndBenchmarks (ARCHITECTURE 17.9): its two
// arms are PartitionArrayForMaximumXorAndAndSolution's, competing maximisers for the same
// partition - the 3^n brute force against the subset XOR basis - so a harness whose arms
// disagree is timing two different problems. nums is the whole input, so Setup draws it from one
// fixed seed and the same ElementCount must rebuild the same array.
public sealed partial class PartitionArrayForMaximumXorAndAndBenchmarksTests
{
    private const int SmallestElementCount = 8;

    [Fact]
    public void Setup_SameElementCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SeededValues_AgreesWithSubsetXorBasis()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SubsetXorBasis(), harness.BruteForce());
    }

    [Fact]
    public void SubsetXorBasis_SeededValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.SubsetXorBasis());
    }

    private static PartitionArrayForMaximumXorAndAndBenchmarks BuildHarness()
    {
        var harness = new PartitionArrayForMaximumXorAndAndBenchmarks { ElementCount = SmallestElementCount };
        harness.Setup();

        return harness;
    }
}
