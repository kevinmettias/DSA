using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CatAndMouseII;

// LeetCode 1728. Cat and Mouse II: grid minimax game-theory recursion over
// (mouseRow, mouseCol, catRow, catCol, turn) via this repo's own Memoizer - the
// same turn-based optimal-play shape CanIWinTests/PredictTheWinnerTests already
// use, just with a 2D position tuple instead of a bitmask/interval. On an even
// turn Mouse moves and wins the state if ANY reachable move leads to a mouse-win
// state; on an odd turn Cat moves and Mouse only wins the state if EVERY reachable
// Cat move still leads to a mouse-win state. Turn count is capped at rows*cols*2 -
// the standard bound for this problem (if neither side has forced a decision
// within twice the number of cells, some (mouseRow,mouseCol,catRow,catCol) board
// state must repeat by pigeonhole with no net progress, so Mouse can never force a
// win from there) - standing in for the official "1000 turns" rule while keeping
// the memoized state space small enough to actually enumerate.
public sealed class CatAndMouseIITests
{
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Fact]
    public void CanMouseWin_MouseAdjacentToFoodAndMovesFirst_ReturnsTrue()
    {
        var canWin = CanMouseWin(["C..", "...", ".MF"], catJump: 1, mouseJump: 1);
        Assert.True(canWin);
    }

    [Fact]
    public void CanMouseWin_MouseJumpsOverCatStraightToFood_ReturnsTrue()
    {
        var canWin = CanMouseWin(["F.C.M"], catJump: 1, mouseJump: 4);
        Assert.True(canWin);
    }

    [Fact]
    public void CanMouseWin_MouseCorneredInCorridorByCat_ReturnsFalse()
    {
        var canWin = CanMouseWin(["F.C.M"], catJump: 1, mouseJump: 1);
        Assert.False(canWin);
    }

    private static bool CanMouseWin(string[] grid, int catJump, int mouseJump)
    {
        var (bounds, mouseStart, catStart, food) = ParseGrid(grid);
        var turnCap = bounds.Rows * bounds.Cols * 2;
        var context = new GameContext(food, turnCap, bounds, mouseJump, catJump);

        return Memoizer.Memoize<(int MouseRow, int MouseCol, int CatRow, int CatCol, int Turn), bool>(
            (mouseStart.Row, mouseStart.Col, catStart.Row, catStart.Col, 0),
            (state, mouseWins) => Recurrence(state, mouseWins, context));
    }

    // Everything one CanMouseWin call's recurrence needs that stays fixed across
    // every memoized state - bundled so Recurrence stays within the max
    // parameter count.
    private readonly record struct GameContext((int Row, int Col) Food, int TurnCap, GridBounds Bounds, int MouseJump, int CatJump);

    private static bool Recurrence(
        (int MouseRow, int MouseCol, int CatRow, int CatCol, int Turn) state,
        Func<(int MouseRow, int MouseCol, int CatRow, int CatCol, int Turn), bool> mouseWins,
        GameContext context)
    {
        var (mouseRow, mouseCol, catRow, catCol, turn) = state;

        if (mouseRow == catRow && mouseCol == catCol)
        {
            return false;
        }

        if (mouseRow == context.Food.Row && mouseCol == context.Food.Col)
        {
            return true;
        }

        if (catRow == context.Food.Row && catCol == context.Food.Col)
        {
            return false;
        }

        if (turn >= context.TurnCap)
        {
            return false;
        }

        if (turn % 2 == 0)
        {
            return ReachableCells((mouseRow, mouseCol), new StepLimit(context.MouseJump, Blocker: null), context.Bounds)
                .Any(next => mouseWins((next.Row, next.Col, catRow, catCol, turn + 1)));
        }

        return ReachableCells((catRow, catCol), new StepLimit(context.CatJump, (mouseRow, mouseCol)), context.Bounds)
            .All(next => mouseWins((mouseRow, mouseCol, next.Row, next.Col, turn + 1)));
    }

    // Reads the grid once into the fixed shape CanMouseWin's recurrence needs:
    // the wall/dimension bounds plus the three cells' starting positions.
    private static (GridBounds Bounds, (int Row, int Col) MouseStart, (int Row, int Col) CatStart, (int Row, int Col) Food) ParseGrid(string[] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var wall = new bool[rows, cols];
        (int Row, int Col) mouseStart = default;
        (int Row, int Col) catStart = default;
        (int Row, int Col) food = default;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                switch (grid[r][c])
                {
                    case '#': wall[r, c] = true; break;
                    case 'M': mouseStart = (r, c); break;
                    case 'C': catStart = (r, c); break;
                    case 'F': food = (r, c); break;
                }
            }
        }

        return (new GridBounds(rows, cols, wall), mouseStart, catStart, food);
    }

    // Fixed grid shape a whole CanMouseWin call shares across every reachability
    // probe, bundled so ReachableCells/StepsInDirection stay within the max
    // parameter count.
    private readonly record struct GridBounds(int Rows, int Cols, bool[,] Wall);

    // How far a single reachability probe may travel, and the one cell (if any)
    // where a landing lands but a path traveling further stops.
    private readonly record struct StepLimit(int MaxSteps, (int Row, int Col)? Blocker);

    // Every cell reachable by jumping 0..limit.MaxSteps steps in one of the 4
    // cardinal directions, stopping at walls/bounds. Mouse passes blocker:null
    // (no extra stop); Cat's blocker is Mouse's cell - Cat may land there (that
    // is the capture) but cannot jump past it, so its path stops there too.
    private static IEnumerable<(int Row, int Col)> ReachableCells((int Row, int Col) origin, StepLimit limit, GridBounds bounds)
    {
        yield return origin;

        foreach (var direction in Directions)
        {
            foreach (var cell in StepsInDirection(origin, direction, limit, bounds))
            {
                yield return cell;
            }
        }
    }

    // One direction's worth of ReachableCells.
    private static IEnumerable<(int Row, int Col)> StepsInDirection(
        (int Row, int Col) origin, (int Row, int Col) direction, StepLimit limit, GridBounds bounds)
    {
        for (var step = 1; step <= limit.MaxSteps; step++)
        {
            var r = origin.Row + (direction.Row * step);
            var c = origin.Col + (direction.Col * step);

            if (r < 0 || r >= bounds.Rows || c < 0 || c >= bounds.Cols || bounds.Wall[r, c])
            {
                break;
            }

            yield return (r, c);

            if (limit.Blocker is { } b && b.Row == r && b.Col == c)
            {
                break;
            }
        }
    }
}
