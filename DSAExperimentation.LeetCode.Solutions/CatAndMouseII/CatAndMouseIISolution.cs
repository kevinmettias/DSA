using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.LeetCode.Solutions.CatAndMouseII;

// LeetCode 1728. Cat and Mouse II: grid minimax game-theory recursion over
// (mouse cell, cat cell, turn) under optimal play. On an even turn Mouse moves and
// wins the state if ANY reachable move leads to a mouse-win state; on an odd turn
// Cat moves and Mouse only wins the state if EVERY reachable Cat move still leads
// to a mouse-win state.
//
// Both strategies evaluate exactly that recurrence and differ in whether a state's
// result is remembered: the baseline re-explores every repeated board position from
// scratch over a plain bool[,] board, the composed arm routes the identical body
// through this repo's own Memoizer over a Grid - the CanIWin/PredictTheWinner shape,
// just with a 2D position pair instead of a bitmask or an interval.
internal static class CatAndMouseIISolution
{
    private const char WallMark = '#';
    private const char MouseMark = 'M';
    private const char CatMark = 'C';
    private const char FoodMark = 'F';

    // Mouse and Cat alternate turns, so parity of the turn counter selects whose
    // move it is.
    private const int PlayerCount = 2;

    // Turn count is capped at rows*cols*2 - the standard bound for this problem. If
    // neither side has forced a decision within twice the number of cells, some
    // board state must repeat by pigeonhole with no net progress, so Mouse can never
    // force a win from there. It stands in for the official "1000 turns" rule while
    // keeping the memoized state space small enough to actually enumerate.
    private const int TurnCapCellMultiplier = 2;

    // The textbook answer: the same minimax recursion with no cache at all, so every
    // repeated board position is re-derived. Deliberately written without this
    // repo's Memoizer - it is the arm the composed strategy below has to justify
    // itself against - and over a plain passability array rather than this repo's
    // Grid: a baseline uses no library implementation (ARCHITECTURE 17.5).
    public static bool CanMouseWinByExhaustiveRecursion(string[] grid, int catJump, int mouseJump)
    {
        var (context, start) = ParsePlainGrid(grid, catJump, mouseJump);

        return CanMouseWinWithoutMemo(start, context);
    }

    // This repo's own answer: the identical recurrence handed to Memoizer, keyed on
    // the board state, so each distinct (mouse, cat, turn) triple is evaluated once.
    public static bool CanMouseWinByMemoizedRecursion(string[] grid, int catJump, int mouseJump)
    {
        var (context, start) = ParseGrid(grid, catJump, mouseJump);

        return Memoizer.Memoize<GameState, bool>(start, new CanMouseWinUnderPlay(context));
    }

    // The baseline's ParseGrid: the same fixed shape, with the walkable cells kept as
    // a plain array instead of a Grid.
    private static (PlainGameContext Context, GameState Start) ParsePlainGrid(
        string[] grid, int catJump, int mouseJump)
    {
        var passable = BuildPassableCells(grid);

        // A two-dimensional array's Length counts every cell, rows times columns.
        var turnCap = passable.Length * TurnCapCellMultiplier;
        var food = Locate(grid, FoodMark);
        var context = new PlainGameContext(passable, food, turnCap, mouseJump, catJump);
        var mouse = Locate(grid, MouseMark);
        var cat = Locate(grid, CatMark);

        return (context, new GameState(mouse, cat, Turn: 0));
    }

    // Every cell but a wall is walkable.
    private static bool[,] BuildPassableCells(string[] grid)
    {
        var passable = new bool[grid.Length, grid[0].Length];

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                passable[row, col] = grid[row][col] != WallMark;
            }
        }

        return passable;
    }

    private static bool CanMouseWinWithoutMemo(GameState state, PlainGameContext context)
    {
        if (DecidedOnPlainBoard(state, context) is { } decided)
        {
            return decided;
        }

        var mouseToMove = state.Turn % PlayerCount == 0;

        return mouseToMove ? CanMouseWinWithSomeMove(state, context) : CanMouseWinAfterEveryCatMove(state, context);
    }

    // The baseline's copy of Decided, reading the same food cell and turn budget from
    // its own context.
    private static bool? DecidedOnPlainBoard(GameState state, PlainGameContext context)
    {
        if (state.Mouse == state.Cat)
        {
            return false;
        }

        if (state.Mouse == context.Food)
        {
            return true;
        }

        if (state.Cat == context.Food)
        {
            return false;
        }

        return state.Turn >= context.TurnCap ? false : null;
    }

    private static bool CanMouseWinWithSomeMove(GameState state, PlainGameContext context)
    {
        var limit = new StepLimit(context.MouseJump, Blocker: null);

        foreach (var next in PlainReachableCells(state.Mouse, limit, context.Passable))
        {
            if (CanMouseWinWithoutMemo(state with { Mouse = next, Turn = state.Turn + 1 }, context))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CanMouseWinAfterEveryCatMove(GameState state, PlainGameContext context)
    {
        var limit = new StepLimit(context.CatJump, state.Mouse);

        foreach (var next in PlainReachableCells(state.Cat, limit, context.Passable))
        {
            if (!CanMouseWinWithoutMemo(state with { Cat = next, Turn = state.Turn + 1 }, context))
            {
                return false;
            }
        }

        return true;
    }

    // The baseline's ReachableCells, over the plain passability array.
    private static IEnumerable<(int Row, int Col)> PlainReachableCells(
        (int Row, int Col) origin, StepLimit limit, bool[,] passable)
    {
        yield return origin;

        for (var line = 0; line < GridDirections.Orthogonal.Length; line++)
        {
            foreach (var cell in PlainStepsInDirection(origin, GridDirections.Orthogonal[line], limit, passable))
            {
                yield return cell;
            }
        }
    }

    // One direction's worth of PlainReachableCells.
    private static IEnumerable<(int Row, int Col)> PlainStepsInDirection(
        (int Row, int Col) origin, GridStep direction, StepLimit limit, bool[,] passable)
    {
        for (var step = 1; step <= limit.MaxSteps; step++)
        {
            var cell = (Row: origin.Row + (direction.DeltaRow * step), Col: origin.Col + (direction.DeltaCol * step));

            if (!IsPassableCell(passable, cell.Row, cell.Col))
            {
                break;
            }

            yield return cell;

            if (limit.Blocker is { } blocker && blocker == cell)
            {
                break;
            }
        }
    }

    private static bool IsPassableCell(bool[,] passable, int row, int col)
        => IsOnBoard(passable, row, col) && passable[row, col];

    private static bool IsOnBoard(bool[,] passable, int row, int col)
        => row >= 0 && row < passable.GetLength(0) && col >= 0 && col < passable.GetLength(1);

    // The four terminal verdicts, or null while the game is still open. Capture is
    // checked first, then Mouse reaching food, then Cat reaching food, then the turn
    // budget running out. The baseline keeps its own copy, DecidedOnPlainBoard.
    private static bool? Decided(GameState state, GameContext context)
    {
        if (state.Mouse == state.Cat)
        {
            return false;
        }

        if (state.Mouse == context.Food)
        {
            return true;
        }

        if (state.Cat == context.Food)
        {
            return false;
        }

        return state.Turn >= context.TurnCap ? false : null;
    }

    // Reads the grid into the fixed shape the memoized recurrence needs: the passable
    // board, the food cell, the turn budget, both jump distances, and the two players'
    // starting cells.
    private static (GameContext Context, GameState Start) ParseGrid(string[] grid, int catJump, int mouseJump)
    {
        var board = BuildBoard(grid);
        var turnCap = board.Rows * board.Cols * TurnCapCellMultiplier;
        var context = new GameContext(board, Locate(grid, FoodMark), turnCap, mouseJump, catJump);

        return (context, new GameState(Locate(grid, MouseMark), Locate(grid, CatMark), Turn: 0));
    }

    // Every cell but a wall is walkable, which is exactly Grid's passability map.
    private static Grid BuildBoard(string[] grid)
    {
        var passable = new bool[grid.Length, grid[0].Length];

        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                passable[row, col] = grid[row][col] != WallMark;
            }
        }

        return new Grid(passable);
    }

    // The one cell carrying a mark. LeetCode guarantees exactly one 'M', 'C' and 'F'
    // per grid.
    private static (int Row, int Col) Locate(string[] grid, char mark)
    {
        for (var row = 0; row < grid.Length; row++)
        {
            var col = grid[row].IndexOf(mark);

            if (col >= 0)
            {
                return (row, col);
            }
        }

        return default;
    }

    // Every cell reachable by jumping 0..limit.MaxSteps steps in one of the 4
    // cardinal directions, stopping at walls/bounds. Mouse passes Blocker: null (no
    // extra stop); Cat's blocker is Mouse's cell - Cat may land there (that is the
    // capture) but cannot jump past it, so its path stops there too.
    private static IEnumerable<(int Row, int Col)> ReachableCells(
        (int Row, int Col) origin, StepLimit limit, Grid board)
    {
        yield return origin;

        for (var line = 0; line < GridDirections.Orthogonal.Length; line++)
        {
            foreach (var cell in StepsInDirection(origin, GridDirections.Orthogonal[line], limit, board))
            {
                yield return cell;
            }
        }
    }

    // One direction's worth of ReachableCells.
    private static IEnumerable<(int Row, int Col)> StepsInDirection(
        (int Row, int Col) origin, GridStep direction, StepLimit limit, Grid board)
    {
        for (var step = 1; step <= limit.MaxSteps; step++)
        {
            var cell = (Row: origin.Row + (direction.DeltaRow * step), Col: origin.Col + (direction.DeltaCol * step));

            if (!board.IsPassable(cell.Row, cell.Col))
            {
                break;
            }

            yield return cell;

            if (limit.Blocker is { } blocker && blocker == cell)
            {
                break;
            }
        }
    }

    // Everything one call's recurrence needs that stays fixed across every state -
    // bundled so the recurrence bodies stay within the parameter budget.
    private readonly record struct GameContext(
        Grid Board, (int Row, int Col) Food, int TurnCap, int MouseJump, int CatJump);

    // The baseline's GameContext: the same fixed facts, with the walkable cells as a
    // plain array.
    private readonly record struct PlainGameContext(
        bool[,] Passable, (int Row, int Col) Food, int TurnCap, int MouseJump, int CatJump);

    // The part of the position that varies: where each player stands and whose move
    // it is. This is the memo key.
    private readonly record struct GameState((int Row, int Col) Mouse, (int Row, int Col) Cat, int Turn);

    // How far a single reachability probe may travel, and the one cell (if any)
    // where a landing lands but a path traveling further stops.
    private readonly record struct StepLimit(int MaxSteps, (int Row, int Col)? Blocker);

    // The recurrence, named: one position under optimal play, where Mouse moves on an
    // even turn and wins on any winning successor while Cat moves on an odd turn and
    // Mouse needs every successor to win. Everything that stays fixed across the whole
    // game - board, food, turn budget, both jump distances - is the constructor's
    // input, so the rule reads only the position that varies.
    private sealed class CanMouseWinUnderPlay(GameContext context) : IRecurrence<GameState, bool>
    {
        public bool Replay(GameState state, IRecurrence<GameState, bool> rest)
        {
            if (Decided(state, context) is { } decided)
            {
                return decided;
            }

            if (state.Turn % PlayerCount == 0)
            {
                return ReachableCells(state.Mouse, new StepLimit(context.MouseJump, Blocker: null), context.Board)
                    .Any(next => rest.Replay(state with { Mouse = next, Turn = state.Turn + 1 }, rest));
            }

            return ReachableCells(state.Cat, new StepLimit(context.CatJump, state.Mouse), context.Board)
                .All(next => rest.Replay(state with { Cat = next, Turn = state.Turn + 1 }, rest));
        }
    }
}
