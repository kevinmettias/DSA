using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ThresholdMajorityQueriesWorkloads (ARCHITECTURE 17.7): LC 3636 answers a batch of
// [l, r, threshold] majority queries over one fixed array. The array is drawn from a small value
// alphabet so real majorities exist to find rather than every query bottoming out at threshold 1, and
// every query names a non-empty window with a threshold inside it - both are properties of the problem's
// own input contract, not of either strategy.
public sealed partial class ThresholdMajorityQueriesWorkloadsTests
{
    private const int ElementCount = 256;
    private const int Seed = 3636; // LC problem number
    private const int ValueAlphabetSize = 20;
    private const int SmallestValue = 1;
    private const int SmallestValueCount = 2;
    private const int QueryWidth = 3;
    private const int SmallestThreshold = 1;

    [Fact]
    public void BuildNums_ElementCount_ReturnsOneValuePerPositionWithinTheAlphabet()
    {
        var nums = ThresholdMajorityQueriesWorkloads.BuildNums(ElementCount, Seed);

        Assert.Equal(ElementCount, nums.Length);
        Assert.All(nums, value => Assert.InRange(value, SmallestValue, ValueAlphabetSize));
    }

    // A single repeated value would make every window's majority the same and no query discriminating,
    // so the array has to hold at least two distinct values for a majority search to say anything.
    [Fact]
    public void BuildNums_DrawnFromASmallAlphabet_HoldsMoreThanOneDistinctValue() =>
        Assert.True(
            ThresholdMajorityQueriesWorkloads.BuildNums(ElementCount, Seed).Distinct().Count()
                >= SmallestValueCount);

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameArray() =>
        Assert.Equal(
            ThresholdMajorityQueriesWorkloads.BuildNums(ElementCount, Seed),
            ThresholdMajorityQueriesWorkloads.BuildNums(ElementCount, Seed));

    [Fact]
    public void BuildQueries_QueryBatch_ReturnsOneTriplePerQueryOverInRangeWindows()
    {
        var queries = ThresholdMajorityQueriesWorkloads.BuildQueries(ElementCount, Seed);

        Assert.NotEmpty(queries);
        Assert.All(queries, query => Assert.Equal(QueryWidth, query.Length));
        Assert.All(queries, query => Assert.InRange(query[0], 0, ElementCount - 1));
        Assert.All(queries, query => Assert.InRange(query[1], query[0], ElementCount - 1));
        Assert.All(queries, query => Assert.InRange(query[2], SmallestThreshold, query[1] - query[0] + 1));
    }

    [Fact]
    public void BuildQueries_SameSeed_ReturnsTheSameQueries() =>
        Assert.Equal(
            AnswerText.Of(ThresholdMajorityQueriesWorkloads.BuildQueries(ElementCount, Seed)),
            AnswerText.Of(ThresholdMajorityQueriesWorkloads.BuildQueries(ElementCount, Seed)));
}
