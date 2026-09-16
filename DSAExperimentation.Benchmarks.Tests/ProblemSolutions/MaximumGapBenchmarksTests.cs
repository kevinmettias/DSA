using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumGapBenchmarks (ARCHITECTURE 17.9): both arms are competing strategies
// for one question - the largest gap between two successive values once the array is sorted - so a
// harness whose arms disagree is timing two different problems. Setup draws the values from the
// fixture's fixed seed, so the same length must rebuild the same workload; neither arm mutates it.
public sealed partial class MaximumGapBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().SelectionSort(), BuildHarness().SelectionSort());

    [Fact]
    public void SelectionSort_AgreesWithMergeSortScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SelectionSort(), harness.MergeSortScan());
    }

    [Fact]
    public void MergeSortScan_AgreesWithSelectionSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortScan(), harness.SelectionSort());
    }

    private static MaximumGapBenchmarks BuildHarness()
    {
        var harness = new MaximumGapBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
