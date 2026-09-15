using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.CheckIfAStringContainsAllBinaryCodesOfSizeK;

// LeetCode 1461. Check If a String Contains All Binary Codes of Size K: does the
// binary string `s` contain every one of the 2^k binary strings of length k as a
// substring?
//
// The two strategies come at it from opposite ends: enumerate the codes and search
// the text for each, or make one pass over the text and record which codes it
// happened to produce.
internal static class CheckIfAStringContainsAllBinaryCodesOfSizeKSolution
{
    private const char Zero = '0';

    // The textbook answer: build each of the 2^k codes and ask whether the text
    // contains it, bailing on the first one it does not. O(2^k * n * k) in the worst
    // case, and deliberately written with nothing but the BCL - it is the arm the
    // sliding pass below has to justify itself against.
    public static bool HasAllCodesByCodeSubstringSearch(string s, int k)
    {
        var total = 1 << k;

        for (var code = 0; code < total; code++)
        {
            var binary = ToBinaryString(code, k);
            if (!s.Contains(binary, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static string ToBinaryString(int code, int k)
    {
        var characters = new char[k];

        for (var i = k - 1; i >= 0; i--)
        {
            characters[i] = (char)(Zero + (code & 1));
            code >>= 1;
        }

        return new string(characters);
    }

    // One O(n) pass: fold each length-k window into an int by shifting the new bit
    // in and masking the one that fell out of range - the rolling-bitmask trick a
    // binary alphabet makes exact - and record every distinct code in this repo's
    // own Set<int> (HashMap-backed). The text holds every code exactly when the set
    // ends up with all 2^k of them.
    public static bool HasAllCodesBySlidingBitmask(string s, int k)
    {
        var total = 1 << k;

        // Fewer than 2^k + k - 1 characters cannot possibly yield 2^k distinct
        // windows, whatever they contain, so the pass is skipped outright.
        if (s.Length < total + k - 1)
        {
            return false;
        }

        return CollectDistinctCodes(s, k).Count == total;
    }

    // Folds each length-k window into an int by shifting the new bit in and masking
    // the one that fell out of range, returning every distinct code the text
    // produced.
    private static Set<int> CollectDistinctCodes(string s, int k)
    {
        var seen = new Set<int>();
        var mask = (1 << k) - 1;
        var code = 0;

        for (var i = 0; i < s.Length; i++)
        {
            code = ((code << 1) | (s[i] - Zero)) & mask;

            if (i >= k - 1)
            {
                seen.TryAdd(code);
            }
        }

        return seen;
    }
}
