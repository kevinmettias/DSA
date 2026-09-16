using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchInRotatedSortedArrayBenchmarks (ARCHITECTURE 17.9): both arms search the
// same rotated array for the same key and report the same position, so a harness whose arms
// disagree is timing two different problems. Setup rebuilds the array from Length alone - the
// rotation pivot is Length divided by the class's own divisor - so the same Length must rebuild the
// same rotation; neither arm mutates the array, so one harness instance is safe to read twice.
public sealed partial class SearchInRotatedSortedArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_SeededRotation_AgreesWithBinarySearchPivotAndSlice()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchPivotAndSlice(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearchPivotAndSlice_SeededRotation_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchPivotAndSlice());
    }

    private static SearchInRotatedSortedArrayBenchmarks BuildHarness()
    {
        var harness = new SearchInRotatedSortedArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
