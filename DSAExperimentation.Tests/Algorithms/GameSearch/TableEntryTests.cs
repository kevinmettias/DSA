using DSAExperimentation.Algorithms.GameSearch;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// A value of 5 remembered from a search to depth 3, asked about windows around it: an exact value
// answers every window, a lower bound only a window whose beta it reaches, an upper bound only one
// whose alpha it stays at or below, and none answers a search to another depth.
public sealed partial class TableEntryTests
{
    private const int Depth = 3;
    private const int Value = 5;

    // Each bound, as its byte, with a window (alpha, beta) it answers.
    public static TheoryData<byte, int, int> Answered => new()
    {
        { (byte)ScoreBound.Exact, 0, 10 },
        { (byte)ScoreBound.AtLeast, 0, 5 },
        { (byte)ScoreBound.AtMost, 5, 10 },
    };

    // Each inexact bound with a window it leaves open.
    public static TheoryData<byte, int, int> LeftOpen => new()
    {
        { (byte)ScoreBound.AtLeast, 0, 6 },
        { (byte)ScoreBound.AtMost, 4, 10 },
    };

    [Theory]
    [MemberData(nameof(Answered))]
    public void CanAnswer_AWindowTheBoundSettles_Answers(byte bound, int alpha, int beta) =>
        Assert.True(Entry(bound).CanAnswer(Depth, new(alpha, beta)));

    [Theory]
    [MemberData(nameof(LeftOpen))]
    public void CanAnswer_AWindowTheBoundLeavesOpen_DoesNot(byte bound, int alpha, int beta) =>
        Assert.False(Entry(bound).CanAnswer(Depth, new(alpha, beta)));

    [Fact]
    public void CanAnswer_ASearchToAnotherDepth_DoesNot() =>
        Assert.False(Entry((byte)ScoreBound.Exact).CanAnswer(Depth + 1, SearchWindow<int>.Widest));

    private static TableEntry<int> Entry(byte bound) => new(Depth, Value, (ScoreBound)bound, 0);
}
