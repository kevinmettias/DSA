using DSAExperimentation.Algorithms.GameSearch;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// The reply's window is the mover's turned over and shifted by the move's gain, and an unbounded
// side stays unbounded rather than overflowing; a value is exact inside a window and a bound
// outside it.
public sealed partial class SearchWindowTests
{
    private const int Gain = 3;

    public static TheoryData<int, byte> Bounds => new()
    {
        { 2, (byte)ScoreBound.AtMost },
        { 5, (byte)ScoreBound.Exact },
        { 8, (byte)ScoreBound.AtLeast },
    };

    [Fact]
    public void ForReply_ABoundedWindow_IsTurnedAndShiftedByTheGain() =>
        Assert.Equal(
            new SearchWindow<int>(Gain - 8, Gain - 2), new SearchWindow<int>(2, 8).ForReply(Gain));

    [Fact]
    public void ForReply_TheWidestWindow_StaysTheWidest() =>
        Assert.Equal(SearchWindow<int>.Widest, SearchWindow<int>.Widest.ForReply(Gain));

    [Fact]
    public void Widest_IsUnboundedOnBothSides() =>
        Assert.Equal(new SearchWindow<int>(-int.MaxValue, int.MaxValue), SearchWindow<int>.Widest);

    [Fact]
    public void RaisedTo_AHigherValue_RaisesAlpha() =>
        Assert.Equal(new SearchWindow<int>(4, 8), new SearchWindow<int>(2, 8).RaisedTo(4));

    [Fact]
    public void RaisedTo_ALowerValue_KeepsAlpha() =>
        Assert.Equal(new SearchWindow<int>(2, 8), new SearchWindow<int>(2, 8).RaisedTo(1));

    [Theory]
    [MemberData(nameof(Bounds))]
    public void BoundOf_AValueFoundInTheWindow_IsExactInsideAndABoundOutside(
        int value, byte expected) =>
        Assert.Equal((ScoreBound)expected, new SearchWindow<int>(2, 8).BoundOf(value));
}
