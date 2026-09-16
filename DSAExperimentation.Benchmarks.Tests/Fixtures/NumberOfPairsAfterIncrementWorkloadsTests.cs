using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NumberOfPairsAfterIncrementWorkloads (ARCHITECTURE 17.7). The reading depends on
// LC 3943's nums1 sitting at the problem's own five-value cap, on nums2 and the query stream being much
// larger, and on the two query kinds alternating so neither strategy replays only one of them.
public sealed partial class NumberOfPairsAfterIncrementWorkloadsTests
{
    private const int Nums1Length = 5; // LC 3943's own cap, not a measurement choice
    private const int Nums2Length = 500;
    private const int QueryCount = 2_000;
    private const int Seed = 3943; // LC problem number
    private const int SmallestValue = 1;
    private const int ValueUpperBound = 100_000; // exclusive
    private const int SmallestDelta = 1;
    private const long SmallestCountTotal = 2;
    private const long LargestCountTotal = (2L * ValueUpperBound) - 1;
    private const int UnusedRangeBound = 0; // Left/Right/Delta on a count query, and Tot on an increment

    [Fact]
    public void BuildNums1_ValueCount_ReturnsTheProblemsOwnFiveValueCap() =>
        Assert.Equal(Nums1Length, NumberOfPairsAfterIncrementWorkloads.BuildNums1(Seed).Length);

    [Fact]
    public void BuildNums1_EveryValue_StaysInsideTheDocumentedBand() =>
        Assert.All(
            NumberOfPairsAfterIncrementWorkloads.BuildNums1(Seed),
            value => Assert.InRange(value, SmallestValue, ValueUpperBound - 1));

    [Fact]
    public void BuildNums2_Length_ReturnsOneValuePerPosition() =>
        Assert.Equal(Nums2Length, NumberOfPairsAfterIncrementWorkloads.BuildNums2(Nums2Length, Seed).Length);

    [Fact]
    public void BuildNums2_EveryValue_StaysInsideTheDocumentedBand() =>
        Assert.All(
            NumberOfPairsAfterIncrementWorkloads.BuildNums2(Nums2Length, Seed),
            value => Assert.InRange(value, SmallestValue, ValueUpperBound - 1));

    // The two entries are the same seeded draw of the same quantity, one of them at the length LC 3943
    // caps for nums1 - so a change to either entry's draw is a workload change for one field of the
    // problem only, which the other entry would otherwise hide.
    [Fact]
    public void BuildNums2_AtTheNums1Length_AgreesWithBuildNums1() =>
        Assert.Equal(
            AnswerText.Of(NumberOfPairsAfterIncrementWorkloads.BuildNums1(Seed)),
            AnswerText.Of(NumberOfPairsAfterIncrementWorkloads.BuildNums2(Nums1Length, Seed)));

    [Fact]
    public void BuildNums1_SameSeed_ReturnsTheSameNums() =>
        Assert.Equal(
            AnswerText.Of(NumberOfPairsAfterIncrementWorkloads.BuildNums1(Seed)),
            AnswerText.Of(NumberOfPairsAfterIncrementWorkloads.BuildNums1(Seed)));

    [Fact]
    public void BuildNums2_SameSeed_ReturnsTheSameNums() =>
        Assert.Equal(
            AnswerText.Of(NumberOfPairsAfterIncrementWorkloads.BuildNums2(Nums2Length, Seed)),
            AnswerText.Of(NumberOfPairsAfterIncrementWorkloads.BuildNums2(Nums2Length, Seed)));

    [Fact]
    public void BuildQueries_QueryCount_ReturnsOneQueryPerPosition() =>
        Assert.Equal(
            QueryCount,
            NumberOfPairsAfterIncrementWorkloads.BuildQueries(QueryCount, Nums2Length, Seed).Length);

    [Fact]
    public void BuildQueries_EveryQuery_IsOneOfTheTwoKindsWithOnlyThatKindsFieldsSet()
    {
        var queries = NumberOfPairsAfterIncrementWorkloads.BuildQueries(QueryCount, Nums2Length, Seed);

        Assert.All(queries, query => Assert.Contains(query.Kind, Enum.GetValues<PairQueryKind>()));
        Assert.All(
            queries.Where(query => query.Kind == PairQueryKind.Increment),
            query => AssertRangeAdd(query, Nums2Length));
        Assert.All(
            queries.Where(query => query.Kind == PairQueryKind.Count),
            query => AssertCountQuery(query));
    }

    // The comment on the generator names an alternating mix - roughly one range-add per count query -
    // which is what keeps both strategies paying for the ranges they replay instead of one of them
    // seeing a stream of one shape only.
    [Fact]
    public void BuildQueries_EveryOtherQuery_AlternatesARangeAddWithACount() =>
        Assert.Equal(
            Enumerable.Range(0, QueryCount).Select(index => index % 2 == 0 ? PairQueryKind.Increment : PairQueryKind.Count),
            NumberOfPairsAfterIncrementWorkloads.BuildQueries(QueryCount, Nums2Length, Seed).Select(query => query.Kind));

    [Fact]
    public void BuildQueries_SameSeed_ReturnsTheSameQueries() =>
        Assert.Equal(
            AnswerText.Of(NumberOfPairsAfterIncrementWorkloads.BuildQueries(QueryCount, Nums2Length, Seed)),
            AnswerText.Of(NumberOfPairsAfterIncrementWorkloads.BuildQueries(QueryCount, Nums2Length, Seed)));

    private static void AssertRangeAdd(PairQuery query, int nums2Length)
    {
        Assert.InRange(query.Left, 0, nums2Length - 1);
        Assert.InRange(query.Right, query.Left, nums2Length - 1);
        Assert.InRange(query.Delta, SmallestDelta, ValueUpperBound - 1);
        Assert.Equal(UnusedRangeBound, query.Tot);
    }

    private static void AssertCountQuery(PairQuery query)
    {
        Assert.InRange(query.Tot, SmallestCountTotal, LargestCountTotal);
        Assert.Equal(UnusedRangeBound, query.Left);
        Assert.Equal(UnusedRangeBound, query.Right);
        Assert.Equal(UnusedRangeBound, query.Delta);
    }
}
