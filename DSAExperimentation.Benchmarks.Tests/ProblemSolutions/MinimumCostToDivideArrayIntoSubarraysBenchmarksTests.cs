using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCostToDivideArrayIntoSubarraysBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a hand-rolled Dictionary memo against this
// repo's Memoizer over the identical partition recurrence - so a harness whose arms disagree is timing
// two different recurrences. Both arms answer with a long minimum cost, which they compare directly.
// Setup builds nums and cost from one seeded stream, so the same Length must rebuild both.
public sealed partial class MinimumCostToDivideArrayIntoSubarraysBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArrays() =>
        Assert.Equal(BuildHarness().DictionaryMemo(), BuildHarness().DictionaryMemo());

    [Fact]
    public void DictionaryMemo_SeededPartitionCosts_AgreesWithMemoizedPartition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedPartition(), harness.DictionaryMemo());
    }

    [Fact]
    public void MemoizedPartition_SeededPartitionCosts_AgreesWithDictionaryMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryMemo(), harness.MemoizedPartition());
    }

    private static MinimumCostToDivideArrayIntoSubarraysBenchmarks BuildHarness()
    {
        var harness = new MinimumCostToDivideArrayIntoSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
