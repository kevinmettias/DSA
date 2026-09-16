using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.NumberOfValidMoveCombinationsOnChessboard;

// LeetCode 2056. Number of Valid Move Combinations On Chessboard: every piece
// picks one destination reachable along one of its own directions (or stays put),
// all pieces then move simultaneously one square per second, and a combination is
// valid only if no two pieces ever share a square at any second.
//
// Two strategies over the same move enumeration. CartesianProduct is the naive
// answer - build every piece's full candidate list up front, take the cartesian
// product, and only walk the whole simultaneous-movement timeline once a complete
// combination has been assembled. PrunedBacktracking composes this repo's own
// Backtrack.Search (the same "fold pruning into Candidates instead of generating a
// full combination and rejecting it afterward" shape BeautifulArrangement/NQueens
// already use), so a move that would collide with an already-chosen piece is never
// combined with the rest to begin with.
//
// No new primitive is needed: per-piece move enumeration and the
// timestep-by-timestep collision check are problem-specific glue over Backtrack's
// existing choose/explore/unchoose contract, so both the Move choice and the shared
// mutable State live here beside the solution (ARCHITECTURE.md #17.3).
internal static class NumberOfValidMoveCombinationsOnChessboardSolution
{
    private const int BoardSize = 8;
    private const string RookPieceType = "rook";
    private const string BishopPieceType = "bishop";
    private const string QueenPieceType = "queen";
    private const string UnknownPieceTypeMessage = "Unknown piece type.";

    private static readonly (int DRow, int DCol)[] RookDirections = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private static readonly (int DRow, int DCol)[] BishopDirections = [(1, 1), (1, -1), (-1, 1), (-1, -1)];
    private static readonly (int DRow, int DCol)[] QueenDirections = [.. RookDirections, .. BishopDirections];

    // The textbook answer: enumerate each piece's moves independently, form the full
    // cartesian product by plain recursion, and pairwise-validate each assembled
    // combination afterwards. Deliberately written with BCL arrays and lists and no
    // search primitive - it is the arm the composed solution below has to justify
    // itself against.
    public static int CountCombinationsByCartesianProduct(string[] pieces, int[][] positions)
    {
        var perPiece = new List<Move>[pieces.Length];

        for (var i = 0; i < pieces.Length; i++)
        {
            perPiece[i] = AllCandidateMoves(pieces[i], positions[i]);
        }

        return CountValidCombinations(positions, perPiece, new Move[pieces.Length], 0);
    }

    private static List<Move> AllCandidateMoves(string pieceType, int[] start)
    {
        var moves = new List<Move> { new(0, 0, 0) };

        foreach (var (dRow, dCol) in DirectionsFor(pieceType))
        {
            for (var distance = 1; IsOnBoard(start[0] + (dRow * distance), start[1] + (dCol * distance)); distance++)
            {
                moves.Add(new Move(dRow, dCol, distance));
            }
        }

        return moves;
    }

    // This repo's own backtracking engine: Candidates for the next piece only offers
    // (direction, distance) moves whose full simultaneous-movement trajectory never
    // collides, at any time step, with a piece already chosen earlier in the
    // combination - so an invalid combination is never assembled to begin with.
    public static int CountCombinationsByPrunedBacktracking(string[] pieces, int[][] positions)
    {
        var count = 0;
        var state = new State(pieces, positions);

        Backtrack.Search<State, Move>(
            state,
            s => s.Moves.Count == pieces.Length,
            s => s.Moves.Count == pieces.Length ? NoCandidates() : CandidateMoves(s),
            (s, move) => s.Moves.Add(move),
            (s, _) => s.Moves.RemoveAt(s.Moves.Count - 1),
            _ => count++);

        return count;
    }

    // Every piece has been given a move, so the combination has no children left.
    private static IEnumerable<Move> NoCandidates() => [];

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

    private static bool IsCollisionFree(State state, int pieceIndex, Move candidate)
    {
        var candidateStart = state.Positions[pieceIndex];

        for (var other = 0; other < state.Moves.Count; other++)
        {
            if (HasMoveCollision(candidateStart, candidate, state.Positions[other], state.Moves[other]))
            {
                return false;
            }
        }

        return true;
    }

    private static int CountValidCombinations(int[][] positions, List<Move>[] perPiece, Move[] combo, int pieceIndex)
    {
        if (pieceIndex == perPiece.Length)
        {
            return IsCombinationCollisionFree(positions, combo) ? 1 : 0;
        }

        var count = 0;

        foreach (var move in perPiece[pieceIndex])
        {
            combo[pieceIndex] = move;
            count += CountValidCombinations(positions, perPiece, combo, pieceIndex + 1);
        }

        return count;
    }

    private static bool IsCombinationCollisionFree(int[][] positions, Move[] combo)
    {
        for (var i = 0; i < combo.Length; i++)
        {
            for (var j = i + 1; j < combo.Length; j++)
            {
                if (HasMoveCollision(positions[i], combo[i], positions[j], combo[j]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Two trajectories are compared at every shared time step (both pieces move one
    // square per step, then hold their final square) - the only way two pieces can
    // ever occupy the same cell, per the problem's simultaneous-movement rule.
    private static bool HasMoveCollision(int[] firstStart, Move first, int[] secondStart, Move second)
    {
        var maxTime = Math.Max(first.Distance, second.Distance);

        for (var timeStep = 1; timeStep <= maxTime; timeStep++)
        {
            if (PositionAt(firstStart, first, timeStep) == PositionAt(secondStart, second, timeStep))
            {
                return true;
            }
        }

        return false;
    }

    private static (int Row, int Col) PositionAt(int[] start, Move move, int timeStep)
    {
        var step = Math.Min(timeStep, move.Distance);

        return (start[0] + (move.DRow * step), start[1] + (move.DCol * step));
    }

    private static bool IsOnBoard(int row, int col) => row is >= 1 and <= BoardSize && col is >= 1 and <= BoardSize;

    private static (int DRow, int DCol)[] DirectionsFor(string pieceType) => pieceType switch
    {
        RookPieceType => RookDirections,
        BishopPieceType => BishopDirections,
        QueenPieceType => QueenDirections,
        _ => throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, UnknownPieceTypeMessage),
    };

    private readonly record struct Move(int DRow, int DCol, int Distance);

    private sealed record State(string[] PieceTypes, int[][] Positions)
    {
        public List<Move> Moves { get; } = [];
    }
}
