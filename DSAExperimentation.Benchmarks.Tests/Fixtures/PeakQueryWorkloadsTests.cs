using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PeakQueryWorkloads (ARCHITECTURE 17.7). The reading depends on the two Peaks In
// Array siblings (LC 3187 and LC 4017) sharing one input shape: values that are peaks neither
// everywhere nor nowhere, and a query script that alternates a peak count with a point update so the
// O(n) and O((r-l)^3) rescans always have real work to do.
public sealed partial class PeakQueryWorkloadsTests
{
    private const int Length = 200;
    private const int Seed = 3187; // LC problem number
    private const int FewestPeakValues = 1;
    private const int QueryFieldCount = 3;
    private const int RangeCountKind = 1;
    private const int PointUpdateKind = 2;
    private const int SecondFieldIndex = 1;
    private const int ThirdFieldIndex = 2;
    private const int QueryKindAlternationCycle = 2; // even positions are a type-1 count, odd positions a type-2 update

    [Fact]
    public void Build_Length_ReturnsOneValueAndOneQueryPerPosition()
    {
        var (nums, queries) = PeakQueryWorkloads.Build(Length, Seed);

        Assert.Equal(Length, nums.Length);
        Assert.Equal(Length, queries.Length);
        Assert.All(queries, query => Assert.Equal(QueryFieldCount, query.Length));
    }

    // Nums stay inside 1..length-1, which is the band that makes peaks neither everywhere nor nowhere:
    // LC 3187 calls an index a peak only when both neighbours exist and are smaller.
    [Fact]
    public void Build_Nums_StayInsideTheValueBandThatLeavesPeaksNeitherEverywhereNorNowhere() =>
        Assert.All(PeakQueryWorkloads.Build(Length, Seed).Nums, value => Assert.InRange(value, 1, Length - 1));

    // Even positions are a type-1 count over the whole array, odd positions a type-2 point update, which
    // is the alternation both benchmarks' comments describe.
    [Fact]
    public void Build_Queries_AlternateAWholeRangePeakCountWithAPointUpdate()
    {
        var (_, queries) = PeakQueryWorkloads.Build(Length, Seed);

        Assert.All(
            Enumerable.Range(0, Length),
            index => AssertQuery(queries[index], index % QueryKindAlternationCycle == 0));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (nums, queries) = PeakQueryWorkloads.Build(Length, Seed);
        var (repeatNums, repeatQueries) = PeakQueryWorkloads.Build(Length, Seed);

        Assert.Equal(AnswerText.Of(nums), AnswerText.Of(repeatNums));
        Assert.Equal(AnswerText.Of(queries), AnswerText.Of(repeatQueries));
    }

    private static void AssertQuery(int[] query, bool isRangeCount)
    {
        Assert.Equal(isRangeCount ? RangeCountKind : PointUpdateKind, query[0]);

        if (isRangeCount)
        {
            Assert.Equal(0, query[SecondFieldIndex]);
            Assert.Equal(Length - 1, query[ThirdFieldIndex]);

            return;
        }

        Assert.InRange(query[SecondFieldIndex], 0, Length - 1);
        Assert.InRange(query[ThirdFieldIndex], FewestPeakValues, Length - 1);
    }
}
