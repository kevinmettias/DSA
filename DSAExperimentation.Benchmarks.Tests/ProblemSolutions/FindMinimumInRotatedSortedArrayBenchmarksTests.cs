using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumInRotatedSortedArrayBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a linear scan of every element against this
// repo's own BinarySearch.LowerBound over the pivot proxy sequence - so a harness whose arms
// disagree has found two different minima. Both answers are one int, so they are compared directly.
//
// Setup rotates the run 0..Length-1 at a fixed non-zero pivot, so the array always holds exactly one
// 0 and never starts with it: the minimum is 0 on every run, which makes the assertion decisive
// rather than a restatement of what the arms returned.
public sealed partial class FindMinimumInRotatedSortedArrayBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup rotates a run of Length distinct values starting at 0, so the smallest value is 0.
    private const int ExpectedMinimum = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameRotatedArray() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_DistinctValuesRotatedAtNonZeroPivot_AgreesWithPivotLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMinimum, harness.LinearScan());
        Assert.Equal(harness.PivotLowerBound(), harness.LinearScan());
    }

    [Fact]
    public void PivotLowerBound_DistinctValuesRotatedAtNonZeroPivot_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMinimum, harness.PivotLowerBound());
        Assert.Equal(harness.LinearScan(), harness.PivotLowerBound());
    }

    private static FindMinimumInRotatedSortedArrayBenchmarks BuildHarness()
    {
        var harness = new FindMinimumInRotatedSortedArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
