using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ThreeSumWithMultiplicityBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - cubic triple-loop counting against MergeSort plus
// the sorted two-pointer sweep that counts each equal-valued span's combinations directly - so a
// harness whose arms disagree is timing two different problems. Both arms return the triplet count
// as an int, so they are compared directly. Setup bounds the values to [0, 100] from a fixed seed,
// the range that forces genuine multiplicities; the same Length must rebuild the same workload.
public sealed partial class ThreeSumWithMultiplicityBenchmarksTests
{
    private const int SmallestLength = 80;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithMergeSortTwoPointers()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortTwoPointers(), harness.BruteForce());
    }

    [Fact]
    public void MergeSortTwoPointers_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MergeSortTwoPointers());
    }

    private static ThreeSumWithMultiplicityBenchmarks BuildHarness()
    {
        var harness = new ThreeSumWithMultiplicityBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
