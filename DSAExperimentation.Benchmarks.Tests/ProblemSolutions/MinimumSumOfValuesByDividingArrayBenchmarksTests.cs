using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumSumOfValuesByDividingArrayBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the cheapest split of nums into contiguous groups
// whose ANDs match the target list - so a harness whose arms disagree is timing two different
// problems. Both arms run the identical PartitionStep recurrence and differ only in how a repeated
// (Index, GroupIndex) state is cached, so agreement proves the hand-threaded dictionary and this
// repo's Memoizer answer the same states - including the shared "no valid split" null. Setup cuts nums
// at random points and reads the groups' own ANDs back off as the targets, so a feasible split always
// exists; the same Length must rebuild the same (nums, targets) pair.
public sealed partial class MinimumSumOfValuesByDividingArrayBenchmarksTests
{
    private const int SmallestLength = 12;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DictionaryMemo(), BuildHarness().DictionaryMemo());

    [Fact]
    public void DictionaryMemo_SameNumsAndTargets_AgreesWithMemoizedPartition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedPartition(), harness.DictionaryMemo());
    }

    [Fact]
    public void MemoizedPartition_SameNumsAndTargets_AgreesWithDictionaryMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryMemo(), harness.MemoizedPartition());
    }

    private static MinimumSumOfValuesByDividingArrayBenchmarks BuildHarness()
    {
        var harness = new MinimumSumOfValuesByDividingArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
