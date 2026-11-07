using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.Domain.Mathematics.Groups;

namespace DSAExperimentation.LeetCode.Solutions.CouplesHoldingHands;

// LeetCode 765. Couples Holding Hands: the fewest adjacent-pair swaps that seat
// every couple (2k, 2k+1) together.
//
// The textbook baseline walks the row two seats at a time and, whenever a seat's
// occupant isn't already beside their partner, looks the partner up by position and
// swaps them into place. This repo's own strategy instead unions each seat pair's
// two occupants by couple id into a DisjointSet - the same NumberOfProvincesSolutionTests/
// RedundantConnectionSolution primitive, applied here to a permutation's cycle
// structure instead of an adjacency matrix. A permutation decomposed into cycles
// needs exactly (cycleLength - 1) swaps per cycle to fix, and summing that over
// every component simplifies to coupleCount minus the distinct-root count, counted
// with this repo's own Set<int>. A third strategy reads the same cycles off one
// permutation of the seats, composed and counted by Domain's Permutation.
[LeetCodeQuestion(
    "couples-holding-hands",
    LeetCodeAnswerOrder.AsWritten,
    Promise = typeof(CouplesHoldingHandsPromise))]
internal static class CouplesHoldingHandsSolution
{
    // Each cycle of couples is walked twice by the seat permutation below, once each way round.
    private const int WaysRoundACycle = 2;

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

        for (var seat = 0; seat < n; seat += CouplesHoldingHandsConstants.SeatsPerCouple)
        {
            if (TrySwapPartnerIntoPlace(seats, position, seat))
            {
                swaps++;
            }
        }

        return swaps;
    }

    // Attempts the one swap this seat needs and reports whether it happened: a seat
    // whose partner already sits beside it changes nothing and answers false.
    private static bool TrySwapPartnerIntoPlace(int[] row, int[] position, int seat)
    {
        var first = row[seat];
        var isFirstOfCouple = first % CouplesHoldingHandsConstants.SeatsPerCouple == 0;
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
        var coupleCount = row.Length / CouplesHoldingHandsConstants.SeatsPerCouple;
        var couples = new DisjointSet(coupleCount);

        for (var seat = 0; seat < row.Length; seat += CouplesHoldingHandsConstants.SeatsPerCouple)
        {
            couples.Union(row[seat] / CouplesHoldingHandsConstants.SeatsPerCouple, row[seat + 1] / CouplesHoldingHandsConstants.SeatsPerCouple);
        }

        var roots = new Set<int>();

        for (var couple = 0; couple < coupleCount; couple++)
        {
            roots.TryAdd(couples.Find(couple));
        }

        return coupleCount - roots.Count;
    }

    // The same cycles in the group of permutations of the seats. X swaps seats 2k and 2k + 1, which
    // pairs a couple's two people as well as a bench's two seats, and the seating R sends each seat
    // to the person in it. From a seat, R^-1 X R is the seat of its occupant's partner, and X then
    // crosses to that bench's other seat, so f = X R^-1 X R, the commutator of the partner swap and
    // the seating, walks from bench to bench along a cycle of couples. A cycle of k couples is two
    // of f's cycles of k seats, one each way round, and a couple already together is two fixed
    // seats, so couples minus cycles of couples is couples minus half of f's cycles.
    public static int MinSwapsByPermutationCycles(int[] row)
    {
        var seating = Permutation.FromImages(row);
        var partnerSwap = PartnerSwap(row.Length);
        var nextSeat = partnerSwap.After(seating.Inverse()).After(partnerSwap).After(seating);
        var coupleCount = row.Length / CouplesHoldingHandsConstants.SeatsPerCouple;
        var coupleCycles = PermutationCycles.Count(nextSeat) / WaysRoundACycle;

        return coupleCount - coupleCycles;
    }

    // X: seats 2k and 2k + 1 swapped, which also sends each person to their partner.
    private static Permutation PartnerSwap(int seatCount)
    {
        var partners = new int[seatCount];

        for (var seat = 0; seat < seatCount; seat += CouplesHoldingHandsConstants.SeatsPerCouple)
        {
            partners[seat] = seat + 1;
            partners[seat + 1] = seat;
        }

        return Permutation.FromImages(partners);
    }
}
