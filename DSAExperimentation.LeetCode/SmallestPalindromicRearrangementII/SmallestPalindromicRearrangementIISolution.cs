using DSAExperimentation.Algorithms.Backtracking;
using PalindromeStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.LeetCode.SmallestPalindromicRearrangementII;

// LeetCode 3518. Smallest Palindromic Rearrangement II: palindrome is already a
// palindrome; return its rank-th lexicographically smallest palindromic
// rearrangement (1-indexed), or "" if fewer than rank exist. As in Part I, every
// palindrome is determined by its left half (each letter's count halved, plus an
// optional odd middle letter) mirrored outward, and lexicographic order of the whole
// palindrome tracks lexicographic order of that left half exactly - so this reduces
// to "find the rank-th lexicographically smallest arrangement of a multiset" and
// mirror it.
internal static class SmallestPalindromicRearrangementIISolution
{
    private const int AlphabetSize = 26;

    // The textbook approach: Backtracking.Backtrack's own doc comment names
    // "Permutations" as exactly this shape (Search's exhaustive-enumeration case) -
    // here TrySearch's early-stop variant instead, since Candidates already yields
    // letters ascending, so depth-first visits complete arrangements in
    // lexicographic order and the rank-th TryCaptureKthArrangement call IS the
    // answer. Deliberately the slow arm: it walks every arrangement before the
    // rank-th one node by node, exactly what the counting strategy below exists to
    // avoid.
    public static string RearrangeByBacktrackingRank(string palindrome, int rank)
    {
        var (halfCounts, middle) = SplitCounts(palindrome);
        var halfLength = halfCounts.Sum();

        var state = new RankState(halfCounts, new List<char>(halfLength), rank);
        var steps = new BacktrackingSteps<RankState, int>(
            IsSolution: st => st.Path.Count == halfLength,
            Candidates: Candidates,
            Choose: Choose,
            Unchoose: Unchoose,
            OnSolution: TryCaptureKthArrangement);

        return Backtrack.TrySearch(state, steps)
            ? BuildPalindrome(state.Result!, middle)
            : string.Empty;
    }

    private static IEnumerable<int> Candidates(RankState state)
    {
        for (var c = 0; c < AlphabetSize; c++)
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
    // than letting the caller read state.Path once TrySearch returns: Backtrack's
    // own TryEachCandidate calls Unchoose unconditionally on the way back out, stop
    // signal or not, so by the time TrySearch's caller sees `true` the path that
    // earned it has already been unwound one Unchoose at a time.
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

    private sealed class RankState(int[] counts, List<char> path, long remaining)
    {
        public int[] Counts { get; } = counts;

        public List<char> Path { get; } = path;

        public long Remaining { get; set; } = remaining;

        public string? Result { get; set; }
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
        for (var c = 0; c < AlphabetSize; c++)
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
        var counts = new int[AlphabetSize];

        foreach (var c in palindrome)
        {
            counts[c - 'a']++;
        }

        var halfCounts = new int[AlphabetSize];
        char? middle = null;

        for (var c = 0; c < AlphabetSize; c++)
        {
            halfCounts[c] = counts[c] / 2;

            if (counts[c] % 2 == 1)
            {
                middle = (char)('a' + c);
            }
        }

        return (halfCounts, middle);
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
}
