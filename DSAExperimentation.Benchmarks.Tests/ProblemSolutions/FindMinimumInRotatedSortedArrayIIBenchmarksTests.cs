using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumInRotatedSortedArrayIIBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this pins the baseline's minimum to the
// value Setup's construction fixes independently of any arm.
//
// Setup rotates the run 0..Length-1 left by a third and then stamps a bounded band of duplicates
// over both ends. At the smallest Length that band reaches indices 0..24 and 175..199, while the
// rotation's own 0 sits at index 134 - untouched by either band - so the minimum is 0 on every run.
public sealed partial class FindMinimumInRotatedSortedArrayIIBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // RotatedSortedArrayWorkloads rotates a run starting at 0 and duplicates only boundary values,
    // so the smallest value survives at the rotation's own pivot.
    private const int ExpectedMinimum = 0;

    [Fact]
    public void LinearScan_DuplicateBoundaryBand_ReturnsZero() =>
        Assert.Equal(ExpectedMinimum, BuildHarness().LinearScan());

    private static FindMinimumInRotatedSortedArrayIIBenchmarks BuildHarness()
    {
        var harness = new FindMinimumInRotatedSortedArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
