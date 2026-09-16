using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.FindTheCountOfGoodIntegers;

// LeetCode 3272. Find the Count of Good Integers: digitCount is small here (<= 10),
// unlike LC 3260's n up to 1e5, so every k-palindromic number of that many digits can
// just be enumerated directly instead of needing a digit-DP. An integer is "good" iff
// SOME rearrangement of its digits is k-palindromic, so the real count is over
// distinct digit multisets, not distinct palindromes: CountGoodIntegers groups every
// k-palindromic palindrome found by its sorted-digit signature and, once per distinct
// signature, adds the number of arrangements of that multiset into digitCount digits
// (a multinomial coefficient) minus the ones that would start with a leading zero.
//
// CountByPalindromeEnumeration walks the palindrome's own half (positions [0, h),
// h = ceil(digitCount/2)) with a hand-rolled recursion - the "what you'd write
// without this repo" arm CountByBacktrackEnumeration has to match.
//
// CountByBacktrackEnumeration asks the same question through Backtrack.Search:
// PalindromeHalfState answers Candidates itself (digits 0-9, 1-9 at the leading
// position), and onSolution records the completed half's mirrored, k-palindromic
// signature - the same "the composed arm just supplies Candidates/Choose/Unchoose,
// the enumeration itself is the repo's" shape
// GenerateParenthesesSolution.GenerateByBacktracking already proves out.
internal static class FindTheCountOfGoodIntegersSolution
{
    public static long CountByPalindromeEnumeration(int digitCount, int divisor)
    {
        var half = new char[HalfLength(digitCount)];
        var signatures = new HashSet<string>();

        EnumerateHalves((0, half), digitCount, divisor, signatures);

        return CountGoodIntegers(signatures, digitCount);
    }

    // The half being built, carried as one value because a buffer and the count of
    // slots it has filled only mean something together - the same "the half so far"
    // PalindromeHalfState names for the composed arm below, spelled as the tuple the
    // hand-rolled walk needs.
    private static void EnumerateHalves(
        (int Position, char[] Digits) half, int digitCount, int divisor, HashSet<string> signatures)
    {
        if (half.Position == half.Digits.Length)
        {
            var candidate = Mirror(half.Digits, digitCount);
            RecordIfKPalindromic(candidate, divisor, signatures);
            return;
        }

        var lowestDigit = half.Position == 0 ? 1 : 0;

        for (var digit = lowestDigit; digit <= 9; digit++)
        {
            half.Digits[half.Position] = (char)('0' + digit);
            EnumerateHalves((half.Position + 1, half.Digits), digitCount, divisor, signatures);
        }
    }

    public static long CountByBacktrackEnumeration(int digitCount, int divisor)
    {
        var state = new PalindromeHalfState(HalfLength(digitCount));
        var signatures = new HashSet<string>();

        Backtrack.Search<PalindromeHalfState, int>(
            state,
            isSolution: s => s.IsComplete,
            candidates: s => s.IsComplete ? NoCandidates() : s.Candidates(),
            choose: (s, digit) => s.Choose(digit),
            unchoose: (s, digit) => s.Unchoose(digit),
            onSolution: s =>
            {
                var candidate = Mirror(s.Digits, digitCount);
                RecordIfKPalindromic(candidate, divisor, signatures);
            });

        return CountGoodIntegers(signatures, digitCount);
    }

    // A completed half has no digit left to place, so it offers the walk nothing to
    // choose - the same ending isSolution already reports.
    private static IEnumerable<int> NoCandidates() => [];

    private static void RecordIfKPalindromic(string full, int divisor, HashSet<string> signatures)
    {
        if (IsDivisible(full, divisor))
        {
            signatures.Add(Canonical(full));
        }
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

    private static string Canonical(string number)
    {
        var chars = number.ToCharArray();
        Array.Sort(chars);
        return new string(chars);
    }

    // One distinct digit multiset (LeetCode's "good" integer) can be witnessed by
    // more than one k-palindromic arrangement - signatures is already deduplicated,
    // so each multiset's full arrangement count is added exactly once here.
    private static long CountGoodIntegers(HashSet<string> signatures, int digitCount)
    {
        var factorial = BuildFactorials(digitCount);
        var total = 0L;

        foreach (var signature in signatures)
        {
            var counts = new int[10];

            foreach (var digit in signature)
            {
                counts[digit - '0']++;
            }

            total += ArrangementsWithoutLeadingZero(counts, digitCount, factorial);
        }

        return total;
    }

    private static long ArrangementsWithoutLeadingZero(int[] counts, int digitCount, long[] factorial)
    {
        var arrangements = Multinomial(counts, digitCount, factorial);

        if (counts[0] == 0)
        {
            return arrangements;
        }

        var leadingZeroCounts = (int[])counts.Clone();
        leadingZeroCounts[0]--;

        return arrangements - Multinomial(leadingZeroCounts, digitCount - 1, factorial);
    }

    private static long Multinomial(int[] counts, int total, long[] factorial)
    {
        var arrangements = factorial[total];

        foreach (var count in counts)
        {
            arrangements /= factorial[count];
        }

        return arrangements;
    }

    private static long[] BuildFactorials(int digitCount)
    {
        var factorial = new long[digitCount + 1];
        factorial[0] = 1;

        for (var i = 1; i <= digitCount; i++)
        {
            factorial[i] = factorial[i - 1] * i;
        }

        return factorial;
    }

    private static string Mirror(IReadOnlyList<char> half, int digitCount)
    {
        var result = new char[digitCount];

        for (var i = 0; i < half.Count; i++)
        {
            result[i] = half[i];
            result[digitCount - 1 - i] = half[i];
        }

        return new string(result);
    }

    private static string Mirror(IReadOnlyList<int> half, int digitCount)
    {
        var result = new char[digitCount];

        for (var i = 0; i < half.Count; i++)
        {
            var digit = (char)('0' + half[i]);
            result[i] = digit;
            result[digitCount - 1 - i] = digit;
        }

        return new string(result);
    }

    private static int HalfLength(int digitCount) => (digitCount + 1) / 2;
}
