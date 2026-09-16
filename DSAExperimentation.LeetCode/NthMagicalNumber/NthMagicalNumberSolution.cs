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
// strategy hands the same monotone predicate to BinarySearch.LowerBound over
// MagicalCountSequence - the "monotone virtual sequence" shape SqrtX and
// FirstBadVersion already use, just with a two-term counting predicate per index
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
    // bounded before the first probe, and LowerBound spends O(log(answer)) counting
    // steps inside it.
    public static int NthMagicalNumberByBinarySearch(int rank, int firstFactor, int secondFactor)
    {
        var lcm = (long)firstFactor / Gcd(firstFactor, secondFactor) * secondFactor;
        var upperBound = checked((int)((long)rank * Math.Min(firstFactor, secondFactor)));
        var sequence = new MagicalCountSequence(rank, firstFactor, secondFactor, lcm, upperBound);

        var x = BinarySearch.LowerBound<int, MagicalCountSequence>(sequence, 1);

        return (int)(x % ModularArithmetic.Modulo);
    }

    private static int Gcd(int first, int second) => second == 0 ? first : Gcd(second, first % second);
}
