using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveDuplicatesFromSortedArrayBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveDuplicatesFromSortedArraySolution's, competing strategies for the same question - LINQ
// Distinct against an in-place indexed-sequence compaction - so a harness whose arms report different
// lengths compacted two different arrays. Both arms mutate the array they are handed, and each gets
// its own copy of _values from the benchmark method itself, so the hoisted workload is never written
// through and one harness serves both arms in either order.
public sealed partial class RemoveDuplicatesFromSortedArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinqDistinct(), BuildHarness().LinqDistinct());

    [Fact]
    public void LinqDistinct_AgreesWithArrayIndexedSequenceCompact()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayIndexedSequenceCompact(), harness.LinqDistinct());
    }

    [Fact]
    public void ArrayIndexedSequenceCompact_AgreesWithLinqDistinct()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinqDistinct(), harness.ArrayIndexedSequenceCompact());
    }

    private static RemoveDuplicatesFromSortedArrayBenchmarks BuildHarness()
    {
        var harness = new RemoveDuplicatesFromSortedArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
