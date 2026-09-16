using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumTotalBeautyOfTheGardensBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the linear scan of the incomplete prefix plus the
// linear height descent against this repo's MergeSort and BinarySearch.LowerBound - so a harness
// whose arms disagree is timing two different problems. Setup draws the flowers from one fixed seed
// and derives the target and the scarce flower budget from the length, so the same Length must
// rebuild the same workload; otherwise two published numbers were never comparable in the first
// place.
public sealed partial class MaximumTotalBeautyOfTheGardensBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearSearchOnAnswer(), BuildHarness().LinearSearchOnAnswer());

    [Fact]
    public void LinearSearchOnAnswer_ScarceFlowerBudget_AgreesWithSortAndBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortAndBinarySearch(), harness.LinearSearchOnAnswer());
    }

    [Fact]
    public void SortAndBinarySearch_ScarceFlowerBudget_AgreesWithLinearSearchOnAnswer()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearSearchOnAnswer(), harness.SortAndBinarySearch());
    }

    private static MaximumTotalBeautyOfTheGardensBenchmarks BuildHarness()
    {
        var harness = new MaximumTotalBeautyOfTheGardensBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
