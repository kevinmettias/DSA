using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.CountSortedVowelStrings;

// LeetCode 1641. Count Sorted Vowel Strings: how many strings of length n consist
// only of vowels and are sorted lexicographically. A sorted string never steps back
// to an earlier vowel, so the answer is the number of non-decreasing sequences of
// length n over the five vowels - a choice at vowel v may only be followed by
// choices >= v, which is the one rule both strategies below encode.
//
// They differ in whether the strings are built at all: the baseline materializes
// every one of them and counts what it made, while the memoized recurrence counts
// by state - (remaining length, smallest allowed vowel index) - and never builds a
// string.
internal static class CountSortedVowelStringsSolution
{
    private const int VowelCount = 5;

    private static readonly char[] Vowels = ['a', 'e', 'i', 'o', 'u'];

    // The textbook brute force: backtrack over every non-decreasing vowel string,
    // materialize it, and report how many were produced. Deliberately BCL only - a
    // char buffer and a List<string> - because it is the arm the memoized
    // recurrence has to justify itself against, and the materializing IS its cost.
    public static int CountVowelStringsByBacktrackingEnumeration(int n)
    {
        var strings = new List<string>();

        Build(new char[n], position: 0, start: 0, strings);

        return strings.Count;
    }

    private static void Build(char[] buffer, int position, int start, List<string> strings)
    {
        if (position == buffer.Length)
        {
            strings.Add(new string(buffer));
            return;
        }

        for (var vowel = start; vowel < VowelCount; vowel++)
        {
            buffer[position] = Vowels[vowel];
            Build(buffer, position + 1, vowel, strings);
        }
    }

    // This repo's own Memoizer caches the same rule by (remaining length, smallest
    // allowed vowel), collapsing the enumeration above to one evaluation per state
    // with no string ever built - the same memoized-recurrence shape UniquePaths
    // and FibonacciNumber already use for their own recurrences.
    public static int CountVowelStringsByMemoizedRecurrence(int n)
    {
        return Memoizer.Memoize<(int Remaining, int Start), int>((n, 0), Count);

        int Count((int Remaining, int Start) state, Func<(int Remaining, int Start), int> count)
        {
            var (remaining, start) = state;

            if (remaining == 0)
            {
                return 1;
            }

            var total = 0;

            for (var vowel = start; vowel < VowelCount; vowel++)
            {
                total += count((remaining - 1, vowel));
            }

            return total;
        }
    }
}
