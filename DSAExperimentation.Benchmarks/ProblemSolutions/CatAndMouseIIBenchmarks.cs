using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Cat and Mouse II (LC 1728): the textbook unmemoized minimax recursion over
// (mouseCol, catCol, turn) on a 1-wide corridor - re-exploring every repeated
// board position from scratch - vs. the same recursion routed through this repo's
// own Memoizer, keyed on that same state tuple (the CanIWinBenchmarks shape: one
// hand-written brute-force recursion vs. Memoizer closing over an identical
// recurrence body). Grid is a single row (Food at one end, Cat in the middle,
// Mouse at the far end) so both players' positions - and therefore the distinct
// state count - stay small while CorridorLength still grows the K^turnCap-shaped
// unmemoized search tree.
[MemoryDiagnoser]
public class CatAndMouseIIBenchmarks
{
    private const int CatJump = 1;
    private const int MouseJump = 1;

    // Mouse and Cat alternate turns, so parity of the turn counter selects whose move it is.
    private const int PlayerCount = 2;

    // Cat starts halfway along the corridor.
    private const int CorridorMidpointDivisor = 2;

    // Turn budget scales with corridor length so both players have room to maneuver.
    private const int TurnCapMultiplier = 2;

    [Params(4, 6)]
    public int CorridorLength;

    private int _catStart;
    private int _mouseStart;
    private int _turnCap;

    [GlobalSetup]
    public void Setup()
    {
        _catStart = CorridorLength / CorridorMidpointDivisor;
        _mouseStart = CorridorLength - 1;
        _turnCap = CorridorLength * TurnCapMultiplier;
    }

    [Benchmark(Baseline = true)]
    public bool UnmemoizedRecursion() => CanMouseWinBruteForce(_mouseStart, _catStart, 0);

    private bool CanMouseWinBruteForce(int mouseCol, int catCol, int turn)
    {
        if (mouseCol == catCol)
        {
            return false;
        }

        if (mouseCol == 0)
        {
            return true;
        }

        if (catCol == 0)
        {
            return false;
        }

        if (turn >= _turnCap)
        {
            return false;
        }

        return turn % PlayerCount == 0
            ? MouseTurnWins(mouseCol, catCol, turn)
            : CatTurnWins(mouseCol, catCol, turn);
    }

    private bool MouseTurnWins(int mouseCol, int catCol, int turn)
    {
        foreach (var next in ReachableCols(mouseCol, MouseJump, blocker: -1))
        {
            if (CanMouseWinBruteForce(next, catCol, turn + 1))
            {
                return true;
            }
        }

        return false;
    }

    private bool CatTurnWins(int mouseCol, int catCol, int turn)
    {
        foreach (var next in ReachableCols(catCol, CatJump, blocker: mouseCol))
        {
            if (!CanMouseWinBruteForce(mouseCol, next, turn + 1))
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool MemoizedRecursion()
        => Memoizer.Memoize<(int MouseCol, int CatCol, int Turn), bool>((_mouseStart, _catStart, 0), Recurrence);

    private bool Recurrence(
        (int MouseCol, int CatCol, int Turn) state, Func<(int MouseCol, int CatCol, int Turn), bool> mouseWins)
    {
        var (mouseCol, catCol, turn) = state;

        if (mouseCol == catCol)
        {
            return false;
        }

        if (mouseCol == 0)
        {
            return true;
        }

        if (catCol == 0)
        {
            return false;
        }

        if (turn >= _turnCap)
        {
            return false;
        }

        if (turn % PlayerCount == 0)
        {
            return ReachableCols(mouseCol, MouseJump, blocker: -1).Any(next => mouseWins((next, catCol, turn + 1)));
        }

        return ReachableCols(catCol, CatJump, blocker: mouseCol).All(next => mouseWins((mouseCol, next, turn + 1)));
    }

    // Columns reachable by moving 0..jump steps left/right along the corridor,
    // stopping at the boundary. Cat additionally stops at (but may land on)
    // blocker - Mouse's current column - it may not jump past it.
    private IEnumerable<int> ReachableCols(int col, int jump, int blocker)
    {
        yield return col;

        for (var step = 1; step <= jump && col - step >= 0; step++)
        {
            yield return col - step;
            if (col - step == blocker)
            {
                break;
            }
        }

        for (var step = 1; step <= jump && col + step < CorridorLength; step++)
        {
            yield return col + step;
            if (col + step == blocker)
            {
                break;
            }
        }
    }
}
