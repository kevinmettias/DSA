using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Tests.DataStructures;

public sealed class RangeBoundsTests
{
    private const string Message = "out of range";

    [Theory]
    [InlineData(0, 1)]
    [InlineData(0, 5)]
    [InlineData(4, 5)]
    public void ValidateIndex_InsideTheSize_DoesNotThrow(int index, int size) =>
        RangeBounds.ValidateIndex(index, size, Message);

    [Theory]
    [InlineData(-1, 5)]
    [InlineData(5, 5)]
    [InlineData(6, 5)]
    [InlineData(0, 0)]
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
    [InlineData(0, 0, 1)]
    [InlineData(0, 4, 5)]
    [InlineData(2, 2, 5)]
    public void ValidateRange_InsideTheSize_DoesNotThrow(int left, int right, int size) =>
        RangeBounds.ValidateRange(left, right, size, Message);

    [Theory]
    [InlineData(-1, 2, 5)]
    [InlineData(0, 5, 5)]
    [InlineData(3, 2, 5)]
    public void ValidateRange_NegativePastTheEndOrInverted_Throws(int left, int right, int size) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => RangeBounds.ValidateRange(left, right, size, Message));

    [Fact]
    public void ValidateRange_AcceptsASingleElementRange() => RangeBounds.ValidateRange(4, 4, 5, Message);
}
