using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ProximityQueryWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3532's
// nums being a sorted sequence with an occasional large jump among small ones, so the resulting
// components are neither one giant blob nor all singletons, and on random queries landing on a genuine
// mix of connected and disconnected pairs.
public sealed partial class ProximityQueryWorkloadsTests
{
    private const int ValueCount = 500;
    private const int QueryCount = 500;
    private const int Seed = 3532;
    private const int MaxDiff = 5; // the bound the generator hands back with the values
    private const int QueryFieldCount = 2;
    private const int FewestComponents = 2;
    private const int ComponentShareDivisor = 2;

    [Fact]
    public void BuildNums_ValueCount_ReturnsOneValuePerPositionAndTheDocumentedBound()
    {
        var (nums, maxDiff) = ProximityQueryWorkloads.BuildNums(ValueCount, Seed);

        Assert.Equal(ValueCount, nums.Length);
        Assert.Equal(MaxDiff, maxDiff);
    }

    // A gap is a distance between consecutive values, so the sequence has to be non-decreasing for the
    // chain scan both strategies share to read the same components the generator intended.
    [Fact]
    public void BuildNums_Nums_AreNonDecreasingSoTheGapChainIsWellDefined()
    {
        var (nums, _) = ProximityQueryWorkloads.BuildNums(ValueCount, Seed);

        Assert.All(
            Enumerable.Range(1, ValueCount - 1),
            index => Assert.True(nums[index] >= nums[index - 1]));
    }

    // How many components this random walk falls into is not fixed, so what is asserted is the bounded
    // shape the comment names: never one giant blob, and never a scattering of singletons - the chain
    // breaking on a large jump is a probabilistic draw, so the band is what is checked.
    [Fact]
    public void BuildNums_Gaps_SplitTheValuesIntoSeveralComponentsButNotIntoAllSingletons()
    {
        var (nums, maxDiff) = ProximityQueryWorkloads.BuildNums(ValueCount, Seed);

        Assert.InRange(
            ComponentCount(nums, maxDiff),
            FewestComponents,
            ValueCount / ComponentShareDivisor);
    }

    [Fact]
    public void BuildQueries_QueryCount_ReturnsOneTwoIndexPairPerPosition()
    {
        var queries = ProximityQueryWorkloads.BuildQueries(ValueCount, QueryCount, Seed);

        Assert.Equal(QueryCount, queries.Length);
        Assert.All(queries, query => Assert.Equal(QueryFieldCount, query.Length));
        Assert.All(queries, query => Assert.InRange(query[0], 0, ValueCount - 1));
        Assert.All(queries, query => Assert.InRange(query[1], 0, ValueCount - 1));
    }

    [Fact]
    public void BuildNums_SameSeed_ReturnsTheSameNums()
    {
        var (nums, maxDiff) = ProximityQueryWorkloads.BuildNums(ValueCount, Seed);
        var (repeatNums, repeatMaxDiff) = ProximityQueryWorkloads.BuildNums(ValueCount, Seed);

        Assert.Equal(AnswerText.Of(nums), AnswerText.Of(repeatNums));
        Assert.Equal(maxDiff, repeatMaxDiff);
    }

    [Fact]
    public void BuildQueries_SameSeed_ReturnsTheSameQueries() =>
        Assert.Equal(
            AnswerText.Of(ProximityQueryWorkloads.BuildQueries(ValueCount, QueryCount, Seed)),
            AnswerText.Of(ProximityQueryWorkloads.BuildQueries(ValueCount, QueryCount, Seed)));

    private static int ComponentCount(int[] nums, int maxDiff)
    {
        var componentCount = 1;

        foreach (var index in Enumerable.Range(1, nums.Length - 1))
        {
            if (nums[index] - nums[index - 1] > maxDiff)
            {
                componentCount++;
            }
        }

        return componentCount;
    }
}
