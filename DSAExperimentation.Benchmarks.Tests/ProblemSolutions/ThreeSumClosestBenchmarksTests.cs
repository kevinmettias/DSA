using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ThreeSumClosestBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - cubic exhaustive scan against MergeSort plus a linear
// two-pointer sweep per fixed first element - so a harness whose arms disagree is timing two
// different problems. Both arms return the closest achievable sum as an int, so they are compared
// directly. Setup draws the values from a fixed seed, so the same Length must rebuild the same
// workload.
public sealed partial class ThreeSumClosestBenchmarksTests
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

    private static ThreeSumClosestBenchmarks BuildHarness()
    {
        var harness = new ThreeSumClosestBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
