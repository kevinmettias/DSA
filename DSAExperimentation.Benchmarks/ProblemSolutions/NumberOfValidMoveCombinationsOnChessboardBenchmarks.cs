using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Valid Move Combinations On Chessboard (LC 2056): CartesianProductThenValidate
// is the naive approach - generate every piece's full candidate-move list up front, take
// their cartesian product, and only check the whole simultaneous-movement timeline for
// collisions once a full combination has been assembled. PrunedBacktracking instead uses
// this repo's own Backtrack.Search (the same "fold pruning into Candidates" shape
// BeautifulArrangement/NQueens already use), so a move that would collide with an
// already-chosen piece is never combined with the rest to begin with.
[MemoryDiagnoser]
public class NumberOfValidMoveCombinationsOnChessboardBenchmarks
{
    private const int BoardSize = 8;
    private const string RookPieceType = "rook";
    private const string BishopPieceType = "bishop";
    private const string QueenPieceType = "queen";
    private const string UnknownPieceTypeMessage = "Unknown piece type.";

    private static readonly (int DRow, int DCol)[] RookDirections = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private static readonly (int DRow, int DCol)[] BishopDirections = [(1, 1), (1, -1), (-1, 1), (-1, -1)];
    private static readonly (int DRow, int DCol)[] QueenDirections = [.. RookDirections, .. BishopDirections];

    private static readonly string[] AllPieceTypes = ["rook", "queen", "bishop", "rook"];
    private static readonly int[][] AllPositions = [[1, 1], [8, 8], [1, 8], [8, 1]];

    [Params(2, 4)]
    public int PieceCount;

    private string[] _pieceTypes = null!;
    private int[][] _positions = null!;

    [GlobalSetup]
    public void Setup()
    {
        _pieceTypes = AllPieceTypes[..PieceCount];
        _positions = AllPositions[..PieceCount];
    }

    [Benchmark(Baseline = true)]
    public int CartesianProductThenValidate()
    {
        var perPiece = new List<Move>[_pieceTypes.Length];
        for (var i = 0; i < _pieceTypes.Length; i++)
        {
            perPiece[i] = AllCandidateMoves(_pieceTypes[i], _positions[i]);
        }

        return CountValidCombinations(perPiece, new Move[_pieceTypes.Length], 0);
    }

    [Benchmark]
    public int PrunedBacktracking()
    {
        var count = 0;
        var state = new State(_pieceTypes, _positions);

        Backtrack.Search<State, Move>(
            state,
            s => s.Moves.Count == _pieceTypes.Length,
            s => s.Moves.Count == _pieceTypes.Length ? [] : CandidateMoves(s),
            (s, move) => s.Moves.Add(move),
            (s, _) => s.Moves.RemoveAt(s.Moves.Count - 1),
            _ => count++);

        return count;
    }

    private int CountValidCombinations(List<Move>[] perPiece, Move[] combo, int pieceIndex)
    {
        if (pieceIndex == perPiece.Length)
        {
            return IsCombinationCollisionFree(combo) ? 1 : 0;
        }

        var count = 0;
        foreach (var move in perPiece[pieceIndex])
        {
            combo[pieceIndex] = move;
            count += CountValidCombinations(perPiece, combo, pieceIndex + 1);
        }

        return count;
    }

    private bool IsCombinationCollisionFree(Move[] combo)
    {
        for (var i = 0; i < combo.Length; i++)
        {
            for (var j = i + 1; j < combo.Length; j++)
            {
                if (MovesCollide(_positions[i], combo[i], _positions[j], combo[j]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private List<Move> AllCandidateMoves(string pieceType, int[] start)
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
            if (MovesCollide(candidateStart, candidate, state.Positions[other], state.Moves[other]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool MovesCollide(int[] firstStart, Move first, int[] secondStart, Move second)
    {
        var maxTime = Math.Max(first.Distance, second.Distance);

        for (var t = 1; t <= maxTime; t++)
        {
            if (PositionAt(firstStart, first, t) == PositionAt(secondStart, second, t))
            {
                return true;
            }
        }

        return false;
    }

    private static (int Row, int Col) PositionAt(int[] start, Move move, int t)
    {
        var step = Math.Min(t, move.Distance);
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

    private sealed class State(string[] pieceTypes, int[][] positions)
    {
        public string[] PieceTypes { get; } = pieceTypes;
        public int[][] Positions { get; } = positions;
        public List<Move> Moves { get; } = [];
    }
}
