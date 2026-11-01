using DSAExperimentation.Domain.TicTacToe;

namespace DSAExperimentation.Tests.Domain.TicTacToe;

public sealed partial class TicTacToeRulesTests
{
    public static TheoryData<int, TicTacToePlayer> MovesToTheirPlayers =>
        new() { { 0, TicTacToePlayer.A }, { 1, TicTacToePlayer.B }, { 2, TicTacToePlayer.A }, { 7, TicTacToePlayer.B } };

    public static TheoryData<int, int> SquaresOnTheMainDiagonal => new() { { 0, 0 }, { 1, 1 }, { 2, 2 } };

    public static TheoryData<int, int> SquaresOffTheMainDiagonal => new() { { 0, 1 }, { 2, 0 }, { 0, 2 } };

    public static TheoryData<int, int> SquaresOnTheAntiDiagonal => new() { { 0, 2 }, { 1, 1 }, { 2, 0 } };

    public static TheoryData<int, int> SquaresOffTheAntiDiagonal => new() { { 0, 0 }, { 2, 2 }, { 1, 0 } };

    // The ninth-move cases pin the rule's order: a completed line wins even on the board's last square.
    public static TheoryData<TicTacToePlayer?, int, TicTacToeOutcome> GamesToTheirOutcomes =>
        new()
        {
            { TicTacToePlayer.A, 5, TicTacToeOutcome.AWins },
            { TicTacToePlayer.B, 6, TicTacToeOutcome.BWins },
            { TicTacToePlayer.A, 9, TicTacToeOutcome.AWins },
            { null, 9, TicTacToeOutcome.Draw },
            { null, 4, TicTacToeOutcome.Pending },
            { null, 0, TicTacToeOutcome.Pending },
        };

    [Theory]
    [MemberData(nameof(MovesToTheirPlayers))]
    public void PlayerMaking_AMovesFirstAndThePlayersAlternate(int moveIndex, TicTacToePlayer expected) =>
        Assert.Equal(expected, TicTacToeRules.PlayerMaking(moveIndex));

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

    [Theory]
    [MemberData(nameof(GamesToTheirOutcomes))]
    public void OutcomeOf_AWinBeatsAFullBoard_ElseFullIsADrawAndShortIsPending(
        TicTacToePlayer? winner, int moveCount, TicTacToeOutcome expected) =>
        Assert.Equal(expected, TicTacToeRules.OutcomeOf(winner, moveCount));
}
