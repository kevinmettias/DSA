using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NthMagicalNumber;

// LeetCode 878. Nth Magical Number: the nth positive integer divisible by either of
// two factors, reported modulo 1e9+7.
//
// count(x) = x/a + x/b - x/lcm(a, b) (inclusion-exclusion over the multiples of
// each factor, subtracting the shared multiples counted twice) is non-decreasing in
// x, so the answer is the first x whose count reaches the requested rank. The
// baseline finds that x by walking every candidate one at a time; the composed
// strategy states the same monotone predicate as MagicalCountReachesRank and hands
// it to MonotonePredicateSearch.FirstTrue - the "binary search on the answer" shape
// FirstBadVersion uses, just with a two-term counting predicate per candidate
// instead of a single comparison.
internal static class NthMagicalNumberSolution
{
    // Deliberately written without this repo's primitives - a plain counting walk
    // from 1 upward, O(answer), and the arm the binary search below has to justify
    // itself against.
    public static int NthMagicalNumberByCountScan(int rank, int firstFactor, int secondFactor)
    {
        var count = 0;
        var x = 0;

        while (count < rank)
        {
            x++;

            if (x % firstFactor == 0 || x % secondFactor == 0)
            {
                count++;
            }
        }

        return (int)(x % ModularArithmetic.Modulo);
    }

    // The nth magical number is at most rank * min(firstFactor, secondFactor) - rank
    // multiples of the smaller factor alone already reach it - so the search window is
    // bounded before the first probe, and FirstTrue spends O(log(answer)) counting
    // steps inside it. That bound reaches 4 * 10^13 at LeetCode's limits, so the window
    // is searched in long and only the reduced answer comes back as an int.
    public static int NthMagicalNumberByBinarySearch(int rank, int firstFactor, int secondFactor)
    {
        var lcm = LeastCommonMultiple.Of((long)firstFactor, secondFactor);
        var upperBound = (long)rank * Math.Min(firstFactor, secondFactor);
        var rule = new MagicalCountReachesRank(rank, firstFactor, secondFactor, lcm);

        var x = MonotonePredicateSearch.FirstTrue(1L, upperBound, rule);

        return (int)(x % ModularArithmetic.Modulo);
    }
}
