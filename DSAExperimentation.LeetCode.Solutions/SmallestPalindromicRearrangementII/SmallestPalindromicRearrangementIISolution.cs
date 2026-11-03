using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.LeetCode.Solutions.Conventions;
using PalindromeStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.Solutions.SmallestPalindromicRearrangementII;

// LeetCode 3518. Smallest Palindromic Rearrangement II: palindrome is already a
// palindrome; return its rank-th lexicographically smallest palindromic
// rearrangement (1-indexed), or "" if fewer than rank exist. As in Part I, every
// palindrome is determined by its left half (each letter's count halved, plus an
// optional odd middle letter) mirrored outward, and lexicographic order of the whole
// palindrome tracks lexicographic order of that left half exactly - so this reduces
// to "find the rank-th lexicographically smallest arrangement of a multiset" and
// mirror it.
[LeetCodeQuestion("smallest-palindromic-rearrangement-ii", LeetCodeAnswerOrder.AsWritten)]
internal static class SmallestPalindromicRearrangementIISolution
{
    // The textbook approach: plain depth-first backtracking over the half's letters
    // that stops early at the rank-th complete arrangement. Candidates yields letters
    // ascending, so depth-first visits complete arrangements in lexicographic order
    // and the rank-th TryCaptureKthArrangement call IS the answer. Deliberately the
    // slow arm: it walks every arrangement before the rank-th one node by node,
    // exactly what the counting strategy below exists to avoid. Written as plain
    // recursion rather than through Backtracking.Backtrack, since a baseline uses no
    // library implementation (ARCHITECTURE 17.5).
    public static string RearrangeByBacktrackingRank(string palindrome, int rank)
    {
        var (halfCounts, middle) = SplitCounts(palindrome);
        var halfLength = halfCounts.Sum();

        var state = new RankState(halfCounts, new List<char>(halfLength), rank);

        if (!TrySearchRankedArrangement(state, halfLength))
        {
            return string.Empty;
        }

        return BuildPalindromeWithBclStack(state.Result!, middle);
    }

    // The baseline's mirror: the same push-then-pop reversal as BuildPalindrome below,
    // over a BCL Stack<char>.
    private static string BuildPalindromeWithBclStack(string left, char? middle)
    {
        var stack = new Stack<char>();

        foreach (var c in left)
        {
            stack.Push(c);
        }

        var right = new char[left.Length];

        for (var i = 0; stack.TryPop(out var c); i++)
        {
            right[i] = c;
        }

        return left + (middle?.ToString() ?? string.Empty) + new string(right);
    }

    // The composed approach: never materializes an arrangement it isn't going to
    // return, only counts them. At each position, CountArrangementsCapped answers
    // "how many completions does placing this letter leave" - capped at the still-
    // outstanding rank, so it is always an O(halfLength)-bounded integer
    // computation, never the astronomical exact multinomial coefficient a
    // length-10^4 string's half can have.
    public static string RearrangeByCountingGreedy(string palindrome, int rank)
    {
        var (halfCounts, middle) = SplitCounts(palindrome);
        var halfLength = halfCounts.Sum();

        if (CountArrangementsCapped(halfCounts, rank) < rank)
        {
            return string.Empty;
        }

        var left = new char[halfLength];
        var remainingRank = (long)rank;

        for (var position = 0; position < halfLength; position++)
        {
            remainingRank = PlaceRankedLetter(halfCounts, left, position, remainingRank);
        }

        return BuildPalindrome(new string(left), middle);
    }

    // The letter this position gets: the smallest letter left whose completions still
    // cover the outstanding rank, placed into the half in place of the rank that
    // letter's own completions consume - returning the rank left over for the next
    // position, which is unchanged when the letter covers the whole of it.
    private static long PlaceRankedLetter(int[] halfCounts, char[] left, int position, long remainingRank)
    {
        for (var c = 0; c < EnglishLetters.Size; c++)
        {
            if (halfCounts[c] == 0)
            {
                continue;
            }

            halfCounts[c]--;
            var completions = CountArrangementsCapped(halfCounts, remainingRank);

            if (remainingRank <= completions)
            {
                left[position] = (char)('a' + c);
                return remainingRank;
            }

            remainingRank -= completions;
            halfCounts[c]++;
        }

        return remainingRank;
    }

    // Mirrors SmallestPalindromicRearrangementISolution.RearrangeByCharStack: LIFO
    // order undoes the left half's own order, so pushing it and popping it back out
    // produces its reverse without a second pass that re-derives it.
    private static string BuildPalindrome(string left, char? middle)
    {
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

        return left + (middle?.ToString() ?? string.Empty) + new string(right);
    }

    // One node of the search: a complete arrangement counts toward the rank, and
    // otherwise each letter still available is chosen in ascending order, searched
    // below, and unchosen on the way back out - stopping the moment a search below
    // reports the rank-th arrangement found.
    private static bool TrySearchRankedArrangement(RankState state, int halfLength)
    {
        if (state.Path.Count == halfLength && TryCaptureKthArrangement(state))
        {
            return true;
        }

        foreach (var letter in Candidates(state))
        {
            Choose(state, letter);
            var stopped = TrySearchRankedArrangement(state, halfLength);
            Unchoose(state, letter);

            if (stopped)
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<int> Candidates(RankState state)
    {
        for (var c = 0; c < EnglishLetters.Size; c++)
        {
            if (state.Counts[c] > 0)
            {
                yield return c;
            }
        }
    }

    private static void Choose(RankState state, int letter)
    {
        state.Counts[letter]--;
        state.Path.Add((char)('a' + letter));
    }

    private static void Unchoose(RankState state, int letter)
    {
        state.Path.RemoveAt(state.Path.Count - 1);
        state.Counts[letter]++;
    }

    // Snapshots Path into Result the instant this is the rank-th arrangement, rather
    // than letting the caller read state.Path once the search returns: the recursion
    // calls Unchoose unconditionally on the way back out, stop signal or not, so by
    // the time its caller sees `true` the path that earned it has already been
    // unwound one Unchoose at a time.
    private static bool TryCaptureKthArrangement(RankState state)
    {
        state.Remaining--;

        if (state.Remaining != 0)
        {
            return false;
        }

        state.Result = new string(state.Path.ToArray());
        return true;
    }

    // Number of distinct arrangements of counts, saturating at cap+1 the moment the
    // running total exceeds it - the exact value stops mattering once it does, and
    // this is what keeps the whole strategy inside long arithmetic despite the true
    // multinomial coefficient for a 10^4-length half being far larger than any
    // fixed-width integer. Built by inserting one occurrence at a time: placing the
    // i-th copy of a letter into a sequence of `remaining` symbols-so-far multiplies
    // the arrangement count by remaining/i (choose its slot, divide out the i
    // now-interchangeable copies), an exact-integer recurrence regardless of
    // insertion order.
    private static long CountArrangementsCapped(int[] counts, long cap)
    {
        var arrangements = 1L;
        var remaining = 0L;

        foreach (var count in counts)
        {
            for (var i = 1; i <= count; i++)
            {
                remaining++;
                arrangements = arrangements * remaining / i;

                if (arrangements > cap)
                {
                    return cap + 1;
                }
            }
        }

        return arrangements;
    }

    private static (int[] HalfCounts, char? Middle) SplitCounts(string palindrome)
    {
        var counts = new int[EnglishLetters.Size];

        foreach (var c in palindrome)
        {
            counts[c - 'a']++;
        }

        var halfCounts = new int[EnglishLetters.Size];
        char? middle = null;

        for (var c = 0; c < EnglishLetters.Size; c++)
        {
            halfCounts[c] = counts[c] / 2;

            if (counts[c] % 2 == 1)
            {
                middle = (char)('a' + c);
            }
        }

        return (halfCounts, middle);
    }

    private sealed class RankState(int[] counts, List<char> path, long remaining)
    {
        public int[] Counts { get; } = counts;

        public List<char> Path { get; } = path;

        public long Remaining { get; set; } = remaining;

        public string? Result { get; set; }
    }

}
