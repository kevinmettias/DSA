using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BoatsToSavePeopleBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - repeatedly rescanning for the heaviest person who still fits
// against sorting once and walking two pointers inward - so a harness whose arms disagree is timing
// two different problems. Both arms read the same private limit, so the comparison also pins that
// the two strategies were handed the same boat capacity. Setup draws the weights from one fixed
// seed, so the same Length must rebuild the same workload.
public sealed partial class BoatsToSavePeopleBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRepeatedScan(), BuildHarness().BruteForceRepeatedScan());

    [Fact]
    public void BruteForceRepeatedScan_WeightsBelowTheLimit_AgreesWithSortThenTwoPointer()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortThenTwoPointer(), harness.BruteForceRepeatedScan());
    }

    [Fact]
    public void SortThenTwoPointer_SeededWeightRun_AgreesWithBruteForceRepeatedScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRepeatedScan(), harness.SortThenTwoPointer());
    }

    private static BoatsToSavePeopleBenchmarks BuildHarness()
    {
        var harness = new BoatsToSavePeopleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
