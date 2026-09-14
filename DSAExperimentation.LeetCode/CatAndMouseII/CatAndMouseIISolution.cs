using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.CatAndMouseII;

// LeetCode 1728. Cat and Mouse II: grid minimax game-theory recursion over
// (mouse cell, cat cell, turn) under optimal play. On an even turn Mouse moves and
// wins the state if ANY reachable move leads to a mouse-win state; on an odd turn
// Cat moves and Mouse only wins the state if EVERY reachable Cat move still leads
// to a mouse-win state.
//
// Both strategies evaluate exactly that recurrence and differ only in whether a
// state's result is remembered: the baseline re-explores every repeated board
// position from scratch, the composed arm routes the identical body through this
// repo's own Memoizer - the CanIWin/PredictTheWinner shape, just with a 2D position
// pair instead of a bitmask or an interval.
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

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook answer: the same minimax recursion with no cache at all, so every
    // repeated board position is re-derived. Deliberately written without this
    // repo's Memoizer - it is the arm the composed strategy below has to justify
    // itself against.
    public static bool CanMouseWinByExhaustiveRecursion(string[] grid, int catJump, int mouseJump)
    {
        var (context, start) = ParseGrid(grid, catJump, mouseJump);

        return CanMouseWinWithoutMemo(start, context);
    }

    // This repo's own answer: the identical recurrence handed to Memoizer, keyed on
    // the board state, so each distinct (mouse, cat, turn) triple is evaluated once.
    public static bool CanMouseWinByMemoizedRecursion(string[] grid, int catJump, int mouseJump)
    {
        var (context, start) = ParseGrid(grid, catJump, mouseJump);

        return Memoizer.Memoize<GameState, bool>(
            start, (state, mouseWins) => CanMouseWinWithMemo(state, mouseWins, context));
    }

    private static bool CanMouseWinWithMemo(GameState state, Func<GameState, bool> mouseWins, GameContext context)
    {
        if (Decided(state, context) is { } decided)
        {
            return decided;
        }

        if (state.Turn % PlayerCount == 0)
        {
            return ReachableCells(state.Mouse, new StepLimit(context.MouseJump, Blocker: null), context.Board)
                .Any(next => mouseWins(state with { Mouse = next, Turn = state.Turn + 1 }));
        }

        return ReachableCells(state.Cat, new StepLimit(context.CatJump, state.Mouse), context.Board)
            .All(next => mouseWins(state with { Cat = next, Turn = state.Turn + 1 }));
    }

    private static bool CanMouseWinWithoutMemo(GameState state, GameContext context)
    {
        if (Decided(state, context) is { } decided)
        {
            return decided;
        }

        var mouseToMove = state.Turn % PlayerCount == 0;

        return mouseToMove ? CanMouseWinWithSomeMove(state, context) : CanMouseWinAfterEveryCatMove(state, context);
    }

    private static bool CanMouseWinWithSomeMove(GameState state, GameContext context)
    {
        var limit = new StepLimit(context.MouseJump, Blocker: null);

        foreach (var next in ReachableCells(state.Mouse, limit, context.Board))
        {
            if (CanMouseWinWithoutMemo(state with { Mouse = next, Turn = state.Turn + 1 }, context))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CanMouseWinAfterEveryCatMove(GameState state, GameContext context)
    {
        var limit = new StepLimit(context.CatJump, state.Mouse);

        foreach (var next in ReachableCells(state.Cat, limit, context.Board))
        {
            if (!CanMouseWinWithoutMemo(state with { Cat = next, Turn = state.Turn + 1 }, context))
            {
                return false;
            }
        }

        return true;
    }

    // The four terminal verdicts both strategies share, or null while the game is
    // still open. Capture is checked first, then Mouse reaching food, then Cat
    // reaching food, then the turn budget running out.
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

    // Reads the grid into the fixed shape both recurrences need: the passable board,
    // the food cell, the turn budget, both jump distances, and the two players'
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

        foreach (var direction in Directions)
        {
            foreach (var cell in StepsInDirection(origin, direction, limit, board))
            {
                yield return cell;
            }
        }
    }

    // One direction's worth of ReachableCells.
    private static IEnumerable<(int Row, int Col)> StepsInDirection(
        (int Row, int Col) origin, (int Row, int Col) direction, StepLimit limit, Grid board)
    {
        for (var step = 1; step <= limit.MaxSteps; step++)
        {
            var cell = (Row: origin.Row + (direction.Row * step), Col: origin.Col + (direction.Col * step));

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

    // The part of the position that varies: where each player stands and whose move
    // it is. This is the memo key.
    private readonly record struct GameState((int Row, int Col) Mouse, (int Row, int Col) Cat, int Turn);

    // How far a single reachability probe may travel, and the one cell (if any)
    // where a landing lands but a path traveling further stops.
    private readonly record struct StepLimit(int MaxSteps, (int Row, int Col)? Blocker);
}
