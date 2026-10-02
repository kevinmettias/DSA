namespace DSAExperimentation.LeetCode.ValidPalindrome;

// LeetCode 125. Valid Palindrome: reading only letters and digits, case-
// insensitively, is the string the same forwards and backwards?
//
// Two strategies: a two-pointer scan closes in from both ends without ever
// copying the string, or a normalized copy is built and compared against its own
// reversal. The scan is O(1) extra space but has to skip characters as it goes;
// the copy pays two full strings for a plain equality check.
internal static class ValidPalindromeSolution
{
    // Keep only letters and digits, lower-cased, then compare that copy with its
    // own reverse. It allocates two strings against the scan's none, but the
    // comparison itself is a straight string equality with no skip logic or index
    // bookkeeping.
    public static bool IsPalindromeByNormalizedReversal(string value)
    {
        var normalized = new string([.. value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant)]);
        var reversed = new string([.. normalized.Reverse()]);

        return normalized == reversed;
    }

    // The two-pointer arm: a pair of pointers closes in from both ends, skipping
    // non-alphanumeric characters on either side before comparing, so the string
    // is never copied or reversed.
    public static bool IsPalindromeByTwoPointerScan(string value)
    {
        var left = 0;
        var right = value.Length - 1;

        while (left < right)
        {
            while (left < right && !char.IsLetterOrDigit(value[left]))
            {
                left++;
            }

            while (left < right && !char.IsLetterOrDigit(value[right]))
            {
                right--;
            }

            if (char.ToLowerInvariant(value[left++]) != char.ToLowerInvariant(value[right--]))
            {
                return false;
            }
        }

        return true;
    }
}
