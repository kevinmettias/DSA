using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NthMagicalNumber;

// LeetCode 878. Nth Magical Number: the nth positive integer divisible by a or by
// b, reported modulo 1e9+7.
//
// count(x) = x/a + x/b - x/lcm(a, b) (inclusion-exclusion over the multiples of
// each factor, subtracting the shared multiples counted twice) is non-decreasing in
// x, so the answer is the first x whose count reaches n. The baseline finds that x
// by walking every candidate one at a time; the composed strategy hands the same
// monotone predicate to BinarySearch.LowerBound over MagicalCountSequence - the
// "monotone virtual sequence" shape SqrtX and FirstBadVersion already use, just
// with a two-term counting predicate per index instead of a single comparison.
internal static class NthMagicalNumberSolution
{
    // Deliberately written without this repo's primitives - a plain counting walk
    // from 1 upward, O(answer), and the arm the binary search below has to justify
    // itself against.
    public static int NthMagicalNumberByCountScan(int n, int a, int b)
    {
        var count = 0;
        var x = 0;

        while (count < n)
        {
            x++;

            if (x % a == 0 || x % b == 0)
            {
                count++;
            }
        }

        return (int)(x % ModularArithmetic.Modulo);
    }

    // The nth magical number is at most n * min(a, b) - n multiples of the smaller
    // factor alone already reach it - so the search window is bounded before the
    // first probe, and LowerBound spends O(log(answer)) counting steps inside it.
    public static int NthMagicalNumberByBinarySearch(int n, int a, int b)
    {
        var lcm = (long)a / Gcd(a, b) * b;
        var upperBound = checked((int)((long)n * Math.Min(a, b)));
        var sequence = new MagicalCountSequence(n, a, b, lcm, upperBound);

        var x = BinarySearch.LowerBound<int, MagicalCountSequence>(sequence, 1);

        return (int)(x % ModularArithmetic.Modulo);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
