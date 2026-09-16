using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for GoodSubsequenceQueriesWorkloads (ARCHITECTURE 17.7). The reading depends on
// LC 3901's two halves being shaped for each other - an array of values and a batch of queries whose
// index and value both stay inside that array's own bounds - so neither strategy is measured on a
// query it could never answer.
public sealed partial class GoodSubsequenceQueriesWorkloadsTests
{
    private const int Length = 256;
    private const int QueryCount = 64;
    private const int Seed = 3901; // LC problem number
    private const int MaxValue = 50;
    private const int QueryFieldCount = 2; // Index, Value

    [Fact]
    public void Build_Length_ReturnsOneValuePerPositionAndOneQueryPerPosition()
    {
        var (nums, queries) = GoodSubsequenceQueriesWorkloads.Build(Length, QueryCount, Seed);

        Assert.Equal(Length, nums.Length);
        Assert.Equal(QueryCount, queries.Length);
    }

    [Fact]
    public void Build_EveryValue_StaysWithinTheClosedRange() =>
        Assert.All(
            GoodSubsequenceQueriesWorkloads.Build(Length, QueryCount, Seed).Nums,
            value => Assert.InRange(value, 1, MaxValue));

    [Fact]
    public void Build_EveryQuery_NamesAnIndexOfTheArrayAndAValueOfTheRange()
    {
        var (_, queries) = GoodSubsequenceQueriesWorkloads.Build(Length, QueryCount, Seed);

        Assert.All(queries, query => Assert.Equal(QueryFieldCount, query.Length));
        Assert.All(queries, query => Assert.InRange(query[0], 0, Length - 1));
        Assert.All(queries, query => Assert.InRange(query[1], 1, MaxValue));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (nums, queries) = GoodSubsequenceQueriesWorkloads.Build(Length, QueryCount, Seed);
        var (repeatNums, repeatQueries) = GoodSubsequenceQueriesWorkloads.Build(Length, QueryCount, Seed);

        Assert.Equal(nums, repeatNums);
        Assert.Equal(AnswerText.Of(queries), AnswerText.Of(repeatQueries));
    }
}
