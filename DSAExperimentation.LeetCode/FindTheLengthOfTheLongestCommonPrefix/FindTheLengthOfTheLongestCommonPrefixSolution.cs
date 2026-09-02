using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.FindTheLengthOfTheLongestCommonPrefix;

// LeetCode 3043. Find the Length of the Longest Common Prefix: the longest
// common prefix over every pair (x in arr1, y in arr2), taken as digit strings.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class FindTheLengthOfTheLongestCommonPrefixSolution
{
    // The textbook all-pairs scan: every x compared digit by digit against
    // every y. The arm the trie strategy below has to beat.
    public static int LongestPrefixLengthByBruteForce(int[] arr1, int[] arr2)
    {
        var best = 0;

        foreach (var x in arr1)
        {
            var xDigits = x.ToString();

            foreach (var y in arr2)
            {
                best = Math.Max(best, SharedPrefixLength(xDigits, y.ToString()));
            }
        }

        return best;
    }

    private static int SharedPrefixLength(string a, string b)
    {
        var max = Math.Min(a.Length, b.Length);
        var length = 0;

        while (length < max && a[length] == b[length])
        {
            length++;
        }

        return length;
    }

    // Every arr1 value, as its digit string, is inserted into this repo's own
    // Trie<TValue> once; each arr2 value then walks only its OWN digit count of
    // HasPrefix checks (at most 10, the widest int) to find how far into the
    // trie it can follow, rather than being re-compared against arr1 one value
    // at a time.
    public static int LongestPrefixLengthByTrie(int[] arr1, int[] arr2)
    {
        var trie = BuildDigitTrie(arr1);
        return LongestPrefixLengthByTrie(trie, arr2);
    }

    public static int LongestPrefixLengthByTrie(Trie<bool> trie, int[] arr2)
    {
        var best = 0;

        foreach (var y in arr2)
        {
            best = Math.Max(best, MatchedPrefixLength(trie, y.ToString()));
        }

        return best;
    }

    private static int MatchedPrefixLength(Trie<bool> trie, string digits)
    {
        var length = 0;

        while (length < digits.Length && trie.HasPrefix(digits[..(length + 1)]))
        {
            length++;
        }

        return length;
    }

    public static Trie<bool> BuildDigitTrie(int[] arr1)
    {
        var trie = new Trie<bool>();

        foreach (var x in arr1)
        {
            trie.Set(x.ToString(), true);
        }

        return trie;
    }
}
