namespace DSAExperimentation.LeetCode.VowelsOfAllSubstrings;

// LeetCode 2063. Vowels of All Substrings: sum, over every substring of the word,
// the number of vowels it contains.
//
// No repo primitive applies - the same "closed-form single pass over the bare
// string" category GasStation/Candy/MaximumProductSubarray already established
// here. The two strategies are the O(n^2) enumeration of substrings and the O(n)
// per-vowel contribution count that replaces it; both answer with a long, because
// the total is quadratic in the word length and overflows an int well inside
// LeetCode's own 10^5 constraint.
internal static class VowelsOfAllSubstringsSolution
{
    // The textbook baseline: fix a start index, extend the substring one character
    // at a time, and add its running vowel count for every end index. Deliberately
    // nothing but a pair of loops over the string it is handed - it is what you
    // would write without noticing the closed form - and it does O(n^2) work.
    public static long CountVowelsBySubstringScan(string word)
    {
        long total = 0;

        for (var start = 0; start < word.Length; start++)
        {
            var vowelsInRun = 0;

            for (var end = start; end < word.Length; end++)
            {
                if (IsVowel(word[end]))
                {
                    vowelsInRun++;
                }

                total += vowelsInRun;
            }
        }

        return total;
    }

    // The vowel at index i is counted once by every substring that contains it, and
    // a substring contains it exactly when its start is one of the (i + 1) indices
    // at or before i and its end is one of the (n - i) indices at or after i. So the
    // whole answer is the sum of (i + 1) * (n - i) over the vowel positions - one
    // pass, no substrings enumerated at all.
    public static long CountVowelsByContributionFormula(string word)
    {
        var length = word.Length;
        long total = 0;

        for (var i = 0; i < length; i++)
        {
            if (IsVowel(word[i]))
            {
                total += (long)(i + 1) * (length - i);
            }
        }

        return total;
    }

    private static bool IsVowel(char character) => character is 'a' or 'e' or 'i' or 'o' or 'u';
}
