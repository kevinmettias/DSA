using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for XORAfterRangeMultiplicationQueriesIWorkloads (ARCHITECTURE 17.7): LC 3653 multiplies
// every position in [l, r] whose index is a multiple of k by v, then XORs the array. Both strategies see the
// same random strides and multipliers, so the fixture owes the problem's own input contract: a window that
// is non-empty, a stride within the array, and a multiplier within the stated bound.
public sealed partial class XORAfterRangeMultiplicationQueriesIWorkloadsTests
{
    private const int NodeCount = 64;
    private const int QueryCount = 32;
    private const int Seed = 3653; // LC problem number
    private const int QueryWidth = 4; // l, r, k, v
    private const int StrideFieldIndex = 2;
    private const int MultiplierFieldIndex = 3;
    private const int MaxStartingValue = 1_000_000_000;
    private const int MaxMultiplier = 100_000;
    private const int SmallestValue = 1;

    [Fact]
    public void Build_StartingValues_ReturnsOnePerPositionInsideTheDocumentedBound()
    {
        var (nums, _) = XORAfterRangeMultiplicationQueriesIWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.Equal(NodeCount, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, SmallestValue, MaxStartingValue));
    }

    [Fact]
    public void Build_EveryQuery_NamesANonEmptyWindowWithAStrideAndMultiplierInRange()
    {
        var (_, queries) = XORAfterRangeMultiplicationQueriesIWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.Equal(QueryCount, queries.Length);
        Assert.All(queries, query => Assert.Equal(QueryWidth, query.Length));
        Assert.All(queries, query => Assert.InRange(query[0], 0, NodeCount - 1));
        Assert.All(queries, query => Assert.InRange(query[1], query[0], NodeCount - 1));
        Assert.All(queries, query => Assert.InRange(query[StrideFieldIndex], SmallestValue, NodeCount));
        Assert.All(queries, query => Assert.InRange(query[MultiplierFieldIndex], SmallestValue, MaxMultiplier));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(XORAfterRangeMultiplicationQueriesIWorkloads.Build(NodeCount, QueryCount, Seed)),
            AnswerText.Of(XORAfterRangeMultiplicationQueriesIWorkloads.Build(NodeCount, QueryCount, Seed)));
}
