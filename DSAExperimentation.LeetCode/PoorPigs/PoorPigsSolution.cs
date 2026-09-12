using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.PoorPigs;

// LeetCode 458. Poor Pigs: fewest pigs needed to find the one poisoned bucket out of
// `buckets`, given minutesToTest to test in and minutesToDie for a pig to show
// symptoms. With rounds = minutesToTest / minutesToDie feeding rounds available per
// pig, (rounds + 1)^pigs distinct outcomes are distinguishable with `pigs` pigs - a
// function monotonically increasing in pigs, so the minimum pig count is the first
// pigs where that count meets or exceeds `buckets`.
//
// The two strategies differ only in how they find that first pigs: recomputing
// basis^pigs from scratch for every candidate, or this repo's own
// BinarySearch.LowerBound over an ArraySequence<long> of precomputed powers.
internal static class PoorPigsSolution
{
    // The textbook answer: recompute basis^pigs by repeated multiplication for every
    // candidate pig count in turn (O(pigs^2) total). Deliberately written without
    // this repo's primitives - it is the arm the composed solution below has to
    // justify itself against.
    public static int MinPigsByLinearRecompute(int buckets, int minutesToDie, int minutesToTest)
    {
        var basis = ((long)minutesToTest / minutesToDie) + 1;
        var pigs = 0;

        while (Power(basis, pigs) < buckets)
        {
            pigs++;
        }

        return pigs;
    }

    private static long Power(long basis, int exponent)
    {
        var result = 1L;

        for (var i = 0; i < exponent; i++)
        {
            result *= basis;
        }

        return result;
    }

    // This repo's own BinarySearch.LowerBound over an ArraySequence<long> of
    // precomputed powers of the basis: build the sequence once (O(pigs)), then binary
    // search it (O(log pigs)) instead of rescanning from scratch for every candidate.
    public static int MinPigsByBinarySearch(int buckets, int minutesToDie, int minutesToTest)
    {
        var basis = ((long)minutesToTest / minutesToDie) + 1;

        var powers = new List<long> { 1L };

        while (powers[^1] < buckets)
        {
            powers.Add(powers[^1] * basis);
        }

        var sequence = new ArraySequence<long>(powers.ToArray());

        return BinarySearch.LowerBound<long, ArraySequence<long>>(sequence, buckets);
    }
}
