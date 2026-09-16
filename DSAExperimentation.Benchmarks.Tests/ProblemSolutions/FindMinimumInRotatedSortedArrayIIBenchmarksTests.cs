using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumInRotatedSortedArrayIIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a linear scan against the duplicate-tolerant
// binary shrink - so a harness whose arms disagree has found two different minima of the same
// rotated array. Both answers are one int, so they are compared directly.
//
// Setup rotates the run 0..Length-1 left by a third and then stamps a bounded band of duplicates
// over both ends. At the smallest Length that band reaches indices 0..24 and 175..199, while the
// rotation's own 0 sits at index 134 - untouched by either band - so the minimum is 0 on every run,
// which makes the assertion decisive rather than a restatement of what the arms returned.
public sealed partial class FindMinimumInRotatedSortedArrayIIBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // RotatedSortedArrayWorkloads rotates a run starting at 0 and duplicates only boundary values,
    // so the smallest value survives at the rotation's own pivot.
    private const int ExpectedMinimum = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameDuplicateBandedArray() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_DuplicateBoundaryBand_AgreesWithDuplicateTolerantBinaryShrink()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMinimum, harness.LinearScan());
        Assert.Equal(harness.DuplicateTolerantBinaryShrink(), harness.LinearScan());
    }

    [Fact]
    public void DuplicateTolerantBinaryShrink_DuplicateBoundaryBand_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMinimum, harness.DuplicateTolerantBinaryShrink());
        Assert.Equal(harness.LinearScan(), harness.DuplicateTolerantBinaryShrink());
    }

    private static FindMinimumInRotatedSortedArrayIIBenchmarks BuildHarness()
    {
        var harness = new FindMinimumInRotatedSortedArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
