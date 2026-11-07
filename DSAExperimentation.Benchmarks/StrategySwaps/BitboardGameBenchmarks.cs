using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// The two forms of IBitboardGame on a board both hold (ARCHITECTURE 17.10): BitboardGame, the
// template, which judges a claim by the masks of the winning sets through its cell and evaluates by
// reading each set's mask, against InARowGame, the strategy for boards beyond 64 cells, which
// shifts 256-bit masks along the line directions for both. Each arm runs the same alpha-beta, in the
// game's own move order, to the same depth from the same position of the 7,6,4 game under free
// placement (Connect Four's board, no gravity), so the trees are the same, every arm answers alike,
// and only the game and its evaluation differ. The positions are cells claimed from the empty
// board, the players alternating.
public class BitboardGameBenchmarks
{
    private BitboardGame<FreePlacement> _setMasks;
    private OpenLinesEvaluation _setEvaluation;
    private BitboardPosition _setPosition;
    private InARowGame<ConnectFourBoard, KOrMore> _shifts;
    private Bitboard256Position _shiftedPosition;

    internal static readonly int[][] Openings =
    [
        [24, 17, 23, 16, 25, 31],
        [10, 18, 11, 25, 32, 19, 26, 12],
    ];

    [Params(2, 4)]
    public int Depth { get; set; }

    [Params(0, 1)]
    public int Opening { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _setMasks = new(MnkGame<ConnectFourBoard>.Board, MnkGame<ConnectFourBoard>.Lines);
        _setEvaluation = new(_setMasks.WinningSets);
        _setPosition = _setMasks.Replay(Openings[Opening]);
        _shifts = new(MnkGame<ConnectFourBoard>.Board);
        _shiftedPosition = _shifts.Replay(Openings[Opening]);
    }

    [Benchmark(Baseline = true)]
    public int SetMasks() =>
        AlphaBeta.Value<
            BitboardGame<FreePlacement>, OpenLinesEvaluation, MoveNumberOrder<BitboardPosition>,
            BitboardPosition, int>(_setMasks, _setEvaluation, default, _setPosition, Depth);

    [Benchmark]
    public int Shifts() =>
        AlphaBeta.Value<
            InARowGame<ConnectFourBoard, KOrMore>,
            ShiftedOpenLinesEvaluation<ConnectFourBoard, KOrMore>,
            MoveNumberOrder<Bitboard256Position>, Bitboard256Position, int>(
            _shifts, new(_shifts), default, _shiftedPosition, Depth);
}
