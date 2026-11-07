using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// Connect Four's strong play against the generic solver it improves on, every arm valuing the same
// position to the same depth with the same open-lines evaluation, so every arm answers alike and
// only the search differs (ARCHITECTURE 17.10):
// - plain negamax (Negamax.ValueToDepth), every move of every position within the depth;
// - alpha-beta trying moves in the game's order, columns left to right: the cutoff alone;
// - alpha-beta trying the centre first: the cutoff and Connect Four's move order;
// - iterative deepening with a transposition table over the centre-first order.
// The recursion is one (DepthLimitedSearch), with the cutoff, the order and the table as its
// axes, so each arm adds one thing to the one before. The positions are columns played from the
// empty board, three in the middle of a game and one the second player has already lost, where the
// search can stop as soon as it finds the first player's double threat.
public class ConnectFourSearchBenchmarks
{
    private BitboardGame<Gravity<ConnectFourBoard>> _game;
    private OpenLinesEvaluation _evaluation;
    private BitboardPosition _position;

    internal static readonly int[][] Openings =
    [
        [3, 3, 2, 4, 4, 2, 3, 5],
        [3, 2, 3, 3, 4, 4, 2, 5, 1, 0],
        [2, 3, 4, 3, 3, 2, 4, 4, 5, 1, 6, 0],
        [3, 3, 3, 3, 2, 2, 4],
    ];

    [Params(4, 6)]
    public int Depth { get; set; }

    [Params(0, 1, 2, 3)]
    public int Opening { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _game = new(MnkGame<ConnectFourBoard>.Board, MnkGame<ConnectFourBoard>.Lines);
        _evaluation = new(_game.WinningSets);
        _position = _game.Replay(Openings[Opening]);
    }

    [Benchmark(Baseline = true)]
    public int PlainNegamax() =>
        Negamax.ValueToDepth<
            BitboardGame<Gravity<ConnectFourBoard>>, OpenLinesEvaluation, BitboardPosition, int>(
            _game, _evaluation, _position, Depth);

    [Benchmark]
    public int AlphaBetaLeftToRight() =>
        AlphaBeta.Value<
            BitboardGame<Gravity<ConnectFourBoard>>, OpenLinesEvaluation,
            MoveNumberOrder<BitboardPosition>, BitboardPosition, int>(
            _game, _evaluation, default, _position, Depth);

    [Benchmark]
    public int AlphaBetaCentreFirst() =>
        AlphaBeta.Value<
            BitboardGame<Gravity<ConnectFourBoard>>, OpenLinesEvaluation,
            CentreFirstColumns<ConnectFourBoard>, BitboardPosition, int>(
            _game, _evaluation, new(_game), _position, Depth);

    [Benchmark]
    public int DeepeningWithTable() =>
        AlphaBeta.BestMoveByDeepening<
            BitboardGame<Gravity<ConnectFourBoard>>, OpenLinesEvaluation,
            CentreFirstColumns<ConnectFourBoard>, BitboardPosition, int>(
            _game, _evaluation, new(_game), _position, Depth).Value;
}
