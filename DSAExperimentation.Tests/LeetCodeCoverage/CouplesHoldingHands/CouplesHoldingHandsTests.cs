using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CouplesHoldingHands;

// LeetCode 765. Couples Holding Hands: each seat pair (2k, 2k+1) is unioned by the
// couple id (person / 2) of whoever occupies it, into this repo's own DisjointSet -
// the same NumberOfProvincesTests/RedundantConnectionTests primitive, applied here to
// a permutation's cycle structure instead of an adjacency matrix. A permutation
// decomposed into cycles needs exactly (cycleLength - 1) swaps per cycle to fix, and
// summing that over every DisjointSet component simplifies to coupleCount minus the
// distinct-root count counted with this repo's own Set<int>.
public sealed partial class CouplesHoldingHandsTests
{
    [Fact]
    public void MinSwapsCouples_ClassicExample_ReturnsMinimumSwapCount()
    {
        int[] row = [0, 2, 1, 3];

        var swaps = MinSwapsCouples(row);

        Assert.Equal(1, swaps);
    }

    [Fact]
    public void MinSwapsCouples_AlreadyPaired_ReturnsZero()
    {
        int[] row = [3, 2, 0, 1];

        var swaps = MinSwapsCouples(row);

        Assert.Equal(0, swaps);
    }

    [Fact]
    public void MinSwapsCouples_OneCycleAcrossThreeCouples_ReturnsCycleLengthMinusOne()
    {
        // Couple ids by seat: 2,0,1,2,0,1 - a single 3-couple cycle, needing 2 swaps.
        int[] row = [4, 0, 2, 5, 1, 3];

        var swaps = MinSwapsCouples(row);

        Assert.Equal(2, swaps);
    }

    private static int MinSwapsCouples(int[] row)
    {
        var coupleCount = row.Length / 2;
        var couples = new DisjointSet(coupleCount);

        for (var seat = 0; seat < row.Length; seat += 2)
        {
            couples.Union(row[seat] / 2, row[seat + 1] / 2);
        }

        var roots = new Set<int>();

        for (var couple = 0; couple < coupleCount; couple++)
        {
            roots.TryAdd(couples.Find(couple));
        }

        return coupleCount - roots.Count;
    }
}
