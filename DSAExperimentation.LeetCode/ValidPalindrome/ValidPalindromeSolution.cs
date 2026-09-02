namespace DSAExperimentation.LeetCode.ValidPalindrome;

// LeetCode 125. Valid Palindrome: reading only letters and digits, case-
// insensitively, is the string the same forwards and backwards?
//
// There is exactly one strategy: pre-migration the test's private helper was
// the only real implementation - the benchmark's two [Benchmark] arms were
// untested placeholders (`=> 1`), not a second arm to reconcile. A pair of
// pointers closes in from both ends, skipping non-alphanumeric characters on
// either side before comparing, so the string is never copied or reversed.
internal static class ValidPalindromeSolution
{
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
