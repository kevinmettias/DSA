using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;
using ConnectFourGame = DSAExperimentation.Domain.Games.PositionalGames.BitboardGame<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.Gravity<
        DSAExperimentation.Domain.Games.PositionalGames.InARow.ConnectFourBoard>>;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// Connect Four's strong play, alpha-beta with the open-lines evaluation and the centre-first
// order, which these tests drive together. Positions are columns played from the empty board,
// first player first, rows numbered from the top:
// - win in one: the first player holds three in the centre column;
// - win in two: the first player holds the centre two of the bottom row, so either neighbour
//   makes an open three the second player cannot block at both ends;
// - block: the second player holds three in the centre column, and any other move loses at once.
public sealed partial class ConnectFourPlayTests
{
    private const int Centre = 3;
    private const int WinValue = BitboardGame<FreePlacement>.WinValue;

    private static readonly int[] WinInOne = [Centre, 4, Centre, 5, Centre, 4];
    private static readonly int[] WinInTwo = [2, 2, Centre, Centre];
    private static readonly int[] MustBlock = [0, Centre, 0, Centre, 6, Centre];

    public static TheoryData<int> Depths => new() { 1, 2, 4, 6 };

    public static TheoryData<int> DepthsFromTwo => new() { 2, 4, 6 };

    public static TheoryData<int> DepthsFromThree => new() { 3, 4, 6 };

    private static ConnectFourGame ConnectFour =>
        new(MnkGame<ConnectFourBoard>.Board, MnkGame<ConnectFourBoard>.Lines);

    [Theory]
    [MemberData(nameof(Depths))]
    public void BestMove_ThreeInAColumn_CompletesTheFour(int depth) =>
        Assert.Equal((Centre, WinValue - (WinInOne.Length + 1)), BestColumn(WinInOne, depth));

    [Theory]
    [MemberData(nameof(DepthsFromThree))]
    public void BestMove_TwoInTheBottomRow_MakesAThreatOpenAtBothEnds(int depth)
    {
        var (column, value) = BestColumn(WinInTwo, depth);

        Assert.Equal((true, WinValue - (WinInTwo.Length + 3)), (column is 1 or 4, value));
    }

    [Theory]
    [MemberData(nameof(DepthsFromTwo))]
    public void BestMove_OpponentsThreeInAColumn_IsBlocked(int depth) =>
        Assert.Equal(Centre, BestColumn(MustBlock, depth).Column);

    [Theory]
    [MemberData(nameof(DepthsFromThree))]
    public void BestMoveByDeepening_FindsTheSameWinInTwo(int depth)
    {
        var game = ConnectFour;
        var position = game.Replay(WinInTwo);
        var (move, value) = AlphaBeta.BestMoveByDeepening<
            ConnectFourGame, OpenLinesEvaluation,
            CentreFirstColumns<ConnectFourBoard>, BitboardPosition, int>(
            game, new(game.WinningSets), new(game), position, depth);

        var column = game.ChoiceOf(position, move);

        Assert.Equal((true, WinValue - (WinInTwo.Length + 3)), (column is 1 or 4, value));
    }

    private static (int Column, int Value) BestColumn(int[] opening, int depth)
    {
        var game = ConnectFour;
        var position = game.Replay(opening);
        var (move, value) = AlphaBeta.BestMove<
            ConnectFourGame, OpenLinesEvaluation,
            CentreFirstColumns<ConnectFourBoard>, BitboardPosition, int>(
            game, new(game.WinningSets), new(game), position, depth);

        return (game.ChoiceOf(position, move), value);
    }

    // On the 4 by 4 board with three in a row and the 5 by 4 board with four, both small enough to
    // solve, a search to the end agrees with negamax's exact value.
    public sealed partial class SmallBoardsTests
    {
        // Columns played on the 5 by 4 board, each leaving a game to finish.
        public static TheoryData<int[]> FiveByFourOpenings => new()
        {
            { [2, 2, 1, 3, 3, 1, 2, 0] },
            { [2, 1, 2, 3, 0, 4, 2, 2, 1] },
            { [0, 4, 1, 3, 2, 2, 3, 1, 0, 4] },
        };

        [Fact]
        public void Value_FourByFourToTheEnd_IsNegamaxsExactValue()
        {
            var game = SmallGame<FourByFourBoard>();

            Assert.Equal(ExactValue(game, default), ValueToTheEnd(game, default));
        }

        // The early passes stop short of the end and take estimates, but the last reaches the end.
        [Fact]
        public void BestMoveByDeepening_FourByFourToTheEnd_HasNegamaxsExactValue()
        {
            var game = SmallGame<FourByFourBoard>();
            var cells = MnkGame<FourByFourBoard>.CellCount;
            var (_, value) = AlphaBeta.BestMoveByDeepening<
                BitboardGame<Gravity<FourByFourBoard>>, ZeroEvaluation<BitboardPosition>,
                CentreFirstColumns<FourByFourBoard>, BitboardPosition, int>(
                game, default, new(game), default, cells);

            Assert.Equal(ExactValue(game, default), value);
        }

        [Theory]
        [MemberData(nameof(FiveByFourOpenings))]
        public void Value_FiveByFourMidGameToTheEnd_IsNegamaxsExactValue(int[] opening)
        {
            var game = SmallGame<FiveByFourBoard>();
            var position = game.Replay(opening);

            Assert.Equal(ExactValue(game, position), ValueToTheEnd(game, position));
        }

        private static BitboardGame<Gravity<TShape>> SmallGame<TShape>()
            where TShape : struct, IMnkShape
            => new(MnkGame<TShape>.Board, MnkGame<TShape>.Lines);

        private static int ExactValue<TShape>(
            BitboardGame<Gravity<TShape>> game, BitboardPosition position)
            where TShape : struct, IMnkShape
            => Negamax.Value<BitboardGame<Gravity<TShape>>, BitboardPosition, int>(game, position);

        // Searched as deep as the board has cells, so every line of play ends before the depth
        // does.
        private static int ValueToTheEnd<TShape>(
            BitboardGame<Gravity<TShape>> game, BitboardPosition position)
            where TShape : struct, IMnkShape
            => AlphaBeta.Value<
                BitboardGame<Gravity<TShape>>, UnreachedEvaluation<BitboardPosition>,
                CentreFirstColumns<TShape>, BitboardPosition, int>(
                game, default, new(game), position, MnkGame<TShape>.CellCount);
    }
}
