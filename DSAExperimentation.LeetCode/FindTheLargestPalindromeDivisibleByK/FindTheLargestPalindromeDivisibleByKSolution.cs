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
    public static string LargestPalindromeByBruteForce(int digitCount, int divisor)
    {
        var half = new char[HalfLength(digitCount)];

        return TryFillDescending(0, half, digitCount, divisor)
            ? Mirror(half, digitCount)
            : string.Empty;
    }

    public static string LargestPalindromeByDigitDpMemo(int digitCount, int divisor)
    {
        var halfLength = HalfLength(digitCount);
        var weight = BuildWeights(digitCount, divisor, halfLength);
        var digitChoice = FeasibleDigitChoices(weight, divisor);

        if (digitChoice is null)
        {
            return string.Empty;
        }

        var half = FillGreedyHalf(halfLength, divisor, weight, digitChoice);

        return Mirror(half, digitCount);
    }

    /// <summary>
    /// The recurrence, named: whether the half-digits still to be chosen at
    /// <c>Position</c> can drive the running residue to <c>Needed</c> - true once the
    /// positions run out and nothing is left needed, and otherwise true as soon as some
    /// digit keeps the rest of the walk feasible. The largest such digit is recorded
    /// against the state, which is what FillGreedyHalf's replay reads back.
    /// </summary>
    private sealed class FeasibleRemainder(
        int[] weights,
        int modulus,
        Dictionary<(int Position, int Needed), int> digitChoice)
        : IRecurrence<(int Position, int Needed), bool>
    {
        /// <inheritdoc/>
        public bool Replay(
            (int Position, int Needed) state,
            IRecurrence<(int Position, int Needed), bool> rest) =>
            IsFeasible(state, (Weights: weights, Modulus: modulus), digitChoice, rest);
    }

    private static int[] BuildWeights(int digitCount, int divisor, int halfLength)
    {
        var pow10 = new int[digitCount];
        pow10[0] = 1 % divisor;

        for (var i = 1; i < digitCount; i++)
        {
            pow10[i] = pow10[i - 1] * 10 % divisor;
        }

        var weight = new int[halfLength];

        for (var i = 0; i < halfLength; i++)
        {
            var mirrorPosition = digitCount - 1 - i;
            weight[i] = WeightForPosition(pow10, i, mirrorPosition, divisor);
        }

        return weight;
    }

    // The residue weight a half digit at this position carries: the position it mirrors
    // contributes its own power of ten as well, unless this is the unmirrored middle of
    // an odd-length palindrome and stands alone.
    private static int WeightForPosition(int[] pow10, int position, int mirrorPosition, int modulus)
    {
        if (mirrorPosition == position)
        {
            return pow10[position];
        }

        return (pow10[position] + pow10[mirrorPosition]) % modulus;
    }

    // The memoized walk, named: it answers whether the start state (0, 0) is feasible
    // at all, and hands back the side dictionary of digits that made each visited
    // state feasible. Null is the "no such palindrome exists" answer - there is then
    // no state for the greedy replay to read, so the empty string is all that is left.
    private static Dictionary<(int Position, int Needed), int>? FeasibleDigitChoices(
        int[] weight, int divisor)
    {
        var digitChoice = new Dictionary<(int Position, int Needed), int>();

        var feasible = Memoizer.Memoize<(int Position, int Needed), bool>(
            (0, 0), new FeasibleRemainder(weight, divisor, digitChoice));

        return feasible ? digitChoice : null;
    }

    // The greedy replay, named: each position's digit is read straight out of the
    // recorded choice, and Needed is re-derived exactly the way the feasibility walk
    // derived it - replay starts at the same (0, 0), so it visits the same states and
    // finds a digit recorded for every one of them.
    private static char[] FillGreedyHalf(
        int halfLength, int divisor, int[] weight,
        Dictionary<(int Position, int Needed), int> digitChoice)
    {
        var half = new char[halfLength];
        var needed = 0;

        for (var position = 0; position < halfLength; position++)
        {
            var digit = digitChoice[(position, needed)];
            half[position] = (char)('0' + digit);
            needed = Reduce(needed - digit * weight[position], divisor);
        }

        return half;
    }

    // Side channel: Memoizer's own return value only reports whether (0, 0) is
    // feasible, so the digit that MADE each visited state feasible is recorded here
    // as it is discovered - every state this records is one the greedy replay above
    // will actually ask for, since replay starts at the same (0, 0) and re-derives
    // Needed the same way this recursion did.
    //
    // The weight table and the modulus it was reduced under are one residue rule:
    // every weight is a residue mod divisor, and neither is ever read without the
    // other. Its length is halfLength itself, so the walk's end reads off the table.
    private static bool IsFeasible(
        (int Position, int Needed) state,
        (int[] Weights, int Modulus) residue,
        Dictionary<(int Position, int Needed), int> digitChoice,
        IRecurrence<(int Position, int Needed), bool> rest)
    {
        var (position, needed) = state;

        if (position == residue.Weights.Length)
        {
            return needed == 0;
        }

        var lowestDigit = position == 0 ? 1 : 0;

        for (var digit = 9; digit >= lowestDigit; digit--)
        {
            var nextNeeded = Reduce(needed - digit * residue.Weights[position], residue.Modulus);

            if (rest.Replay((position + 1, nextNeeded), rest))
            {
                digitChoice[state] = digit;
                return true;
            }
        }

        return false;
    }

    private static bool TryFillDescending(int position, char[] half, int digitCount, int divisor)
    {
        if (position == half.Length)
        {
            var candidate = Mirror(half, digitCount);
            return IsDivisible(candidate, divisor);
        }

        var lowestDigit = position == 0 ? 1 : 0;

        for (var digit = 9; digit >= lowestDigit; digit--)
        {
            half[position] = (char)('0' + digit);

            if (TryFillDescending(position + 1, half, digitCount, divisor))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsDivisible(string number, int divisor)
    {
        var remainder = 0;

        foreach (var digit in number)
        {
            remainder = (remainder * 10 + (digit - '0')) % divisor;
        }

        return remainder == 0;
    }

    private static int Reduce(int value, int divisor) => ((value % divisor) + divisor) % divisor;

    private static int HalfLength(int digitCount) => (digitCount + 1) / 2;

    private static string Mirror(char[] half, int digitCount)
    {
        var result = new char[digitCount];

        for (var i = 0; i < half.Length; i++)
        {
            result[i] = half[i];
            result[digitCount - 1 - i] = half[i];
        }

        return new string(result);
    }
}
