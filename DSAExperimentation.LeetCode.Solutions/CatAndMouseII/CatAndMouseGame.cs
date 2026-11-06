using DSAExperimentation.Domain.Games.Turns;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.LeetCode.Solutions.CatAndMouseII;

// LC 1728's game as Domain's game graph over the walkable cells' numbers, for
// CatAndMouseIISolution's retrograde analysis. A position is numbered by the mouse's cell, then
// the cat's, then the mover. It is over once the cat is on the mouse or either animal is on the
// food, and only the mouse on the food, uncaught, is the mouse's win. Each animal's moves from
// every cell are fixed before play, so a move is a lookup.
internal readonly struct CatAndMouseGame(int[][] mouseMoves, int[][] catMoves, int food)
    : IGameGraph<CatAndMousePosition>
{
    // The mouse moves first.
    public const int MouseSeat = TwoPlayersAlternating.First;
    public const int CatSeat = TwoPlayersAlternating.Second;

    private int CellCount => mouseMoves.Length;

    public int PositionCount => CellCount * CellCount * TwoPlayersAlternating.PlayerCount;

    public CatAndMousePosition PositionAt(int index)
    {
        var cells = index / TwoPlayersAlternating.PlayerCount;
        var mover = index % TwoPlayersAlternating.PlayerCount;

        return new CatAndMousePosition(cells / CellCount, cells % CellCount, mover);
    }

    public int IndexOf(CatAndMousePosition position)
    {
        var cells = (position.Mouse * CellCount) + position.Cat;

        return (cells * TwoPlayersAlternating.PlayerCount) + position.Mover;
    }

    public int MoveCount(CatAndMousePosition position)
    {
        if (IsOver(position))
        {
            return 0;
        }

        if (position.Mover == MouseSeat)
        {
            return mouseMoves[position.Mouse].Length;
        }

        return catMoves[position.Cat].Length;
    }

    private bool IsOver(CatAndMousePosition position) =>
        position.Cat == position.Mouse || position.Mouse == food || position.Cat == food;

    public CatAndMousePosition Next(CatAndMousePosition position, int move)
    {
        if (position.Mover == MouseSeat)
        {
            return position with { Mouse = mouseMoves[position.Mouse][move], Mover = CatSeat };
        }

        return position with { Cat = catMoves[position.Cat][move], Mover = MouseSeat };
    }

    public GameOutcome OutcomeWhenOver(CatAndMousePosition position)
    {
        var winner = IsMouseFed(position) ? MouseSeat : CatSeat;

        return position.Mover == winner ? GameOutcome.Win : GameOutcome.Loss;
    }

    private bool IsMouseFed(CatAndMousePosition position) =>
        position.Mouse == food && position.Cat != position.Mouse;
}
