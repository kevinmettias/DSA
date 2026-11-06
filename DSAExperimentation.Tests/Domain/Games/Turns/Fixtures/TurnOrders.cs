using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Tests.Domain.Games.Turns.Fixtures;

// Reads a turn order through its contract, as a game generic over it does, so each order's tests
// compare whole runs of moves rather than one move at a time.
internal static class TurnOrders
{
    // The seat making each of the first moves.
    public static int[] Seats<TTurns>(int moves)
        where TTurns : struct, ITurnOrder
        => [.. Enumerable.Range(0, moves).Select(move => TTurns.SeatMaking(move))];

    // The turn each of the first moves belongs to.
    public static int[] Turns<TTurns>(int moves)
        where TTurns : struct, ITurnOrder
        => [.. Enumerable.Range(0, moves).Select(move => TTurns.TurnOf(move))];
}
