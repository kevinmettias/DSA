namespace DSAExperimentation.LeetCode.ValidPalindromeII;

// LeetCode 680. Valid Palindrome II: can at most one character be deleted so the
// rest of the string reads the same forwards and backwards?
//
// The two-pointer scan - ValidPalindromeTests' (LC 125) own strategy, extended
// to retry once with either pointer skipped past the first mismatch - is the
// intended O(n) answer. The baseline is the textbook "try deleting each
// character in turn, then re-check the whole result" approach (O(n^2): n
// candidate strings, each an O(n) build-plus-check) it has to beat.
internal static class ValidPalindromeIISolution
{
    public static bool IsValidPalindromeByBruteForceDeletion(string text)
    {
        if (IsPalindromeRange(text, 0, text.Length - 1))
        {
            return true;
        }

        for (var skip = 0; skip < text.Length; skip++)
        {
            var candidate = text.Remove(skip, 1);

            if (IsPalindromeRange(candidate, 0, candidate.Length - 1))
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsValidPalindromeByMismatchSkip(string text)
    {
        var left = 0;
        var right = text.Length - 1;

        while (left < right)
        {
            if (text[left] != text[right])
            {
                return IsPalindromeRange(text, left + 1, right) || IsPalindromeRange(text, left, right - 1);
            }

            left++;
            right--;
        }

        return true;
    }

    private static bool IsPalindromeRange(string text, int left, int right)
    {
        while (left < right)
        {
            if (text[left] != text[right])
            {
                return false;
            }

            left++;
            right--;
        }

        return true;
    }
}
