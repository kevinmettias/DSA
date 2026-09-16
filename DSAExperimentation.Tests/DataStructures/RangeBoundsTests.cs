using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Tests.DataStructures;

public sealed partial class RangeBoundsTests
{
    private const string Message = "out of range";

    public static TheoryData<int, int> IndicesInsideTheSize =>
        new() { { 0, 1 }, { 0, 5 }, { 4, 5 } };

    public static TheoryData<int, int> IndicesOutsideTheSize =>
        new() { { -1, 5 }, { 5, 5 }, { 6, 5 }, { 0, 0 } };

    public static TheoryData<int, int, int> RangesInsideTheSize =>
        new() { { 0, 0, 1 }, { 0, 4, 5 }, { 2, 2, 5 } };

    public static TheoryData<int, int, int> RangesNegativePastTheEndOrInverted =>
        new() { { -1, 2, 5 }, { 0, 5, 5 }, { 3, 2, 5 } };

    [Theory]
    [MemberData(nameof(IndicesInsideTheSize))]
    public void ValidateIndex_InsideTheSize_DoesNotThrow(int index, int size) =>
        Assert.Null(Record.Exception(() => RangeBounds.ValidateIndex(index, size, Message)));

    [Theory]
    [MemberData(nameof(IndicesOutsideTheSize))]
    public void ValidateIndex_OutsideTheSize_Throws(int index, int size) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RangeBounds.ValidateIndex(index, size, Message));

    [Fact]
    public void ValidateIndex_CarriesTheCallersMessage()
    {
        var error = Assert.Throws<ArgumentOutOfRangeException>(
            () => RangeBounds.ValidateIndex(9, 2, "index past the end"));

        Assert.Contains("index past the end", error.Message);
    }

    [Theory]
    [MemberData(nameof(RangesInsideTheSize))]
    public void ValidateRange_InsideTheSize_DoesNotThrow(int left, int right, int size) =>
        Assert.Null(Record.Exception(() => RangeBounds.ValidateRange(left, right, size, Message)));

    [Theory]
    [MemberData(nameof(RangesNegativePastTheEndOrInverted))]
    public void ValidateRange_NegativePastTheEndOrInverted_Throws(int left, int right, int size) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RangeBounds.ValidateRange(left, right, size, Message));

    [Fact]
    public void ValidateRange_AcceptsASingleElementRange() =>
        Assert.Null(Record.Exception(() => RangeBounds.ValidateRange(4, 4, 5, Message)));
}
