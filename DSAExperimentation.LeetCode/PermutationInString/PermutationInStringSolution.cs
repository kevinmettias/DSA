using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PermutationInString;

// LeetCode 567. Permutation in String: does s2 contain a contiguous substring
// that is a permutation of s1? Both strategies compare character-frequency
// counts over a fixed-size window of s1.Length; they differ in whether the
// window's frequency map is rebuilt from scratch at every position or
// maintained incrementally as it slides, using a running "matched distinct
// characters" counter - the same shape this repo's Find All Anagrams in a
// String (438) coverage uses.
internal static class PermutationInStringSolution
{
    // The textbook answer: rebuild and compare a fresh frequency Dictionary for
    // every window start, O(|s2| * |s1|). Deliberately written without this
    // repo's HashMap - it is the arm the sliding-window strategy below has to
    // justify itself against.
    public static bool HasPermutationByPerWindowRebuild(PermutationPattern s1, SearchedText s2)
    {
        if (s1.Text.Length > s2.Text.Length)
        {
            return false;
        }

        var target = BuildFrequencyMap(s1.Text);

        for (var start = 0; start <= s2.Text.Length - s1.Text.Length; start++)
        {
            var slice = s2.Text.Substring(start, s1.Text.Length);
            var window = BuildFrequencyMap(slice);
            if (HasEqualFrequencies(window, target))
            {
                return true;
            }
        }

        return false;
    }

    private static Dictionary<char, int> BuildFrequencyMap(string value)
    {
        var counts = new Dictionary<char, int>();

        foreach (var c in value)
        {
            counts.TryGetValue(c, out var count);
            counts[c] = count + 1;
        }

        return counts;
    }

    private static bool HasEqualFrequencies(Dictionary<char, int> window, Dictionary<char, int> target)
    {
        if (window.Count != target.Count)
        {
            return false;
        }

        foreach (var (key, expected) in target)
        {
            if (!window.TryGetValue(key, out var actual) || actual != expected)
            {
                return false;
            }
        }

        return true;
    }

    // This repo's own HashMap<char,int>: one window map maintained incrementally
    // as it slides, with a running "matched distinct characters" counter so a
    // full match is a count comparison rather than a per-window walk -
    // O(|s1| + |s2|).
    public static bool HasPermutationBySlidingWindow(PermutationPattern s1, SearchedText s2)
    {
        if (s1.Text.Length > s2.Text.Length)
        {
            return false;
        }

        var need = BuildNeedMap(s1.Text);
        var window = new HashMap<char, int>();
        var matched = 0;
        var context = new SlidingWindowContext(s1.Text, need, window);

        for (var i = 0; i < s2.Text.Length; i++)
        {
            if (HasWindowMatch(context, s2.Text, i, ref matched))
            {
                return true;
            }
        }

        return false;
    }

    private static HashMap<char, int> BuildNeedMap(string pattern)
    {
        var frequencies = new HashMap<char, int>();

        foreach (var c in pattern)
        {
            frequencies.TryGetValue(c, out var count);
            frequencies.Set(c, count + 1);
        }

        return frequencies;
    }

    // One step of the fixed-size window: absorbs the character at enteringIndex,
    // then - once the window has reached s1's length - checks for a full match and
    // evicts the character leaving the window. Returns true once the window matches
    // s1 exactly.
    private static bool HasWindowMatch(
        SlidingWindowContext context, string s2, int enteringIndex, ref int matched)
    {
        var (s1, need, window) = context;

        AbsorbEnteringChar(need, window, s2[enteringIndex], ref matched);

        if (enteringIndex < s1.Length - 1)
        {
            return false;
        }

        if (matched == need.Count)
        {
            return true;
        }

        EvictLeavingChar(need, window, s2[enteringIndex - s1.Length + 1], ref matched);
        return false;
    }

    private static void AbsorbEnteringChar(HashMap<char, int> need, HashMap<char, int> window, char entering, ref int matched)
    {
        if (!need.TryGetValue(entering, out var needed))
        {
            return;
        }

        window.TryGetValue(entering, out var count);
        window.Set(entering, count + 1);
        if (count + 1 == needed)
        {
            matched++;
        }
    }

    private static void EvictLeavingChar(HashMap<char, int> need, HashMap<char, int> window, char leaving, ref int matched)
    {
        if (!need.TryGetValue(leaving, out var neededLeaving))
        {
            return;
        }

        window.TryGetValue(leaving, out var leavingCount);
        if (leavingCount == neededLeaving)
        {
            matched--;
        }

        window.Set(leaving, leavingCount - 1);
    }

    private readonly record struct SlidingWindowContext(string S1, HashMap<char, int> Need, HashMap<char, int> Window);
}
