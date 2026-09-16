using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RelativeSortArrayBenchmarks (ARCHITECTURE 17.9): both arms are
// RelativeSortArraySolution's, competing strategies for the same question - a comparer that
// re-derives each rank with Array.IndexOf against a precomputed rank table - so a harness whose arms
// disagree orders two different ways. Both arms sort a clone of _arr1 rather than _arr1 itself, so
// the hoisted workload is read-only and one harness serves both arms in either order.
public sealed partial class RelativeSortArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScanComparerSort()),
            AnswerText.Of(BuildHarness().LinearScanComparerSort()));

    [Fact]
    public void LinearScanComparerSort_AgreesWithHashMapMergeSort()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.HashMapMergeSort()),
            AnswerText.Of(harness.LinearScanComparerSort()));
    }

    [Fact]
    public void HashMapMergeSort_AgreesWithLinearScanComparerSort()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LinearScanComparerSort()),
            AnswerText.Of(harness.HashMapMergeSort()));
    }

    private static RelativeSortArrayBenchmarks BuildHarness()
    {
        var harness = new RelativeSortArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
