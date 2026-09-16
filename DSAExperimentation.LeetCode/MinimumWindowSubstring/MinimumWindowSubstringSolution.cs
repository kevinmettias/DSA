using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MinimumWindowSubstring;

// LeetCode 76. Minimum Window Substring: the smallest substring of the searched
// text that contains every required character (respecting multiplicity), or "" if no
// such window exists.
//
// Both strategies track each needed character's remaining count and expand a
// window's right edge until every count is covered, then shrink from the left for
// as long as it stays covered. They differ in what "remaining count" is kept in:
// the naive baseline restarts that count from scratch at every start index using a
// BCL Dictionary, deliberately written without this repo's primitives - the arm
// the composed solution has to justify itself against. The composed solution keeps
// a single count alive across the whole scan in this repo's own HashMap<char,int>,
// the same primitive/precedent LongestSubstringWithoutRepeatingCharacters uses for
// its own sliding window.
internal static class MinimumWindowSubstringSolution
{
    // The textbook O(|s|^2) restart-from-every-start scan.
    public static string MinWindowByBruteForce(SearchedText searchedText, RequiredCharacters requiredCharacters)
    {
        var bestStart = 0;
        var bestLength = int.MaxValue;

        for (var start = 0; start < searchedText.Text.Length; start++)
        {
            var length = ShortestWindowFrom(searchedText, requiredCharacters, start);

            if (length < bestLength)
            {
                bestLength = length;
                bestStart = start;
            }
        }

        return bestLength == int.MaxValue
            ? string.Empty
            : searchedText.Text.Substring(bestStart, bestLength);
    }

    private static int ShortestWindowFrom(
        SearchedText searchedText, RequiredCharacters requiredCharacters, int start)
    {
        var need = BuildNeedCounts(requiredCharacters.Text);
        var missing = requiredCharacters.Text.Length;

        for (var end = start; end < searchedText.Text.Length; end++)
        {
            missing = ConsumeCharacter(need, searchedText.Text[end], missing);

            if (missing == 0)
            {
                return end - start + 1;
            }
        }

        return int.MaxValue;
    }

    private static Dictionary<char, int> BuildNeedCounts(string requiredText)
    {
        var need = new Dictionary<char, int>();

        foreach (var ch in requiredText)
        {
            need[ch] = need.GetValueOrDefault(ch) + 1;
        }

        return need;
    }

    private static int ConsumeCharacter(Dictionary<char, int> need, char incoming, int missing)
    {
        if (!need.TryGetValue(incoming, out var remaining))
        {
            return missing;
        }

        need[incoming] = remaining - 1;
        return remaining > 0 ? FewerMissing(missing) : missing;
    }

    // Consuming a character that was still needed leaves one fewer missing.
    private static int FewerMissing(int missing) => missing - 1;

    // A single O(|s| + |t|) pass tracking each needed character's remaining count in
    // this repo's own HashMap<char,int>, sliding the window's right edge forward and
    // only ever shrinking from the left once every needed character is covered.
    public static string MinWindowBySlidingWindowHashMap(
        SearchedText searchedText, RequiredCharacters requiredCharacters)
    {
        var need = BuildNeedHashMap(requiredCharacters.Text);
        var state = new WindowState(
            Left: 0, BestStart: 0, BestLength: int.MaxValue, Missing: requiredCharacters.Text.Length);

        for (var right = 0; right < searchedText.Text.Length; right++)
        {
            state = AdvanceRight(searchedText.Text, need, right, state);
        }

        return state.BestLength == int.MaxValue
            ? string.Empty
            : searchedText.Text.Substring(state.BestStart, state.BestLength);
    }

    private static HashMap<char, int> BuildNeedHashMap(string requiredText)
    {
        var need = new HashMap<char, int>();

        foreach (var ch in requiredText)
        {
            need.TryGetValue(ch, out var count);
            need.Set(ch, count + 1);
        }

        return need;
    }

    // The sliding window's loop-carried state: the window's left edge, the best
    // window found so far, and how many needed characters are still missing.
    private readonly record struct WindowState(int Left, int BestStart, int BestLength, int Missing);

    // Absorbs text[right] into the window, then shrinks from the left for as long as
    // every needed character is still covered.
    private static WindowState AdvanceRight(string text, HashMap<char, int> need, int right, WindowState state)
    {
        var incoming = text[right];
        if (need.TryGetValue(incoming, out var remaining))
        {
            need.Set(incoming, remaining - 1);
            if (remaining > 0)
            {
                state = state with { Missing = state.Missing - 1 };
            }
        }

        while (state.Missing == 0)
        {
            state = ShrinkWindow(text, need, right, state);
        }

        return state;
    }

    // One "shrink" step once every needed character is covered: records the window
    // if it beats the best found so far, then drops the leftmost character, freeing
    // it back into `need` and marking it missing again if it's now under-covered.
    private static WindowState ShrinkWindow(string text, HashMap<char, int> need, int right, WindowState state)
    {
        var (bestStart, bestLength) = UpdateBest(state.Left, right, state.BestStart, state.BestLength);
        var missing = ReleaseOutgoing(text, need, state.Left, state.Missing);

        return new WindowState(state.Left + 1, bestStart, bestLength, missing);
    }

    private static (int BestStart, int BestLength) UpdateBest(int left, int right, int bestStart, int bestLength)
    {
        if (right - left + 1 < bestLength)
        {
            bestStart = left;
            bestLength = right - left + 1;
        }

        return (bestStart, bestLength);
    }

    // Frees the leftmost character back into `need`; if it's now under-covered
    // (its need count crosses back above zero), the window is missing it again.
    private static int ReleaseOutgoing(string text, HashMap<char, int> need, int left, int missing)
    {
        var outgoing = text[left];
        if (need.TryGetValue(outgoing, out var freed))
        {
            need.Set(outgoing, freed + 1);
            if (freed >= 0)
            {
                missing++;
            }
        }

        return missing;
    }

    // The two ends of a minimum-window search, named for what they are in this problem
    // rather than left as two adjacent `string` positions a caller could hand over the
    // wrong way round with the compiler none the wiser. `searchedText` is the text a
    // window is cut from; `requiredCharacters` is the characters that window has to
    // cover.
    internal readonly record struct SearchedText(string Text);

    internal readonly record struct RequiredCharacters(string Text);
}
