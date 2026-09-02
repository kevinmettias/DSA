using PalindromeStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.SmallestPalindromicRearrangementI;

// LeetCode 3517. Smallest Palindromic Rearrangement I: s is already a palindrome;
// return its lexicographically smallest palindromic permutation. Every letter's
// count is even except at most one (the middle letter, on odd length) - halving
// each count and laying the halves out in ascending letter order gives the
// smallest possible left half, and mirroring that half gives the smallest
// possible right half.
//
// The two strategies differ only in how the right half is produced from the left
// - a char[] the caller reverses at the end, or this repo's own Stack<char>, the
// same "LIFO order undoes the reversal" trick AddBinarySolution uses.
internal static class SmallestPalindromicRearrangementISolution
{
    private const int AlphabetSize = 26;

    public static string RearrangeByCharArrayReverse(string s)
    {
        var counts = CountLowercaseLetters(s);
        var left = BuildAscendingHalf(counts);
        var middle = MiddleLetter(counts);

        var right = new char[left.Length];

        for (var i = 0; i < left.Length; i++)
        {
            right[i] = left[left.Length - 1 - i];
        }

        return left + middle + new string(right);
    }

    public static string RearrangeByCharStack(string s)
    {
        var counts = CountLowercaseLetters(s);
        var left = BuildAscendingHalf(counts);
        var middle = MiddleLetter(counts);

        var stack = new PalindromeStack();

        foreach (var c in left)
        {
            stack.Push(c);
        }

        var right = new char[left.Length];

        for (var i = 0; stack.TryPop(out var c); i++)
        {
            right[i] = c;
        }

        return left + middle + new string(right);
    }

    private static int[] CountLowercaseLetters(string s)
    {
        var counts = new int[AlphabetSize];

        foreach (var c in s)
        {
            counts[c - 'a']++;
        }

        return counts;
    }

    // Ascending letter order is already the array's own iteration order, so no
    // sort is needed - each letter contributes half its count, floored, which
    // drops the one odd letter's leftover copy for MiddleLetter to report.
    private static string BuildAscendingHalf(int[] counts)
    {
        var chars = new List<char>();

        for (var c = 0; c < AlphabetSize; c++)
        {
            for (var copies = 0; copies < counts[c] / 2; copies++)
            {
                chars.Add((char)('a' + c));
            }
        }

        return new string(chars.ToArray());
    }

    private static string MiddleLetter(int[] counts)
    {
        for (var c = 0; c < AlphabetSize; c++)
        {
            if (counts[c] % 2 == 1)
            {
                return ((char)('a' + c)).ToString();
            }
        }

        return string.Empty;
    }
}
