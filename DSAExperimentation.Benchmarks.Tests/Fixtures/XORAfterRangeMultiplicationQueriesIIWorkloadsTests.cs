using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for XORAfterRangeMultiplicationQueriesIIWorkloads (ARCHITECTURE 17.7): LC 3655's
// sqrt-decomposition arm only has a bucketed path to measure when some queries use a small stride, so this
// fixture splits its batch in two - even indices draw from the small-stride range, odd indices from the whole
// array. That split is the workload's reason to exist, so it is asserted alongside the input contract.
public sealed partial class XORAfterRangeMultiplicationQueriesIIWorkloadsTests
{
    private const int NodeCount = 64;
    private const int QueryCount = 32;
    private const int Seed = 3655; // LC problem number
    private const int QueryWidth = 4; // l, r, k, v
    private const int StrideFieldIndex = 2;
    private const int MultiplierFieldIndex = 3;
    private const int MaxStartingValue = 1_000_000_000;
    private const int MaxMultiplier = 100_000;
    private const int SmallestValue = 1;
    private const int SmallStrideIndexParity = 0;
    private const int QueryIndexCycle = 2; // even indices draw the small stride, odd indices the whole array

    [Fact]
    public void Build_StartingValues_ReturnsOnePerPositionInsideTheDocumentedBound()
    {
        var (nums, _) = XORAfterRangeMultiplicationQueriesIIWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.Equal(NodeCount, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, SmallestValue, MaxStartingValue));
    }

    [Fact]
    public void Build_EveryQuery_NamesANonEmptyWindowWithAStrideAndMultiplierInRange()
    {
        var (_, queries) = XORAfterRangeMultiplicationQueriesIIWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.Equal(QueryCount, queries.Length);
        Assert.All(queries, query => Assert.Equal(QueryWidth, query.Length));
        Assert.All(queries, query => Assert.InRange(query[0], 0, NodeCount - 1));
        Assert.All(queries, query => Assert.InRange(query[1], query[0], NodeCount - 1));
        Assert.All(queries, query => Assert.InRange(query[StrideFieldIndex], SmallestValue, NodeCount));
        Assert.All(queries, query => Assert.InRange(query[MultiplierFieldIndex], SmallestValue, MaxMultiplier));
    }

    // The half that gives the bucketed path its work: every small-stride index carries a stride at most
    // sqrt(n), which is the bound the decomposition's bucket width is chosen for.
    [Fact]
    public void Build_SmallStrideHalf_DrawsStridesNoLargerThanTheSquareRootBound()
    {
        var (_, queries) = XORAfterRangeMultiplicationQueriesIIWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.All(
            Enumerable.Range(0, QueryCount).Where(index => index % QueryIndexCycle == SmallStrideIndexParity),
            index => Assert.InRange(queries[index][StrideFieldIndex], SmallestValue, SmallStrideCeiling));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(XORAfterRangeMultiplicationQueriesIIWorkloads.Build(NodeCount, QueryCount, Seed)),
            AnswerText.Of(XORAfterRangeMultiplicationQueriesIIWorkloads.Build(NodeCount, QueryCount, Seed)));

    private static int SmallStrideCeiling => Math.Max(1, (int)Math.Sqrt(NodeCount));
}
