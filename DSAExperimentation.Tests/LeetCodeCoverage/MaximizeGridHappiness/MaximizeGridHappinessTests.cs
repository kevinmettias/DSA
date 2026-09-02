using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximizeGridHappiness;

// LeetCode 1659. Maximize Grid Happiness: profile DP over "which type occupies each of
// the last n cells" (n = column count), cell by cell in row-major order, via this
// repo's own Memoizer - the same (Row, PrevMask)-shaped state
// MaximumStudentsTakingExamTests already establishes, widened to a 4-tuple
// (Pos, Mask, Introverts, Extroverts) since two independently-exhaustible people
// pools replace that problem's single per-row choice. Mask packs the last n
// placements in base 3 (0 = empty, 1 = introvert, 2 = extrovert); the up neighbor is
// mask's oldest digit (mask / 3^(n-1)), the left neighbor is its newest (mask % 3),
// and placing a new person shifts a fresh digit in while dropping the oldest.
public sealed class MaximizeGridHappinessTests
{
    [Fact]
    public void GetMaxGridHappiness_LeetCodeExampleOne_ReturnsTwoHundredForty()
    {
        var actual = GetMaxGridHappiness(m: 2, n: 3, introvertsCount: 1, extrovertsCount: 2);

        Assert.Equal(240, actual);
    }

    [Fact]
    public void GetMaxGridHappiness_LeetCodeExampleTwo_ReturnsTwoHundredSixty()
    {
        var actual = GetMaxGridHappiness(m: 3, n: 1, introvertsCount: 2, extrovertsCount: 1);

        Assert.Equal(260, actual);
    }

    [Fact]
    public void GetMaxGridHappiness_LeetCodeExampleThree_ReturnsTwoHundredForty()
    {
        var actual = GetMaxGridHappiness(m: 2, n: 2, introvertsCount: 4, extrovertsCount: 0);

        Assert.Equal(240, actual);
    }

    // Bundles the grid-shape values BestFrom/Neighbors/ShiftIn need so passing them
    // around stays a single parameter instead of three loose, always-together ints.
    private readonly record struct GridLayout(int ColumnCount, int TotalCells, int OldestDigitScale);

    // Bundles one candidate placement (which type, its base happiness, its already-
    // shifted mask) with the neighbors/state it needs, so WithPersonPlaced stays a
    // two-parameter helper instead of the five loose values it would otherwise take.
    private readonly record struct PlacementAttempt(
        int PersonType,
        int BaseHappiness,
        int Up,
        int Left,
        (int Pos, int Mask, int Introverts, int Extroverts) State,
        int NewMask);

    private static int GetMaxGridHappiness(int m, int n, int introvertsCount, int extrovertsCount)
    {
        var layout = new GridLayout(n, m * n, PowerOfThree(n - 1));

        return Memoizer.Memoize<(int Pos, int Mask, int Introverts, int Extroverts), int>(
            (0, 0, introvertsCount, extrovertsCount), (state, recurse) => BestFrom(state, layout, recurse));
    }

    private static int PowerOfThree(int exponent)
    {
        var value = 1;
        for (var i = 0; i < exponent; i++)
        {
            value *= 3;
        }

        return value;
    }

    private static int BestFrom(
        (int Pos, int Mask, int Introverts, int Extroverts) state,
        GridLayout layout,
        Func<(int Pos, int Mask, int Introverts, int Extroverts), int> bestFrom)
    {
        var (pos, mask, introverts, extroverts) = state;
        if (pos == layout.TotalCells || (introverts == 0 && extroverts == 0))
        {
            return 0;
        }

        var neighbors = Neighbors(pos, mask, layout);
        var best = bestFrom((pos + 1, ShiftIn(mask, 0, layout.OldestDigitScale), introverts, extroverts));

        var introvertAttempt = MakeAttempt((1, 120), neighbors, state, layout);
        best = BestConsideringPlacement(best, introvertAttempt, bestFrom);

        var extrovertAttempt = MakeAttempt((2, 40), neighbors, state, layout);
        best = BestConsideringPlacement(best, extrovertAttempt, bestFrom);

        return best;
    }

    private static (int Up, int Left) Neighbors(int pos, int mask, GridLayout layout)
    {
        var row = pos / layout.ColumnCount;
        var col = pos % layout.ColumnCount;
        return (row > 0 ? mask / layout.OldestDigitScale : 0, col > 0 ? mask % 3 : 0);
    }

    private static int ShiftIn(int mask, int newType, int oldestDigitScale) => (mask % oldestDigitScale) * 3 + newType;

    private static PlacementAttempt MakeAttempt(
        (int Type, int BaseHappiness) profile,
        (int Up, int Left) neighbors,
        (int Pos, int Mask, int Introverts, int Extroverts) state,
        GridLayout layout)
        => new(
            profile.Type, profile.BaseHappiness, neighbors.Up, neighbors.Left,
            state, ShiftIn(state.Mask, profile.Type, layout.OldestDigitScale));

    // Skips the recursive lookup entirely once the relevant pool is exhausted -
    // placing that type isn't a legal candidate move, so there's nothing to try.
    private static int BestConsideringPlacement(
        int best, PlacementAttempt attempt, Func<(int Pos, int Mask, int Introverts, int Extroverts), int> bestFrom)
    {
        var (_, _, introverts, extroverts) = attempt.State;
        var poolCount = attempt.PersonType == 1 ? introverts : extroverts;

        if (poolCount == 0)
        {
            return best;
        }

        var withPerson = WithPersonPlaced(attempt, bestFrom);
        return Math.Max(best, withPerson);
    }

    private static int WithPersonPlaced(
        PlacementAttempt attempt, Func<(int Pos, int Mask, int Introverts, int Extroverts), int> bestFrom)
    {
        var gain = attempt.BaseHappiness
            + NeighborDelta(attempt.PersonType, attempt.Up)
            + NeighborDelta(attempt.PersonType, attempt.Left);

        var (pos, _, introverts, extroverts) = attempt.State;
        var nextState = attempt.PersonType == 1
            ? (pos + 1, attempt.NewMask, introverts - 1, extroverts)
            : (pos + 1, attempt.NewMask, introverts, extroverts - 1);

        return gain + bestFrom(nextState);
    }

    private static int NeighborDelta(int personType, int neighbor)
    {
        if (neighbor == 0)
        {
            return 0;
        }

        var selfDelta = personType == 1 ? -30 : 20;
        var neighborDelta = neighbor == 1 ? -30 : 20;
        return selfDelta + neighborDelta;
    }
}
