using DupStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.LexicographicallySmallestStringAfterDeletingDuplicateCharacters;

// LeetCode 3816. Lexicographically Smallest String After Deleting Duplicate
// Characters: repeatedly choose a letter that currently appears at least twice and
// delete one occurrence of it; return the lexicographically smallest string
// reachable this way.
//
// Both strategies apply the same greedy rule: a character may be dropped only if
// it still has another copy left afterward (in the stack or the untouched
// suffix), and it should be dropped whenever it is immediately followed by
// something smaller, since a smaller prefix always wins lexicographically. What
// remains at the very end gets one more pass, since a trailing character with a
// spare copy elsewhere can only shrink the string by leaving - a strict prefix of
// a string is always lexicographically smaller than the string itself.
internal static class LexicographicallySmallestStringAfterDeletingDuplicateCharactersSolution
{
    private const int AlphabetSize = 26;

    // The textbook answer: repeatedly bubble through the buffer for the leftmost
    // removable character - one with a spare copy left over that either precedes
    // something smaller (a removable inversion) or has nothing after it at all
    // (a redundant trailing duplicate, which a strict prefix always beats) - and
    // restart the scan after every deletion. Deliberately without this repo's
    // Stack, the arm the single-pass sweep below has to justify itself against.
    public static string SmallestStringByRepeatedScan(string s)
    {
        var counts = CountLetters(s);
        var chars = new List<char>(s);

        var deletedSomething = true;

        while (deletedSomething)
        {
            deletedSomething = false;

            for (var i = 0; i < chars.Count; i++)
            {
                var isTrailing = i == chars.Count - 1;
                var precedesSomethingSmaller = !isTrailing && chars[i] > chars[i + 1];

                if (counts[chars[i] - 'a'] <= 1 || !(isTrailing || precedesSomethingSmaller))
                {
                    continue;
                }

                counts[chars[i] - 'a']--;
                chars.RemoveAt(i);
                deletedSomething = true;
                break;
            }
        }

        return new string([.. chars]);
    }

    // Single left-to-right sweep with this repo's own Stack<char>: cnt[c] tracks
    // how many copies of c remain undeleted (in the stack plus the untouched
    // suffix), decremented only when a copy is actually dropped - the same
    // LargestRectangleInHistogramSolution precedent (Stack<int> read via
    // TryPeek/TryPop in a monotonic sweep), applied to characters instead of bar
    // indices.
    public static string SmallestStringByMonotonicStack(string s)
    {
        var counts = CountLetters(s);
        var stack = new DupStack();

        foreach (var c in s)
        {
            while (stack.TryPeek(out var top) && top > c && counts[top - 'a'] > 1)
            {
                stack.TryPop(out _);
                counts[top - 'a']--;
            }

            stack.Push(c);
        }

        while (stack.TryPeek(out var trailing) && counts[trailing - 'a'] > 1)
        {
            stack.TryPop(out _);
            counts[trailing - 'a']--;
        }

        return DrainToString(stack);
    }

    private static int[] CountLetters(string s)
    {
        var counts = new int[AlphabetSize];

        foreach (var c in s)
        {
            counts[c - 'a']++;
        }

        return counts;
    }

    private static string DrainToString(DupStack stack)
    {
        var chars = new List<char>();

        while (stack.TryPop(out var c))
        {
            chars.Add(c);
        }

        chars.Reverse();
        return new string([.. chars]);
    }
}
