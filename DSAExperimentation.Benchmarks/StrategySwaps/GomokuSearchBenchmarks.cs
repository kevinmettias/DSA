using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// Gomoku's strong play against the generic solver it improves on (ARCHITECTURE 17.10): on positions
// where the player to move has a forced win, alpha-beta with the threats tried first and the
// shifted open-lines evaluation, searched exactly as deep as the shortest win, against victory by
// continuous fours. Both answer whether the player to move has a forced win, so they agree; the
// first must look at every reply within the depth, the second only at the attacker's fours. The
// positions, X to move on the 15 by 15 board, as (row, column):
// - a double four, won in three plies;
// - a four whose forced block makes the defender a four, answered by a four that takes its
//   winning cell and is open at both ends: won in five plies.
public class GomokuSearchBenchmarks
{
    private static readonly (int Row, int Column)[][] Attackers =
    [
        [(4, 3), (4, 7), (4, 9), (5, 4), (5, 7), (5, 8), (6, 5), (7, 4), (7, 5)],
        [(4, 10), (5, 9), (7, 4), (7, 5), (7, 6), (8, 7), (9, 7), (11, 8)],
    ];

    private static readonly (int Row, int Column)[][] Defenders =
    [
        [(0, 0), (0, 14), (3, 2), (3, 10), (10, 12), (12, 1), (13, 7), (14, 0), (14, 14)],
        [(7, 3), (8, 8), (9, 8), (10, 8)],
    ];

    // How many plies deep each position's shortest win lies.
    private static readonly int[] WinDepths = [3, 5];

    private InARowGame<GomokuBoard, KOrMore> _game;
    private Bitboard256Position _position;

    [Params(0, 1)]
    public int Position { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _game = new(MnkGame<GomokuBoard>.Board);
        _position = new(Mask(Attackers[Position]), Mask(Defenders[Position]), IsLost: false);
    }

    private static Bitboard256 Mask((int Row, int Column)[] cells) =>
        InARowLines<GomokuBoard, KOrMore>.MaskOf(
            [.. cells.Select(cell => MnkGame<GomokuBoard>.CellAt(cell.Row, cell.Column))]);

    [Benchmark(Baseline = true)]
    public bool IsWonByAlphaBeta() =>
        AlphaBeta.Value<
            InARowGame<GomokuBoard, KOrMore>, ShiftedOpenLinesEvaluation<GomokuBoard, KOrMore>,
            ThreatsFirstCells<GomokuBoard, KOrMore>, Bitboard256Position, int>(
            _game, new(_game), new(_game), _position, WinDepths[Position])
        > InARowGame<GomokuBoard, KOrMore>.WinValue / 2;

    [Benchmark]
    public bool IsWonByContinuousFours() =>
        ContinuousFours.TryFindWin(_game, _position, out _);
}
