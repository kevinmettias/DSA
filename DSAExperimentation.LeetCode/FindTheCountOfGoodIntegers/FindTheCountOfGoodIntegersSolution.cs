using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.FindTheCountOfGoodIntegers;

// LeetCode 3272. Find the Count of Good Integers: n is small here (<= 10), unlike LC
// 3260's n up to 1e5, so every k-palindromic n-digit number can just be enumerated
// directly instead of needing a digit-DP. An integer is "good" iff SOME rearrangement
// of its digits is k-palindromic, so the real count is over distinct digit
// multisets, not distinct palindromes: CountGoodIntegers groups every k-palindromic
// palindrome found by its sorted-digit signature and, once per distinct signature,
// adds the number of n-digit arrangements of that multiset (a multinomial
// coefficient) minus the ones that would start with a leading zero.
//
// CountByPalindromeEnumeration walks the palindrome's own half (positions
// [0, h), h = ceil(n/2)) with a hand-rolled recursion - the "what you'd write
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
    public static long CountByPalindromeEnumeration(int n, int k)
    {
        var half = new char[HalfLength(n)];
        var signatures = new HashSet<string>();

        EnumerateHalves(0, half, n, k, signatures);

        return CountGoodIntegers(signatures, n);
    }

    private static void EnumerateHalves(int position, char[] half, int n, int k, HashSet<string> signatures)
    {
        if (position == half.Length)
        {
            RecordIfKPalindromic(Mirror(half, n), k, signatures);
            return;
        }

        var lowestDigit = position == 0 ? 1 : 0;

        for (var digit = lowestDigit; digit <= 9; digit++)
        {
            half[position] = (char)('0' + digit);
            EnumerateHalves(position + 1, half, n, k, signatures);
        }
    }

    public static long CountByBacktrackEnumeration(int n, int k)
    {
        var state = new PalindromeHalfState(HalfLength(n));
        var signatures = new HashSet<string>();

        Backtrack.Search<PalindromeHalfState, int>(
            state,
            isSolution: s => s.IsComplete,
            candidates: s => s.IsComplete ? [] : s.Candidates(),
            choose: (s, digit) => s.Choose(digit),
            unchoose: (s, digit) => s.Unchoose(digit),
            onSolution: s => RecordIfKPalindromic(Mirror(s.Digits, n), k, signatures));

        return CountGoodIntegers(signatures, n);
    }

    private static void RecordIfKPalindromic(string full, int k, HashSet<string> signatures)
    {
        if (IsDivisible(full, k))
        {
            signatures.Add(Canonical(full));
        }
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

    private static string Canonical(string number)
    {
        var chars = number.ToCharArray();
        Array.Sort(chars);
        return new string(chars);
    }

    // One distinct digit multiset (LeetCode's "good" integer) can be witnessed by
    // more than one k-palindromic arrangement - signatures is already deduplicated,
    // so each multiset's full arrangement count is added exactly once here.
    private static long CountGoodIntegers(HashSet<string> signatures, int n)
    {
        var factorial = BuildFactorials(n);
        var total = 0L;

        foreach (var signature in signatures)
        {
            var counts = new int[10];

            foreach (var digit in signature)
            {
                counts[digit - '0']++;
            }

            total += ArrangementsWithoutLeadingZero(counts, n, factorial);
        }

        return total;
    }

    private static long ArrangementsWithoutLeadingZero(int[] counts, int n, long[] factorial)
    {
        var arrangements = Multinomial(counts, n, factorial);

        if (counts[0] == 0)
        {
            return arrangements;
        }

        var leadingZeroCounts = (int[])counts.Clone();
        leadingZeroCounts[0]--;

        return arrangements - Multinomial(leadingZeroCounts, n - 1, factorial);
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

    private static long[] BuildFactorials(int n)
    {
        var factorial = new long[n + 1];
        factorial[0] = 1;

        for (var i = 1; i <= n; i++)
        {
            factorial[i] = factorial[i - 1] * i;
        }

        return factorial;
    }

    private static string Mirror(IReadOnlyList<char> half, int n)
    {
        var result = new char[n];

        for (var i = 0; i < half.Count; i++)
        {
            result[i] = half[i];
            result[n - 1 - i] = half[i];
        }

        return new string(result);
    }

    private static string Mirror(IReadOnlyList<int> half, int n)
    {
        var result = new char[n];

        for (var i = 0; i < half.Count; i++)
        {
            var digit = (char)('0' + half[i]);
            result[i] = digit;
            result[n - 1 - i] = digit;
        }

        return new string(result);
    }

    private static int HalfLength(int n) => (n + 1) / 2;
}
