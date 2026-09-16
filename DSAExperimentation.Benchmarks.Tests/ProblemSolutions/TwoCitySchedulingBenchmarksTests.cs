using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TwoCitySchedulingBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the BCL Array.Sort greedy over the cost differences against
// the same greedy routed through this repo's own MergeSort - so a harness whose arms disagree is
// timing two different problems. Both arms return the minimum total cost as an int, so they are
// compared directly. Setup builds the already-projected people from a fixed seed, so the same
// Length must rebuild the same workload; the projected order is what the sort arm reorders, so
// both arms see the same multiset of people however the sort swaps them.
public sealed partial class TwoCitySchedulingBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ArraySortGreedy(), BuildHarness().ArraySortGreedy());

    [Fact]
    public void ArraySortGreedy_SmallestLength_AgreesWithMergeSortGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortGreedy(), harness.ArraySortGreedy());
    }

    [Fact]
    public void MergeSortGreedy_SmallestLength_AgreesWithArraySortGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ArraySortGreedy(), harness.MergeSortGreedy());
    }

    private static TwoCitySchedulingBenchmarks BuildHarness()
    {
        var harness = new TwoCitySchedulingBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
