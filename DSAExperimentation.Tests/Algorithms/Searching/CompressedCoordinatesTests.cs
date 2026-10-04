using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.Tests.Algorithms.Searching;

// Values arrives unsorted and with every coordinate repeated at least once, so a case that passes is
// one where sorting and deduplication both happened: the distinct coordinates are 2, 5, 9 and 40.
public sealed partial class CompressedCoordinatesTests
{
    private static readonly int[] Values = [40, 5, 9, 5, 2, 40, 9, 2];

    public static TheoryData<int, int> CoordinatesToRanks =>
        new() { { 2, 0 }, { 5, 1 }, { 9, 2 }, { 40, 3 } };

    public static TheoryData<int, int> ValuesToCountsBelow =>
        new() { { 1, 0 }, { 5, 1 }, { 6, 2 }, { 41, 4 } };

    public static TheoryData<int, int> ValuesToCountsAtOrBelow =>
        new() { { 1, 0 }, { 5, 2 }, { 6, 2 }, { 40, 4 } };

    [Fact]
    public void Count_RepeatedValues_CountsEachCoordinateOnce() =>
        Assert.Equal(4, new CompressedCoordinates<int>(Values).Count);

    [Fact]
    public void Count_NoValues_IsZero() =>
        Assert.Equal(0, new CompressedCoordinates<int>([]).Count);

    [Theory]
    [MemberData(nameof(CoordinatesToRanks))]
    public void RankOf_Coordinate_ReturnsItsPositionInAscendingOrder(int value, int expected) =>
        Assert.Equal(expected, new CompressedCoordinates<int>(Values).RankOf(value));

    [Fact]
    public void RankOf_ValueBetweenCoordinates_Throws() =>
        Assert.Throws<ArgumentException>(() => new CompressedCoordinates<int>(Values).RankOf(6));

    [Fact]
    public void RankOf_ValuePastTheLastCoordinate_Throws() =>
        Assert.Throws<ArgumentException>(() => new CompressedCoordinates<int>(Values).RankOf(41));

    [Theory]
    [MemberData(nameof(ValuesToCountsBelow))]
    public void LowerBound_AnyValue_CountsCoordinatesBelowIt(int value, int expected) =>
        Assert.Equal(expected, new CompressedCoordinates<int>(Values).LowerBound(value));

    [Theory]
    [MemberData(nameof(ValuesToCountsAtOrBelow))]
    public void UpperBound_AnyValue_CountsCoordinatesAtOrBelowIt(int value, int expected) =>
        Assert.Equal(expected, new CompressedCoordinates<int>(Values).UpperBound(value));

    // The long instantiation is the one the prefix-total solutions use; a value past int still ranks.
    [Fact]
    public void RankOf_LongCoordinatePastInt_ReturnsItsRank()
    {
        var coordinates = new CompressedCoordinates<long>([5_000_000_000L, -3L, 5_000_000_000L, 7L]);

        Assert.Equal(2, coordinates.RankOf(5_000_000_000L));
    }

    // The caller's span is copied before sorting, so its own array keeps its order.
    [Fact]
    public void CompressedCoordinates_BuiltFromAnArray_LeavesTheArrayUnchanged()
    {
        int[] values = [3, 1, 2];

        _ = new CompressedCoordinates<int>(values);

        Assert.Equal([3, 1, 2], values);
    }
}
