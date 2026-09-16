using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.UglyNumberIII;

// LeetCode 1201. Ugly Number III: the rank-th positive integer divisible by the
// first factor, by the second, or by the third.
//
// count(x) = x/a + x/b + x/c - x/lcm(a,b) - x/lcm(a,c) - x/lcm(b,c) + x/lcm(a,b,c)
// (three-set inclusion-exclusion over the multiples of each factor) is
// non-decreasing in x, so the answer is the first candidate whose count reaches
// the requested rank. The baseline finds it by walking every candidate one at a
// time; the composed strategy hands the same monotone predicate to
// BinarySearch.LowerBound over UglyCountSequence - the same shape NthMagicalNumber
// already uses for two factors, extended to three.
internal static class UglyNumberIIISolution
{
    // Deliberately written without this repo's primitives - a plain counting walk
    // from 1 upward, O(answer), and the arm the binary search below has to justify
    // itself against.
    public static int NthUglyNumberByCountScan(
        int rank, int firstFactor, int secondFactor, int thirdFactor)
    {
        var count = 0;
        var x = 0;

        while (count < rank)
        {
            x++;

            if (IsMultipleOfAnyFactor(x, firstFactor, secondFactor, thirdFactor))
            {
                count++;
            }
        }

        return x;
    }

    // A number is ugly when at least one of the three factors divides it - the union the
    // original definition names.
    private static bool IsMultipleOfAnyFactor(
        int value, int firstFactor, int secondFactor, int thirdFactor) =>
        value % firstFactor == 0 || value % secondFactor == 0 || value % thirdFactor == 0;

    // The rank-th ugly number is at most rank times the smallest factor - rank
    // multiples of the smallest factor alone already reach it - so UglyCountSequence
    // derives that bound as its own window, and LowerBound spends O(log(answer))
    // counting steps inside it.
    public static int NthUglyNumberByBinarySearch(
        int rank, int firstFactor, int secondFactor, int thirdFactor)
    {
        var sequence = new UglyCountSequence(rank, firstFactor, secondFactor, thirdFactor);

        return BinarySearch.LowerBound<int, UglyCountSequence>(sequence, 1);
    }
}
