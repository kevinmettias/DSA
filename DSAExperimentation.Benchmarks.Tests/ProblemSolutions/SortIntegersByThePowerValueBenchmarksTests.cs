using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortIntegersByThePowerValueBenchmarks (ARCHITECTURE 17.9): both arms
// recompute the same Collatz powers over the same range and report the same k-th pair, so a
// harness whose arms disagree is timing two different problems. There is no [GlobalSetup] to
// rebuild - the measured range is four constants away from RangeLength, so the workload is the
// same on every call and there is nothing to hold.
//
// Each arm reports one integer value, which is the answer itself: the range is [1, RangeLength]
// and k is the range length, so the reported pair is the last in sorted order.
public sealed partial class SortIntegersByThePowerValueBenchmarksTests
{
    private const int SmallestRangeLength = 200;

    [Fact]
    public void InsertionSort_SmallestRange_AgreesWithMergeSortAscending()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortAscending(), harness.InsertionSort());
    }

    [Fact]
    public void MergeSortAscending_SmallestRange_AgreesWithInsertionSort()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InsertionSort(), harness.MergeSortAscending());
    }

    private static SortIntegersByThePowerValueBenchmarks BuildHarness() =>
        new() { RangeLength = SmallestRangeLength };
}
