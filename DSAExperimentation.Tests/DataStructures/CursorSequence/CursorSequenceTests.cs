using DSAExperimentation.DataStructures.CursorSequence;

namespace DSAExperimentation.Tests.DataStructures.CursorSequence;

public sealed partial class CursorSequenceTests
{
    [Fact]
    public void CountBeforeCursor_AfterInserts_IsTheCursorsPosition() =>
        Assert.Equal(3, SequenceOf("abc").CountBeforeCursor);

    [Fact]
    public void CountAfterCursor_AfterAMoveLeft_CountsWhatTheCursorCrossed()
    {
        var sequence = SequenceOf("abc");
        sequence.MoveLeft(2);

        Assert.Equal(2, sequence.CountAfterCursor);
    }

    [Fact]
    public void InsertBeforeCursor_AfterAMoveLeft_LandsBetweenTheTwoSides()
    {
        var sequence = SequenceOf("abc");
        sequence.MoveLeft(1);
        sequence.InsertBeforeCursor('x');
        sequence.MoveRight(1);

        Assert.Equal("abxc", Read(sequence));
    }

    [Fact]
    public void RemoveBeforeCursor_MoreThanPrecede_RemovesThemAllAndSaysHowMany()
    {
        var sequence = SequenceOf("ab");

        Assert.Equal(2, sequence.RemoveBeforeCursor(5));
        Assert.Equal(0, sequence.CountBeforeCursor);
    }

    [Fact]
    public void RemoveBeforeCursor_LeavesWhatFollowsTheCursor()
    {
        var sequence = SequenceOf("abcd");
        sequence.MoveLeft(1);

        Assert.Equal(2, sequence.RemoveBeforeCursor(2));
        sequence.MoveRight(1);
        Assert.Equal("ad", Read(sequence));
    }

    [Fact]
    public void RemoveBeforeCursor_NegativeCount_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SequenceOf("ab").RemoveBeforeCursor(-1));

    [Fact]
    public void MoveLeft_PastTheStart_StopsThereAndSaysHowFarItWent()
    {
        var sequence = SequenceOf("abc");

        Assert.Equal(3, sequence.MoveLeft(5));
        Assert.Equal(0, sequence.CountBeforeCursor);
    }

    [Fact]
    public void MoveLeft_NegativeSteps_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SequenceOf("ab").MoveLeft(-1));

    [Fact]
    public void MoveRight_PastTheEnd_StopsThereAndSaysHowFarItWent()
    {
        var sequence = SequenceOf("abc");
        sequence.MoveLeft(3);

        Assert.Equal(2, sequence.MoveRight(2));
        Assert.Equal(1, sequence.MoveRight(5));
        Assert.Equal(0, sequence.CountAfterCursor);
    }

    [Fact]
    public void MoveRight_AfterMoveLeft_RestoresTheOriginalOrder()
    {
        var sequence = SequenceOf("abcd");
        sequence.MoveLeft(3);
        sequence.MoveRight(3);

        Assert.Equal("abcd", Read(sequence));
    }

    [Fact]
    public void MoveRight_NegativeSteps_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SequenceOf("ab").MoveRight(-1));

    [Fact]
    public void ClearAfterCursor_DropsOnlyWhatFollowsTheCursor()
    {
        var sequence = SequenceOf("abc");
        sequence.MoveLeft(2);
        sequence.ClearAfterCursor();

        Assert.Equal(0, sequence.CountAfterCursor);
        Assert.Equal("a", Read(sequence));
    }

    [Fact]
    public void GetBeforeCursor_ReadsByPositionFromTheStart()
    {
        var sequence = SequenceOf("abc");
        sequence.MoveLeft(1);

        Assert.Equal('b', sequence.GetBeforeCursor(1));
    }

    [Fact]
    public void GetBeforeCursor_PositionAtTheCursor_Throws()
    {
        var sequence = SequenceOf("abc");
        sequence.MoveLeft(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => sequence.GetBeforeCursor(2));
    }

    private static CursorSequence<char> SequenceOf(string text)
    {
        var sequence = new CursorSequence<char>();

        foreach (var c in text)
        {
            sequence.InsertBeforeCursor(c);
        }

        return sequence;
    }

    // Everything before the cursor, in order.
    private static string Read(CursorSequence<char> sequence) =>
        new([.. Enumerable.Range(0, sequence.CountBeforeCursor).Select(sequence.GetBeforeCursor)]);
}
