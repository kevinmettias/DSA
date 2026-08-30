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
        => Assert.Equal(240, GetMaxGridHappiness(m: 2, n: 3, introvertsCount: 1, extrovertsCount: 2));

    [Fact]
    public void GetMaxGridHappiness_LeetCodeExampleTwo_ReturnsTwoHundredSixty()
        => Assert.Equal(260, GetMaxGridHappiness(m: 3, n: 1, introvertsCount: 2, extrovertsCount: 1));

    [Fact]
    public void GetMaxGridHappiness_LeetCodeExampleThree_ReturnsTwoHundredForty()
        => Assert.Equal(240, GetMaxGridHappiness(m: 2, n: 2, introvertsCount: 4, extrovertsCount: 0));

    private static int GetMaxGridHappiness(int m, int n, int introvertsCount, int extrovertsCount)
    {
        var oldestDigitScale = 1;
        for (var i = 0; i < n - 1; i++)
        {
            oldestDigitScale *= 3;
        }

        var totalCells = m * n;

        return Memoizer.Memoize<(int Pos, int Mask, int Introverts, int Extroverts), int>(
            (0, 0, introvertsCount, extrovertsCount), BestFrom);

        int BestFrom(
            (int Pos, int Mask, int Introverts, int Extroverts) state,
            Func<(int Pos, int Mask, int Introverts, int Extroverts), int> bestFrom)
        {
            var (pos, mask, introverts, extroverts) = state;
            if (pos == totalCells || (introverts == 0 && extroverts == 0))
            {
                return 0;
            }

            var row = pos / n;
            var col = pos % n;
            var up = row > 0 ? mask / oldestDigitScale : 0;
            var left = col > 0 ? mask % 3 : 0;

            var best = bestFrom((pos + 1, ShiftIn(mask, 0), introverts, extroverts));

            if (introverts > 0)
            {
                var gain = 120 + NeighborDelta(1, up) + NeighborDelta(1, left);
                best = Math.Max(best, gain + bestFrom((pos + 1, ShiftIn(mask, 1), introverts - 1, extroverts)));
            }

            if (extroverts > 0)
            {
                var gain = 40 + NeighborDelta(2, up) + NeighborDelta(2, left);
                best = Math.Max(best, gain + bestFrom((pos + 1, ShiftIn(mask, 2), introverts, extroverts - 1)));
            }

            return best;
        }

        int ShiftIn(int mask, int newType) => (mask % oldestDigitScale) * 3 + newType;
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
