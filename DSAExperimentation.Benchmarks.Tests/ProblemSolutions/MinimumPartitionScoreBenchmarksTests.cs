using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumPartitionScoreBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the cheapest split of nums into exactly the required number of
// contiguous groups - so a harness whose arms disagree is timing two different problems. Both arms run
// the identical recurrence and differ only in how a repeated (Index, GroupsUsed) state is cached, so
// agreement is what proves the hand-threaded dictionary and this repo's Memoizer answer the same
// states. Both read the one array [GlobalSetup] built, drawn from a fixed seed, so the same Length
// must rebuild the same array and the same group count.
public sealed partial class MinimumPartitionScoreBenchmarksTests
{
    private const int SmallestLength = 12;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DictionaryMemo(), BuildHarness().DictionaryMemo());

    [Fact]
    public void DictionaryMemo_SameValueRun_AgreesWithMemoizedPartition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedPartition(), harness.DictionaryMemo());
    }

    [Fact]
    public void MemoizedPartition_SameValueRun_AgreesWithDictionaryMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryMemo(), harness.MemoizedPartition());
    }

    private static MinimumPartitionScoreBenchmarks BuildHarness()
    {
        var harness = new MinimumPartitionScoreBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
