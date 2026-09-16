using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveDuplicatesFromSortedArrayIIBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveDuplicatesFromSortedArrayIISolution's, competing strategies for the same question - LINQ
// GroupBy capped at two against an in-place indexed-sequence compaction - so a harness whose arms
// report different lengths compacted two different arrays. Both arms mutate the array they are handed,
// and each gets its own copy of _values from the benchmark method itself, so the hoisted workload is
// never written through and one harness serves both arms in either order.
public sealed partial class RemoveDuplicatesFromSortedArrayIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinqGroupCapTwo(), BuildHarness().LinqGroupCapTwo());

    [Fact]
    public void LinqGroupCapTwo_AgreesWithArrayIndexedSequenceCompact()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArrayIndexedSequenceCompact(), harness.LinqGroupCapTwo());
    }

    [Fact]
    public void ArrayIndexedSequenceCompact_AgreesWithLinqGroupCapTwo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinqGroupCapTwo(), harness.ArrayIndexedSequenceCompact());
    }

    private static RemoveDuplicatesFromSortedArrayIIBenchmarks BuildHarness()
    {
        var harness = new RemoveDuplicatesFromSortedArrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
