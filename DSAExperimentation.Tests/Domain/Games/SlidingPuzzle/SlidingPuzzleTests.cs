using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.States;
using DSAExperimentation.Domain.Games.SlidingPuzzle;
using DSAExperimentation.Domain.Mathematics.Groups;

namespace DSAExperimentation.Tests.Domain.Games.SlidingPuzzle;

// The general puzzle read through three boards: LeetCode 773's 2 by 3, and the 8- and 15-puzzles.
public sealed partial class SlidingPuzzleTests
{
    // An arrangement and how many slides away LC 773's examples put it from solved, or -1.
    public static TheoryData<string> SolvableTwoByThreeBoards => new() { "123450", "412503", "123405" };

    public static TheoryData<string> UnsolvableTwoByThreeBoards => new() { "123540", "213450" };

    [Fact]
    public void Solved_ReadsTheTilesInOrderWithTheEmptyCellLast()
    {
        Assert.Equal("123450", SlidingPuzzle<TwoByThree>.Solved);
        Assert.Equal("123456780", SlidingPuzzle<EightPuzzle>.Solved);
        Assert.Equal("123456789ABCDEF0", SlidingPuzzle<FifteenPuzzle>.Solved);
    }

    [Fact]
    public void CellCount_IsRowsTimesColumns() => Assert.Equal(16, SlidingPuzzle<FifteenPuzzle>.CellCount);

    [Fact]
    public void CellSymbols_AreTheEmptyCellsThenEachTiles() => Assert.Equal("012345", SlidingPuzzle<TwoByThree>.CellSymbols);

    [Fact]
    public void MostSlides_IsOnePerNeighbourInsideTheBoard() => Assert.Equal(4, SlidingPuzzle<EightPuzzle>.MostSlides);

    // A corner has two neighbours, an edge three, the middle four, each listed in cell order.
    [Fact]
    public void SlideSources_AreTheEmptyCellsNeighboursInCellOrder()
    {
        Assert.Equal([1, 3], SlideSources<EightPuzzle>(0));
        Assert.Equal([0, 2, 4], SlideSources<EightPuzzle>(1));
        Assert.Equal([1, 3, 5, 7], SlideSources<EightPuzzle>(4));
        Assert.Equal([5, 7], SlideSources<EightPuzzle>(8));
    }

    [Fact]
    public void WriteOneSlideAway_TradesTheEmptyCellWithEachNeighbour()
    {
        var into = new string[SlidingPuzzle<TwoByThree>.MostSlides];

        var written = SlidingPuzzle<TwoByThree>.WriteOneSlideAway("123450", into);

        Assert.Equal(["120453", "123405"], into[..written]);
    }

    [Fact]
    public void InversionCount_OfTheSolvedBoard_IsNone() => Assert.Equal(0, SlidingPuzzle<FifteenPuzzle>.InversionCount("123456789ABCDEF0"));

    [Fact]
    public void InversionCount_LeavesTheEmptyCellOut() => Assert.Equal(1, SlidingPuzzle<TwoByThree>.InversionCount("213045"));

    [Fact]
    public void TilePermutation_OfTheSolvedBoard_IsTheIdentity() =>
        Assert.Equal(
            Permutation.Identity(15),
            SlidingPuzzle<FifteenPuzzle>.TilePermutation("123456789ABCDEF0"));

    // Tiles 14 and 15 are points 13 and 14, read in each other's places.
    [Fact]
    public void TilePermutation_OfLoydsFourteenFifteen_SwapsTheLastTwoTiles() =>
        Assert.Equal(
            Permutation.FromCycles(15, [13, 14]),
            SlidingPuzzle<FifteenPuzzle>.TilePermutation("123456789ABCDFE0"));

    [Fact]
    public void TilePermutation_LeavesTheEmptyCellOut() =>
        Assert.Equal(
            Permutation.FromImages([1, 0, 2, 3, 4]),
            SlidingPuzzle<TwoByThree>.TilePermutation("213045"));

    [Fact]
    public void TilePermutation_IsEvenExactlyWhenTheInversionsAre()
    {
        var space = new PuzzleSlides<TwoByThree>();

        Assert.All(Enumerable.Range(0, space.StateCount).Select(space.StateAt), board =>
            Assert.Equal(
                int.IsEvenInteger(SlidingPuzzle<TwoByThree>.InversionCount(board)),
                PermutationCycles.IsEven(SlidingPuzzle<TwoByThree>.TilePermutation(board))));
    }

    [Fact]
    public void TilePermutation_WithATileTwice_Throws() =>
        Assert.Throws<ArgumentException>(() => SlidingPuzzle<TwoByThree>.TilePermutation("113045"));

    [Theory]
    [MemberData(nameof(SolvableTwoByThreeBoards))]
    public void IsSolvable_OnAnOddWidth_WhenTheInversionsAreEven(string board) => Assert.True(SlidingPuzzle<TwoByThree>.IsSolvable(board));

    [Theory]
    [MemberData(nameof(UnsolvableTwoByThreeBoards))]
    public void IsSolvable_OnAnOddWidth_NotWhenTheInversionsAreOdd(string board) => Assert.False(SlidingPuzzle<TwoByThree>.IsSolvable(board));

    // Sam Loyd's 14-15 puzzle: the solved board with its last two tiles swapped, which no sequence
    // of slides undoes.
    [Fact]
    public void IsSolvable_OnAnEvenWidth_NotLoydsFourteenFifteen() => Assert.False(SlidingPuzzle<FifteenPuzzle>.IsSolvable("123456789ABCDFE0"));

    // Moving the empty cell up a row flips the inversions' parity and its row's together.
    [Fact]
    public void IsSolvable_OnAnEvenWidth_TheSolvedBoardOneSlideUp() => Assert.True(SlidingPuzzle<FifteenPuzzle>.IsSolvable("123456789AB0DEFC"));

    // The law's oracle: the arrangements slides reach from solved are exactly those it calls
    // solvable, half of the 720. On 3 by 2 the width is even and the rows odd, so only counting the
    // empty cell's row from the bottom gets it right.
    [Fact]
    public void IsSolvable_OnAnOddWidth_IsExactlyWhatSlidesReachFromSolved() => AssertTheLawIsWhatSlidesReach<TwoByThree>();

    [Fact]
    public void IsSolvable_OnAnEvenWidth_IsExactlyWhatSlidesReachFromSolved() => AssertTheLawIsWhatSlidesReach<ThreeByTwo>();

    private static void AssertTheLawIsWhatSlidesReach<TBoard>()
        where TBoard : struct, IPuzzleBoard
    {
        var space = new PuzzleSlides<TBoard>();
        var graph = StateGraph<string>.Build(space, new EveryState<string>());
        Assert.True(graph.TryGetNode(SlidingPuzzle<TBoard>.Solved, out var solved));

        var reached = BreadthFirstDistances.From<StateNode<string>, StateTopology<string>, ListChildren<StateNode<string>>>(solved)
            .Keys.Select(node => node.State).ToHashSet();
        var solvable = Enumerable.Range(0, space.StateCount).Select(space.StateAt).Where(SlidingPuzzle<TBoard>.IsSolvable).ToHashSet();

        Assert.Equal(360, solvable.Count);
        Assert.True(reached.SetEquals(solvable));
    }

    private static int[] SlideSources<TBoard>(int empty)
        where TBoard : struct, IPuzzleBoard
        => SlidingPuzzle<TBoard>.SlideSources(empty).ToArray();

    // LeetCode 773's board.
    private readonly struct TwoByThree : IPuzzleBoard
    {
        public static int Rows => 2;

        public static int Columns => 3;
    }

    // The same cells stood on end: an even width and an odd number of rows.
    private readonly struct ThreeByTwo : IPuzzleBoard
    {
        public static int Rows => 3;

        public static int Columns => 2;
    }
}
