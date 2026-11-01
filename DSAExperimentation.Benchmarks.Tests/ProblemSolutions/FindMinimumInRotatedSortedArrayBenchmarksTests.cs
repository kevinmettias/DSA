using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumInRotatedSortedArrayBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this pins the baseline's minimum to the
// value Setup's construction fixes independently of any arm.
//
// Setup rotates the run 0..Length-1 at a fixed non-zero pivot, so the array always holds exactly one
// 0 and never starts with it: the minimum is 0 on every run.
public sealed partial class FindMinimumInRotatedSortedArrayBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup rotates a run of Length distinct values starting at 0, so the smallest value is 0.
    private const int ExpectedMinimum = 0;

    [Fact]
    public void LinearScan_DistinctValuesRotatedAtNonZeroPivot_ReturnsZero() =>
        Assert.Equal(ExpectedMinimum, BuildHarness().LinearScan());

    private static FindMinimumInRotatedSortedArrayBenchmarks BuildHarness()
    {
        var harness = new FindMinimumInRotatedSortedArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
