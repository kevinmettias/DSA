using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfValidMoveCombinationsOnChessboard;

// LeetCode 2056. Number of Valid Move Combinations On Chessboard: this repo's own
// Backtrack.Search engine (the same "fold pruning into Candidates instead of
// generating a full combination and rejecting it afterward" shape
// BeautifulArrangement/NQueens already use). Candidates for the next piece only
// offers (direction, distance) moves whose full simultaneous-movement trajectory
// never collides, at any time step, with a piece already chosen earlier in the
// combination - so an invalid combination is never assembled to begin with. No new
// primitive needed: per-piece move enumeration and the timestep-by-timestep
// collision check are problem-specific glue over Backtrack's existing
// choose/explore/unchoose contract.
public sealed partial class NumberOfValidMoveCombinationsOnChessboardTests
{
    private static readonly (int DRow, int DCol)[] RookDirections = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private static readonly (int DRow, int DCol)[] BishopDirections = [(1, 1), (1, -1), (-1, 1), (-1, -1)];
    private static readonly (int DRow, int DCol)[] QueenDirections = [.. RookDirections, .. BishopDirections];

    [Fact]
    public void CountCombinations_SoloRookInCorner_ReturnsFifteen()
    {
        var actual = CountCombinations(["rook"], [[1, 1]]);
        Assert.Equal(15, actual);
    }

    [Fact]
    public void CountCombinations_SoloBishopInCorner_ReturnsEight()
    {
        var actual = CountCombinations(["bishop"], [[1, 1]]);
        Assert.Equal(8, actual);
    }

    [Fact]
    public void CountCombinations_SoloQueenInCorner_ReturnsTwentyTwo()
    {
        var actual = CountCombinations(["queen"], [[1, 1]]);
        Assert.Equal(22, actual);
    }

    [Fact]
    public void CountCombinations_TwoRooksOppositeCorners_ReturnsTwoHundredTwentyThree()
    {
        var actual = CountCombinations(["rook", "rook"], [[1, 1], [8, 8]]);
        Assert.Equal(223, actual);
    }

    [Fact]
    public void CountCombinations_RookAndBishopOppositeCorners_ReturnsOneHundredNineteen()
    {
        var actual = CountCombinations(["rook", "bishop"], [[1, 1], [8, 8]]);
        Assert.Equal(119, actual);
    }

    private static int CountCombinations(string[] pieceTypes, int[][] positions)
    {
        var count = 0;
        var state = new State(pieceTypes, positions);

        Backtrack.Search<State, Move>(
            state,
            s => s.Moves.Count == pieceTypes.Length,
            s => s.Moves.Count == pieceTypes.Length ? [] : CandidateMoves(s),
            (s, move) => s.Moves.Add(move),
            (s, _) => s.Moves.RemoveAt(s.Moves.Count - 1),
            _ => count++);

        return count;
    }

    private static IEnumerable<Move> CandidateMoves(State state)
    {
        var pieceIndex = state.Moves.Count;
        var start = state.Positions[pieceIndex];

        if (IsCollisionFree(state, pieceIndex, new Move(0, 0, 0)))
        {
            yield return new Move(0, 0, 0);
        }

        foreach (var (dRow, dCol) in DirectionsFor(state.PieceTypes[pieceIndex]))
        {
            for (var distance = 1; IsOnBoard(start[0] + (dRow * distance), start[1] + (dCol * distance)); distance++)
            {
                var move = new Move(dRow, dCol, distance);
                if (IsCollisionFree(state, pieceIndex, move))
                {
                    yield return move;
                }
            }
        }
    }

    // Every already-chosen piece's trajectory is checked at every shared time step
    // (both pieces move one square per step, then hold their final square) - the
    // only way two pieces can ever occupy the same cell, per the problem's
    // simultaneous-movement rule.
    private static bool IsCollisionFree(State state, int pieceIndex, Move candidate)
    {
        var candidateStart = state.Positions[pieceIndex];

        for (var other = 0; other < state.Moves.Count; other++)
        {
            var existingMove = state.Moves[other];
            var existingStart = state.Positions[other];
            var maxTime = Math.Max(candidate.Distance, existingMove.Distance);

            for (var t = 1; t <= maxTime; t++)
            {
                if (PositionAt(candidateStart, candidate, t) == PositionAt(existingStart, existingMove, t))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static (int Row, int Col) PositionAt(int[] start, Move move, int t)
    {
        var step = Math.Min(t, move.Distance);
        return (start[0] + (move.DRow * step), start[1] + (move.DCol * step));
    }

    private static bool IsOnBoard(int row, int col) => row is >= 1 and <= 8 && col is >= 1 and <= 8;

    private static (int DRow, int DCol)[] DirectionsFor(string pieceType) => pieceType switch
    {
        "rook" => RookDirections,
        "bishop" => BishopDirections,
        "queen" => QueenDirections,
        _ => throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, "Unknown piece type."),
    };

    private readonly record struct Move(int DRow, int DCol, int Distance);

    private sealed class State(string[] pieceTypes, int[][] positions)
    {
        public string[] PieceTypes { get; } = pieceTypes;
        public int[][] Positions { get; } = positions;
        public List<Move> Moves { get; } = [];
    }
}
