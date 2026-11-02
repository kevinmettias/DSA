using DSAExperimentation.Domain.Games.SlidingPuzzle;

namespace DSAExperimentation.Tests.Domain.Games.SlidingPuzzle;

public sealed partial class PuzzleSlidesTests
{
    [Fact]
    public void StateCount_IsEveryArrangementOfTheCells() => Assert.Equal(720, new PuzzleSlides<TwoByThree>().StateCount);

    // 16! arrangements do not fit an int, so the 15-puzzle's space cannot be enumerated.
    [Fact]
    public void StateCount_OfABoardTooLargeToNumber_Throws()
        => Assert.Throws<OverflowException>(() => new PuzzleSlides<FifteenPuzzle>().StateCount);

    [Fact]
    public void MaxSuccessors_IsOnePerNeighbourOfTheEmptyCell() => Assert.Equal(4, new PuzzleSlides<TwoByThree>().MaxSuccessors);

    [Fact]
    public void StateAt_NumbersArrangementsInTheOrderTheyRead()
    {
        var space = new PuzzleSlides<TwoByThree>();

        Assert.Equal("012345", space.StateAt(0));
        Assert.Equal("012354", space.StateAt(1));
        Assert.Equal("543210", space.StateAt(719));
    }

    [Fact]
    public void StateAt_NamesEveryArrangementOnce()
    {
        var space = new PuzzleSlides<TwoByThree>();
        var arrangements = Enumerable.Range(0, space.StateCount).Select(space.StateAt).ToList();

        Assert.Equal(720, arrangements.Distinct().Count());
        Assert.All(arrangements, board => Assert.Equal("012345", string.Concat(board.Order())));
    }

    [Fact]
    public void WriteSuccessors_AreTheArrangementsOneSlideAway()
    {
        var successors = new string[4];

        var written = new PuzzleSlides<TwoByThree>().WriteSuccessors("412503", successors);

        Assert.Equal(["402513", "412053", "412530"], successors[..written]);
    }

    // LeetCode 773's board.
    private readonly struct TwoByThree : IPuzzleBoard
    {
        public static int Rows => 2;

        public static int Columns => 3;
    }
}
