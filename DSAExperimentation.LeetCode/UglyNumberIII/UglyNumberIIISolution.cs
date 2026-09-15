using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.UglyNumberIII;

// LeetCode 1201. Ugly Number III: the nth positive integer divisible by a, by b,
// or by c.
//
// count(x) = x/a + x/b + x/c - x/lcm(a,b) - x/lcm(a,c) - x/lcm(b,c) + x/lcm(a,b,c)
// (three-set inclusion-exclusion over the multiples of each factor) is
// non-decreasing in x, so the answer is the first x whose count reaches n. The
// baseline finds that x by walking every candidate one at a time; the composed
// strategy hands the same monotone predicate to BinarySearch.LowerBound over
// UglyCountSequence - the same shape NthMagicalNumber already uses for two
// factors, extended to three.
internal static class UglyNumberIIISolution
{
    // Deliberately written without this repo's primitives - a plain counting walk
    // from 1 upward, O(answer), and the arm the binary search below has to justify
    // itself against.
    public static int NthUglyNumberByCountScan(int n, int a, int b, int c)
    {
        var count = 0;
        var x = 0;

        while (count < n)
        {
            x++;

            if (IsMultipleOfAnyFactor(x, a, b, c))
            {
                count++;
            }
        }

        return x;
    }

    // A number is ugly when at least one of the three factors divides it - the union the
    // original definition names.
    private static bool IsMultipleOfAnyFactor(int value, int a, int b, int c) =>
        value % a == 0 || value % b == 0 || value % c == 0;

    // The nth ugly number is at most n * min(a, b, c) - n multiples of the smallest
    // factor alone already reach it - so UglyCountSequence derives that bound as its
    // own window, and LowerBound spends O(log(answer)) counting steps inside it.
    public static int NthUglyNumberByBinarySearch(int n, int a, int b, int c)
    {
        var sequence = new UglyCountSequence(n, a, b, c);

        return BinarySearch.LowerBound<int, UglyCountSequence>(sequence, 1);
    }
}
