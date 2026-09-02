using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.FindTheLargestPalindromeDivisibleByK;

// LeetCode 3260. Find the Largest Palindrome Divisible by K: the largest n-digit
// palindrome (n up to 1e5, far past long/BigInteger territory) that is divisible by
// k (1-9). Every candidate is built from its own left half - positions
// [0, h), h = ceil(n/2) - the rest mirrors automatically, so both strategies only
// ever choose h digits, not n.
//
// LargestPalindromeByBruteForce tries every half in strictly descending
// lexicographic order (== descending numeric order once the leading digit is fixed
// nonzero, since every half has the same length) and leaf-checks divisibility by
// materializing the mirrored string - the 9*10^(h-1)-leaf walk
// LargestPalindromeByDigitDpMemo has to beat.
//
// LargestPalindromeByDigitDpMemo never materializes a candidate that fails.
// Mirroring makes each half digit d at position i contribute d * weight[i] to the
// full number's residue mod k, where weight[i] = (10^i + 10^(n-1-i)) % k, or just
// 10^i % k for the unmirrored middle digit of an odd-length palindrome. Memoizer
// walks (Position, NeededResidue) - "can positions [Position, h) still bring the
// running residue to NeededResidue?" - exactly once per state, the same tuple-state
// digit-DP composition NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo
// already proves out, and records the largest digit that keeps each visited state
// feasible in a side dictionary so the greedy walk that follows looks the answer up
// in O(1) per position instead of re-deriving it.
internal static class FindTheLargestPalindromeDivisibleByKSolution
{
    public static string LargestPalindromeByBruteForce(int n, int k)
    {
        var half = new char[HalfLength(n)];

        return TryFillDescending(0, half, n, k) ? Mirror(half, n) : string.Empty;
    }

    private static bool TryFillDescending(int position, char[] half, int n, int k)
    {
        if (position == half.Length)
        {
            return IsDivisible(Mirror(half, n), k);
        }

        var lowestDigit = position == 0 ? 1 : 0;

        for (var digit = 9; digit >= lowestDigit; digit--)
        {
            half[position] = (char)('0' + digit);

            if (TryFillDescending(position + 1, half, n, k))
            {
                return true;
            }
        }

        return false;
    }

    public static string LargestPalindromeByDigitDpMemo(int n, int k)
    {
        var h = HalfLength(n);
        var weight = BuildWeights(n, k, h);
        var digitChoice = new Dictionary<(int Position, int Needed), int>();

        var feasible = Memoizer.Memoize<(int Position, int Needed), bool>(
            (0, 0),
            (state, recurse) => IsFeasible(state, h, weight, k, digitChoice, recurse));

        if (!feasible)
        {
            return string.Empty;
        }

        var half = new char[h];
        var needed = 0;

        for (var position = 0; position < h; position++)
        {
            var digit = digitChoice[(position, needed)];
            half[position] = (char)('0' + digit);
            needed = Reduce(needed - digit * weight[position], k);
        }

        return Mirror(half, n);
    }

    // Side channel: Memoizer's own return value only reports whether (0, 0) is
    // feasible, so the digit that MADE each visited state feasible is recorded here
    // as it is discovered - every state this records is one the greedy replay above
    // will actually ask for, since replay starts at the same (0, 0) and re-derives
    // Needed the same way this recursion did.
    private static bool IsFeasible(
        (int Position, int Needed) state,
        int h,
        int[] weight,
        int k,
        Dictionary<(int Position, int Needed), int> digitChoice,
        Func<(int Position, int Needed), bool> recurse)
    {
        var (position, needed) = state;

        if (position == h)
        {
            return needed == 0;
        }

        var lowestDigit = position == 0 ? 1 : 0;

        for (var digit = 9; digit >= lowestDigit; digit--)
        {
            var nextNeeded = Reduce(needed - digit * weight[position], k);

            if (recurse((position + 1, nextNeeded)))
            {
                digitChoice[state] = digit;
                return true;
            }
        }

        return false;
    }

    private static int[] BuildWeights(int n, int k, int h)
    {
        var pow10 = new int[n];
        pow10[0] = 1 % k;

        for (var i = 1; i < n; i++)
        {
            pow10[i] = pow10[i - 1] * 10 % k;
        }

        var weight = new int[h];

        for (var i = 0; i < h; i++)
        {
            var mirrorPosition = n - 1 - i;
            weight[i] = mirrorPosition == i ? pow10[i] : (pow10[i] + pow10[mirrorPosition]) % k;
        }

        return weight;
    }

    private static bool IsDivisible(string number, int k)
    {
        var remainder = 0;

        foreach (var digit in number)
        {
            remainder = (remainder * 10 + (digit - '0')) % k;
        }

        return remainder == 0;
    }

    private static int Reduce(int value, int k) => ((value % k) + k) % k;

    private static int HalfLength(int n) => (n + 1) / 2;

    private static string Mirror(char[] half, int n)
    {
        var result = new char[n];

        for (var i = 0; i < half.Length; i++)
        {
            result[i] = half[i];
            result[n - 1 - i] = half[i];
        }

        return new string(result);
    }
}
