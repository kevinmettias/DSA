using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MakeArrayElementsEqualToZeroBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - replaying every candidate selection of triplets
// against a prefix-sum balance that scores the whole array in one pass - so a harness whose arms
// disagree is timing two different problems. Both arms return the selection count. Setup draws the
// values from one fixed seed and then pins nums[0] to zero for LC 3354's "at least one zero"
// precondition, so the same Length must rebuild the same array and the same count.
public sealed partial class MakeArrayElementsEqualToZeroBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().BruteForceSimulation(), BuildHarness().BruteForceSimulation());
        Assert.Equal(BuildHarness().PrefixSumBalance(), BuildHarness().PrefixSumBalance());
    }

    [Fact]
    public void BruteForceSimulation_SeededZerosAndSmallValues_AgreesWithPrefixSumBalance()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrefixSumBalance(), harness.BruteForceSimulation());
    }

    [Fact]
    public void PrefixSumBalance_SeededZerosAndSmallValues_AgreesWithBruteForceSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSimulation(), harness.PrefixSumBalance());
    }

    private static MakeArrayElementsEqualToZeroBenchmarks BuildHarness()
    {
        var harness = new MakeArrayElementsEqualToZeroBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
