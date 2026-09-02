using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.LongestSubstringWithoutRepeatingCharacters;

// LeetCode 3. Longest Substring Without Repeating Characters: how long is the
// longest run of the string with no character repeated.
//
// The composed solution tracks each character's last-seen index in this repo's
// own HashMap<char,int> and jumps the window's left edge straight past a repeat
// instead of re-scanning from the next start index - the baseline it has to beat.
internal static class LongestSubstringWithoutRepeatingCharactersSolution
{
    // The textbook answer: re-scan forward from every start index until a repeat
    // is hit. Deliberately written without this repo's primitives - a BCL
    // HashSet per start index costs O(n^2) overall versus the sliding window's
    // O(n).
    public static int FindLengthByBruteForce(string text)
    {
        var longest = 0;

        for (var start = 0; start < text.Length; start++)
        {
            var seen = new HashSet<char>();

            for (var end = start; end < text.Length; end++)
            {
                if (!seen.Add(text[end]))
                {
                    break;
                }

                longest = Math.Max(longest, end - start + 1);
            }
        }

        return longest;
    }

    // A single sliding-window pass: jump the window's left edge straight past a
    // repeat instead of shrinking it one character at a time.
    public static int FindLengthBySlidingWindowHashMap(string text)
    {
        var lastSeenIndex = new HashMap<char, int>();
        var windowStart = 0;
        var longest = 0;

        for (var windowEnd = 0; windowEnd < text.Length; windowEnd++)
        {
            var current = text[windowEnd];

            if (lastSeenIndex.TryGetValue(current, out var previousIndex) && previousIndex >= windowStart)
            {
                windowStart = previousIndex + 1;
            }

            lastSeenIndex.Set(current, windowEnd);
            longest = Math.Max(longest, windowEnd - windowStart + 1);
        }

        return longest;
    }
}
