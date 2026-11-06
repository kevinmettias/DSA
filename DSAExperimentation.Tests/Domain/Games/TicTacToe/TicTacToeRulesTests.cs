using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.TicTacToe;
using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Tests.Domain.Games.TicTacToe;

public sealed partial class TicTacToeRulesTests
{
    private const int Squares = TicTacToeRules.BoardSize * TicTacToeRules.BoardSize;

    public static TheoryData<int, TicTacToePlayer> MovesToTheirPlayers =>
        new() { { 0, TicTacToePlayer.X }, { 1, TicTacToePlayer.O }, { 2, TicTacToePlayer.X }, { 7, TicTacToePlayer.O } };

    public static TheoryData<int, int> SquaresOnTheMainDiagonal => new() { { 0, 0 }, { 1, 1 }, { 2, 2 } };

    public static TheoryData<int, int> SquaresOffTheMainDiagonal => new() { { 0, 1 }, { 2, 0 }, { 0, 2 } };

    public static TheoryData<int, int> SquaresOnTheAntiDiagonal => new() { { 0, 2 }, { 1, 1 }, { 2, 0 } };

    public static TheoryData<int, int> SquaresOffTheAntiDiagonal => new() { { 0, 0 }, { 2, 2 }, { 1, 0 } };

    // The ninth-move cases pin the rule's order: a completed line wins even on the board's last square.
    public static TheoryData<TicTacToePlayer?, int, TicTacToeOutcome> GamesToTheirOutcomes =>
        new()
        {
            { TicTacToePlayer.X, 5, TicTacToeOutcome.XWins },
            { TicTacToePlayer.O, 6, TicTacToeOutcome.OWins },
            { TicTacToePlayer.X, 9, TicTacToeOutcome.XWins },
            { null, 9, TicTacToeOutcome.Draw },
            { null, 4, TicTacToeOutcome.Pending },
            { null, 0, TicTacToeOutcome.Pending },
        };

    [Theory]
    [MemberData(nameof(MovesToTheirPlayers))]
    public void PlayerMaking_AMovesFirstAndThePlayersAlternate(int moveIndex, TicTacToePlayer expected) =>
        Assert.Equal(expected, TicTacToeRules.PlayerMaking(moveIndex));

    [Fact]
    public void BoardSize_IsTheBoardsWidth() =>
        Assert.Equal(TicTacToeRules.BoardSize, TicTacToeBoard.Columns);

    // X sits in the first seat of two players alternating, O in the second.
    [Fact]
    public void PlayerMaking_XHoldsTheFirstSeatOfTwoPlayersAlternating() =>
        Assert.All(Enumerable.Range(0, Squares), move => Assert.Equal(
            TwoPlayersAlternating.SeatMaking(move) == TwoPlayersAlternating.First,
            TicTacToeRules.PlayerMaking(move) == TicTacToePlayer.X));

    [Theory]
    [MemberData(nameof(SquaresOnTheMainDiagonal))]
    public void IsOnDiagonal_SquareOnIt_ReturnsTrue(int row, int column) =>
        Assert.True(TicTacToeRules.IsOnDiagonal(row, column));

    [Theory]
    [MemberData(nameof(SquaresOffTheMainDiagonal))]
    public void IsOnDiagonal_SquareOffIt_ReturnsFalse(int row, int column) =>
        Assert.False(TicTacToeRules.IsOnDiagonal(row, column));

    [Theory]
    [MemberData(nameof(SquaresOnTheAntiDiagonal))]
    public void IsOnAntiDiagonal_SquareOnIt_ReturnsTrue(int row, int column) =>
        Assert.True(TicTacToeRules.IsOnAntiDiagonal(row, column));

    [Theory]
    [MemberData(nameof(SquaresOffTheAntiDiagonal))]
    public void IsOnAntiDiagonal_SquareOffIt_ReturnsFalse(int row, int column) =>
        Assert.False(TicTacToeRules.IsOnAntiDiagonal(row, column));

    public static TheoryData<PositionalOutcome, TicTacToeOutcome> PositionalOutcomesToTicTacToes =>
        new()
        {
            { PositionalOutcome.WonBy(TwoPlayersAlternating.First), TicTacToeOutcome.XWins },
            { PositionalOutcome.WonBy(TwoPlayersAlternating.Second), TicTacToeOutcome.OWins },
            { PositionalOutcome.Drawn, TicTacToeOutcome.Draw },
            { PositionalOutcome.Pending, TicTacToeOutcome.Pending },
        };

    [Fact]
    public void Game_IsTheWholeBoardWithItsEightLines() =>
        Assert.Equal(
            [Squares, 8],
            [TicTacToeRules.Game.Board.PlayableCount, TicTacToeRules.Game.WinningSets.Count]);

    [Theory]
    [MemberData(nameof(PositionalOutcomesToTicTacToes))]
    public void OutcomeOf_APositionalOutcome_NamesTheFirstSeatX(
        PositionalOutcome outcome, TicTacToeOutcome expected) =>
        Assert.Equal(expected, TicTacToeRules.OutcomeOf(outcome));

    [Theory]
    [MemberData(nameof(GamesToTheirOutcomes))]
    public void OutcomeOf_AWinBeatsAFullBoard_ElseFullIsADrawAndShortIsPending(
        TicTacToePlayer? winner, int moveCount, TicTacToeOutcome expected) =>
        Assert.Equal(expected, TicTacToeRules.OutcomeOf(winner, moveCount));
}
