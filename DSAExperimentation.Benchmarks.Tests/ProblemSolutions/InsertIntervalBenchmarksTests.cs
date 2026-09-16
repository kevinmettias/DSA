using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertIntervalBenchmarks (ARCHITECTURE 17.9). Both arms are competing
// strategies for the same question - append-and-sort-then-merge against this repo's own
// IntervalSet, which already maintains the merged invariant on every Add - so they are asserted
// to agree. Both arms return only the resulting interval count, a proxy rather than the merged
// intervals themselves: two strategies that merged into differently-shaped intervals but landed
// on the same count would still agree, and no return type is changed here to strengthen it. The
// count is nonetheless decisive on its own, because the fixture fixes it: intervals i is
// [3i, 3i+1] and the new interval is [Length, 2 * Length], so at Length = 200 the new interval
// [200, 400] swallows the chain of intervals 67..133 - whose starts 201..399 all fall inside it -
// and nothing else, since interval 66 ends at 199 and interval 134 starts at 402. That leaves the
// 67 intervals before the chain, the one merged block, and the 66 after it. Setup is seeded by
// construction rather than by a Random, so the same Length must rebuild the same intervals and
// the same new interval.
public sealed partial class InsertIntervalBenchmarksTests
{
    private const int SmallestLength = 200;

    // 67 intervals before the swallowed chain, one merged block, 66 after it.
    private const int ExpectedMergedIntervalCount = 134;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ListInsertAndMerge(), BuildHarness().ListInsertAndMerge());

    [Fact]
    public void ListInsertAndMerge_MergedIntervalCount_AgreesWithIntervalSetAdd()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IntervalSetAdd(), harness.ListInsertAndMerge());
        Assert.Equal(ExpectedMergedIntervalCount, harness.ListInsertAndMerge());
    }

    [Fact]
    public void IntervalSetAdd_MergedIntervalCount_AgreesWithListInsertAndMerge()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListInsertAndMerge(), harness.IntervalSetAdd());
        Assert.Equal(ExpectedMergedIntervalCount, harness.IntervalSetAdd());
    }

    private static InsertIntervalBenchmarks BuildHarness()
    {
        var harness = new InsertIntervalBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
