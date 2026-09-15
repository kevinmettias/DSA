using RepoCharSet = DSAExperimentation.DataStructures.Set.Set<char>;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.RemoveDuplicateLetters;

// LeetCode 316. Remove Duplicate Letters: from a string of lowercase letters,
// build the lexicographically smallest subsequence that still contains every
// distinct letter exactly once.
//
// The two strategies differ in how they discover, at each step, which letter is
// safe to place next: the textbook approach recomputes it from scratch by
// rescanning the remaining substring and recursing; this repo's own approach
// walks the string once, maintaining a monotonic candidate stack (this repo's
// own Stack<char>) and a set of which letters are already on it (Set<char>).
internal static class RemoveDuplicateLettersSolution
{
    private const int AlphabetSize = 26;

    // The textbook recursive alternative: at each of up to 26 levels, finds the
    // earliest letter whose remaining suffix still contains every distinct
    // letter of the current string, then recurses on that suffix with the
    // letter stripped out. Deliberately written without this repo's
    // primitives - it is the arm the composed solution below has to justify
    // itself against.
    public static string SmallestSubsequenceByRecursiveSplit(string s)
    {
        if (s.Length == 0)
        {
            return string.Empty;
        }

        var distinctInS = DistinctLetters(s);

        for (var candidate = 0; candidate < AlphabetSize; candidate++)
        {
            var placed = TryPlaceCandidate(s, distinctInS, candidate);

            if (placed is not null)
            {
                return placed;
            }
        }

        return string.Empty;
    }

    private static string? TryPlaceCandidate(string s, bool[] distinctInS, int candidate)
    {
        if (!distinctInS[candidate])
        {
            return null;
        }

        var c = (char)('a' + candidate);
        var suffix = s[s.IndexOf(c)..];

        if (!SameDistinctLetters(distinctInS, DistinctLetters(suffix)))
        {
            return null;
        }

        var remainder = suffix.Replace(c.ToString(), string.Empty);

        return c + SmallestSubsequenceByRecursiveSplit(remainder);
    }

    private static bool SameDistinctLetters(bool[] left, bool[] right)
    {
        for (var i = 0; i < AlphabetSize; i++)
        {
            if (left[i] != right[i])
            {
                return false;
            }
        }

        return true;
    }

    // Walks the string once, greedily maintaining a monotonic candidate stack:
    // skip a letter already placed, otherwise pop any larger letter still on
    // the stack that reappears later in the string, then push. Popping keeps
    // the candidate answer lexicographically smallest without ever rescanning.
    public static string SmallestSubsequenceByStackAndSet(string s)
    {
        var lastOccurrence = new int[AlphabetSize];
        for (var i = 0; i < s.Length; i++)
        {
            lastOccurrence[s[i] - 'a'] = i;
        }

        var candidate = new CandidateStack();

        for (var i = 0; i < s.Length; i++)
        {
            AppendCandidateLetter(candidate, lastOccurrence, s[i], i);
        }

        return DrainToAnswer(candidate);
    }

    private static void AppendCandidateLetter(CandidateStack candidate, int[] lastOccurrence, char c, int index)
    {
        if (candidate.OnStack.Has(c))
        {
            return;
        }

        while (candidate.Stack.TryPeek(out var top) && ShouldPopTop(top, c, lastOccurrence, index))
        {
            candidate.Stack.TryPop(out _);
            candidate.OnStack.TryRemove(top);
        }

        candidate.Stack.Push(c);
        candidate.OnStack.TryAdd(c);
    }

    // The letter on top is larger than the one arriving and still appears later in the
    // string, so dropping it now keeps the answer smaller without ever losing it.
    private static bool ShouldPopTop(char top, char incoming, int[] lastOccurrence, int index) =>
        top > incoming && lastOccurrence[top - 'a'] > index;

    // The candidate stack read off into the answer, bottom of the stack first.
    private static string DrainToAnswer(CandidateStack candidate)
    {
        var result = new char[candidate.Stack.Count];

        for (var i = result.Length - 1; i >= 0; i--)
        {
            candidate.Stack.TryPop(out result[i]);
        }

        return new string(result);
    }

    private static bool[] DistinctLetters(string s)
    {
        var seen = new bool[AlphabetSize];
        foreach (var c in s)
        {
            seen[c - 'a'] = true;
        }

        return seen;
    }

    // The candidate answer (as a stack) and which letters are currently on it,
    // always mutated together.
    private sealed record CandidateStack
    {
        public RepoCharStack Stack { get; } = new();
        public RepoCharSet OnStack { get; } = new();
    }
}
