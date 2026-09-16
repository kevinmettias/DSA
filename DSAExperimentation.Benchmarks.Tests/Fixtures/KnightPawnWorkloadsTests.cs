using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for KnightPawnWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3283's game
// being a knight and a scattering of pawns on distinct squares of the problem's fixed board, sized
// within its own position bound.
public sealed partial class KnightPawnWorkloadsTests
{
    private const int PawnCount = 8;
    private const int Seed = 3283; // LC problem number
    private const int BoardSize = 50;
    private const int PositionFieldCount = 2; // Row, Col
    private const int KnightSquareCount = 1;

    [Fact]
    public void BuildGame_PawnCount_ReturnsOnePositionPerPawn()
    {
        var (_, _, positions) = KnightPawnWorkloads.BuildGame(PawnCount, Seed);

        Assert.Equal(PawnCount, positions.Length);
        Assert.All(positions, position => Assert.Equal(PositionFieldCount, position.Length));
    }

    [Fact]
    public void BuildGame_EverySquare_StaysOnTheFixedBoard()
    {
        var (kx, ky, positions) = KnightPawnWorkloads.BuildGame(PawnCount, Seed);

        Assert.InRange(kx, 0, BoardSize - 1);
        Assert.InRange(ky, 0, BoardSize - 1);
        Assert.All(positions, position => Assert.InRange(position[0], 0, BoardSize - 1));
        Assert.All(positions, position => Assert.InRange(position[1], 0, BoardSize - 1));
    }

    // The knight and every pawn have to sit on a square of their own: two pieces on one square is an
    // input LC 3283's own move resolution would never see.
    [Fact]
    public void BuildGame_KnightAndPawns_OccupyDistinctSquares()
    {
        var (kx, ky, positions) = KnightPawnWorkloads.BuildGame(PawnCount, Seed);
        var squares = positions.Select(position => (position[0], position[1])).ToHashSet();

        squares.Add((kx, ky));

        Assert.Equal(PawnCount + KnightSquareCount, squares.Count);
    }

    [Fact]
    public void BuildGame_SameSeed_ReturnsTheSameGame()
    {
        var (kx, ky, positions) = KnightPawnWorkloads.BuildGame(PawnCount, Seed);
        var (repeatKx, repeatKy, repeatPositions) = KnightPawnWorkloads.BuildGame(PawnCount, Seed);

        Assert.Equal(kx, repeatKx);
        Assert.Equal(ky, repeatKy);
        Assert.Equal(AnswerText.Of(positions), AnswerText.Of(repeatPositions));
    }
}
