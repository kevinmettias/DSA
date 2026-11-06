using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch;

// Values every position of a finite game graph by retrograde analysis: backward induction from the
// positions its rules decide. A position with a move to a position its opponent loses is a win. A
// position whose every move leads to a position its opponent wins is a loss, found by counting
// down each position's moves not yet known to lead to such a win. Whatever is never decided is a
// draw: neither player can force an end from it, so best play goes round a cycle for ever. Negamax
// and SpragueGrundy recurse forward from one position and overflow the stack on a cycle; this
// visits every position once and works back along every move once, so a cycle costs it nothing.
//
// Working back needs each position's predecessors, which the game does not give, so they are laid
// out first from its moves in two flat arrays: every position's run of predecessors end to end,
// and where each run starts. Positions are then worked back from in the order they were decided,
// which is breadth-first, so each is decided one move further from the end than the position that
// decides it: a win one move beyond its quickest losing successor, a loss one beyond its slowest
// winning one, the last to be decided. That is the length of best play SolvedGame reports.
/// <summary>
/// Values every position of a finite game graph as a win, a loss or a draw by retrograde analysis.
/// </summary>
public static class RetrogradeAnalysis
{
    /// <summary>
    /// Values every position of a game as a win, a loss or a draw for the player to move.
    /// </summary>
    /// <param name="game">The game's rules, carrying the problem's input.</param>
    /// <returns>
    /// Every position's outcome, and how many moves best play lasts from it, by the game's
    /// numbering.
    /// </returns>
    public static SolvedGame Solve<TGame, TPosition>(TGame game)
        where TGame : struct, IGameGraph<TPosition>
    {
        var induction = new BackwardInduction<TGame, TPosition>(game);

        return induction.Run();
    }

    // One analysis: the predecessors laid out from the game's moves, then the positions decided by
    // the rules, and each decided position worked back from in the order it was decided.
    private sealed class BackwardInduction<TGame, TPosition>(TGame game)
        where TGame : struct, IGameGraph<TPosition>
    {
        // For each position, how many of its moves are not yet known to lead to a position the
        // opponent wins; at first, all of them.
        private readonly int[] _undecidedMoves = new int[game.PositionCount];

        // Where each position's run of predecessors starts in the list of them, the run ending
        // where the next position's starts; one more entry than there are positions closes the
        // last run.
        private readonly int[] _firstPredecessor = new int[game.PositionCount + 1];

        private readonly GameOutcome[] _outcomes = new GameOutcome[game.PositionCount];
        private readonly int[] _movesToEnd = new int[game.PositionCount];

        // The decided positions in the order they were decided, which is the order they are worked
        // back from; each position is decided at most once, so one slot per position is enough.
        private readonly int[] _decided = new int[game.PositionCount];

        private int _decidedCount;

        public SolvedGame Run()
        {
            var predecessors = ListPredecessors();
            DecideByRules();
            WorkBack(predecessors);

            return new SolvedGame(_outcomes, _movesToEnd);
        }

        // Every move recorded at the position it leads to: counted there first, so each run's place
        // is known, then written into its run from the run's end back to its start.
        private int[] ListPredecessors()
        {
            CountMoves();
            var moveTotal = EndRuns();
            var predecessors = new int[moveTotal];

            for (var index = 0; index < _undecidedMoves.Length; index++)
            {
                var position = game.PositionAt(index);

                for (var move = 0; move < _undecidedMoves[index]; move++)
                {
                    var next = game.Next(position, move);
                    var successor = game.IndexOf(next);
                    _firstPredecessor[successor]--;
                    predecessors[_firstPredecessor[successor]] = index;
                }
            }

            return predecessors;
        }

        // Each position's moves counted, both at the position they leave and at each position they
        // lead to, which is the length of that position's run of predecessors.
        private void CountMoves()
        {
            for (var index = 0; index < _undecidedMoves.Length; index++)
            {
                var position = game.PositionAt(index);
                var moveCount = game.MoveCount(position);
                _undecidedMoves[index] = moveCount;

                for (var move = 0; move < moveCount; move++)
                {
                    var next = game.Next(position, move);
                    var successor = game.IndexOf(next);
                    _firstPredecessor[successor]++;
                }
            }
        }

        // Each run's end found by summing the runs' lengths up to it; the last entry is then the
        // end of the last run, the count of every move.
        private int EndRuns()
        {
            var runEnd = 0;

            for (var index = 0; index < _firstPredecessor.Length; index++)
            {
                runEnd += _firstPredecessor[index];
                _firstPredecessor[index] = runEnd;
            }

            return runEnd;
        }

        // The positions with no move left take the outcome their rules give. A drawn one settles
        // no predecessor, since a move to it is a way to draw, so it is left as every position
        // starts, and only wins and losses are worked back from.
        private void DecideByRules()
        {
            for (var index = 0; index < _undecidedMoves.Length; index++)
            {
                if (_undecidedMoves[index] > 0)
                {
                    continue;
                }

                var position = game.PositionAt(index);
                var outcome = game.OutcomeWhenOver(position);

                if (outcome != GameOutcome.Draw)
                {
                    Decide(index, outcome, movesToEnd: 0);
                }
            }
        }

        // Each decided position, in the order decided, settles what it can of its predecessors. A
        // position decided on the way joins the end of the order, so the walk goes on until every
        // decided position has been worked back from.
        private void WorkBack(int[] predecessors)
        {
            for (var cursor = 0; cursor < _decidedCount; cursor++)
            {
                SettlePredecessorsOf(_decided[cursor], predecessors);
            }
        }

        // The predecessors a decided position settles take the outcome opposite its own, a loss
        // making them wins and a win making them losses, one move further from the end.
        private void SettlePredecessorsOf(int position, int[] predecessors)
        {
            var outcome = _outcomes[position];
            var isLoss = outcome == GameOutcome.Loss;
            var settledOutcome = isLoss ? GameOutcome.Win : GameOutcome.Loss;
            var settledMovesToEnd = _movesToEnd[position] + 1;
            var runEnd = _firstPredecessor[position + 1];

            for (var slot = _firstPredecessor[position]; slot < runEnd; slot++)
            {
                var predecessor = predecessors[slot];

                if (TrySettle(predecessor, outcome))
                {
                    Decide(predecessor, settledOutcome, settledMovesToEnd);
                }
            }
        }

        // Counts a move from a predecessor still undecided to a position with this outcome, and
        // reports whether that settles the predecessor: a move to a loss always does, and a move to
        // a win does once it is the last of the predecessor's moves left undecided.
        private bool TrySettle(int predecessor, GameOutcome successorOutcome)
        {
            if (_outcomes[predecessor] != GameOutcome.Draw)
            {
                return false;
            }

            if (successorOutcome == GameOutcome.Loss)
            {
                return true;
            }

            _undecidedMoves[predecessor]--;

            return _undecidedMoves[predecessor] == 0;
        }

        // Records a position's outcome and length of best play, and appends it to the order.
        private void Decide(int index, GameOutcome outcome, int movesToEnd)
        {
            _outcomes[index] = outcome;
            _movesToEnd[index] = movesToEnd;
            _decided[_decidedCount++] = index;
        }
    }
}
