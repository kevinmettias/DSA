using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.CouplesHoldingHands;

// LeetCode 765. Couples Holding Hands: the fewest adjacent-pair swaps that seat
// every couple (2k, 2k+1) together.
//
// The textbook baseline walks the row two seats at a time and, whenever a seat's
// occupant isn't already beside their partner, looks the partner up by position and
// swaps them into place. This repo's own strategy instead unions each seat pair's
// two occupants by couple id into a DisjointSet - the same NumberOfProvincesTests/
// RedundantConnectionTests primitive, applied here to a permutation's cycle
// structure instead of an adjacency matrix. A permutation decomposed into cycles
// needs exactly (cycleLength - 1) swaps per cycle to fix, and summing that over
// every component simplifies to coupleCount minus the distinct-root count, counted
// with this repo's own Set<int>.
internal static class CouplesHoldingHandsSolution
{
    private const int SeatsPerCouple = 2;

    // The greedy swap simulation every LeetCode editorial leads with. Deliberately
    // written without this repo's primitives - it is the arm the composed solution
    // below has to justify itself against.
    public static int MinSwapsByGreedySwap(int[] row)
    {
        var seats = (int[])row.Clone();
        var n = seats.Length;
        var position = new int[n];

        for (var i = 0; i < n; i++)
        {
            position[seats[i]] = i;
        }

        var swaps = 0;

        for (var seat = 0; seat < n; seat += SeatsPerCouple)
        {
            if (SwapPartnerIntoPlace(seats, position, seat))
            {
                swaps++;
            }
        }

        return swaps;
    }

    private static bool SwapPartnerIntoPlace(int[] row, int[] position, int seat)
    {
        var first = row[seat];
        var isFirstOfCouple = first % SeatsPerCouple == 0;
        var partner = isFirstOfCouple ? NextSeatNumber(first) : PreviousSeatNumber(first);

        if (row[seat + 1] == partner)
        {
            return false;
        }

        var partnerSeat = position[partner];
        var displaced = row[seat + 1];

        row[seat + 1] = partner;
        row[partnerSeat] = displaced;
        position[partner] = seat + 1;
        position[displaced] = partnerSeat;

        return true;
    }

    // Couple k occupies the two seat numbers 2k and 2k+1, so a partner is always the
    // neighbouring seat number: the one above the couple's first seat, the one below
    // its second.
    private static int NextSeatNumber(int seat) => seat + 1;

    private static int PreviousSeatNumber(int seat) => seat - 1;

    // Union each seat pair by couple id and count distinct components with this
    // repo's own DisjointSet + Set<int> (NumberOfProvincesBenchmarks' exact shape).
    public static int MinSwapsByDisjointSet(int[] row)
    {
        var coupleCount = row.Length / SeatsPerCouple;
        var couples = new DisjointSet(coupleCount);

        for (var seat = 0; seat < row.Length; seat += SeatsPerCouple)
        {
            couples.Union(row[seat] / SeatsPerCouple, row[seat + 1] / SeatsPerCouple);
        }

        var roots = new Set<int>();

        for (var couple = 0; couple < coupleCount; couple++)
        {
            roots.TryAdd(couples.Find(couple));
        }

        return coupleCount - roots.Count;
    }
}
