using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MergeSimilarItemsWorkloads (ARCHITECTURE 17.7). The reading depends on LC 2363's
// two item arrays holding disjoint values, so neither arm gets an early-exit shortcut from a shared
// value and the naive linear scan always pays its full-length walk.
public sealed partial class MergeSimilarItemsWorkloadsTests
{
    private const int Length = 128;
    private const int Stride = 2;
    private const int Weight = 1;
    private const int FirstEvenValue = 0;
    private const int FirstOddValue = 1;
    private const int ItemFieldCount = 2; // Value, Weight

    [Fact]
    public void BuildDisjointValues_Length_ReturnsOneItemPerRequestedPositionInEachArray()
    {
        var (items1, items2) = MergeSimilarItemsWorkloads.BuildDisjointValues(Length);

        Assert.Equal(Length, items1.Length);
        Assert.Equal(Length, items2.Length);
        Assert.All(items1, item => Assert.Equal(ItemFieldCount, item.Length));
        Assert.All(items2, item => Assert.Equal(ItemFieldCount, item.Length));
    }

    [Fact]
    public void BuildDisjointValues_Items1_AreTheEvenValuesWithWeightOne()
    {
        var (items1, _) = MergeSimilarItemsWorkloads.BuildDisjointValues(Length);

        Assert.Equal(
            Enumerable.Range(0, Length).Select(position => FirstEvenValue + (position * Stride)),
            items1.Select(item => item[0]));
        Assert.All(items1, item => Assert.Equal(Weight, item[1]));
    }

    [Fact]
    public void BuildDisjointValues_Items2_AreTheOddValuesWithWeightOne()
    {
        var (_, items2) = MergeSimilarItemsWorkloads.BuildDisjointValues(Length);

        Assert.Equal(
            Enumerable.Range(0, Length).Select(position => FirstOddValue + (position * Stride)),
            items2.Select(item => item[0]));
        Assert.All(items2, item => Assert.Equal(Weight, item[1]));
    }

    [Fact]
    public void BuildDisjointValues_NoValue_AppearsInBothArrays()
    {
        var (items1, items2) = MergeSimilarItemsWorkloads.BuildDisjointValues(Length);

        Assert.Empty(items1.Select(item => item[0]).Intersect(items2.Select(item => item[0])));
    }
}
