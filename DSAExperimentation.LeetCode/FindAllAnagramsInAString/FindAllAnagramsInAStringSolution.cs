using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FindAllAnagramsInAString;

// LeetCode 438. Find All Anagrams in a String: every start index of a substring of the
// scanned text that is an anagram of the pattern.
//
// ByBruteForceRebuild is the textbook O(n*m) baseline - a fresh frequency count built
// and compared for every window start, written without this repo's own collections.
// BySlidingWindow keeps one window HashMap<char,int> alive across the whole scan
// (the same frequency-map shape ValidAnagramTests uses) plus a running "matched
// distinct characters" counter, so each character enters and leaves the window
// exactly once - O(|s| + |p|) instead of rebuilding a fresh frequency map for every
// window start.
internal static class FindAllAnagramsInAStringSolution
{
    // The textbook answer: a fresh BCL Dictionary<char,int> built and compared for
    // every window start. Deliberately written without this repo's own collections -
    // it is the arm the composed solution below has to justify itself against.
    public static List<int> FindAnagramIndicesByBruteForceRebuild(ScannedText scannedText, AnagramPattern pattern)
    {
        var result = new List<int>();
        if (pattern.Text.Length > scannedText.Text.Length)
        {
            return result;
        }

        var target = BuildFrequencyMap(pattern.Text);

        for (var start = 0; start <= scannedText.Text.Length - pattern.Text.Length; start++)
        {
            var candidate = scannedText.Text.Substring(start, pattern.Text.Length);
            var window = BuildFrequencyMap(candidate);
            if (HasSameFrequencies(window, target))
            {
                result.Add(start);
            }
        }

        return result;
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

    private static bool HasSameFrequencies(Dictionary<char, int> window, Dictionary<char, int> target)
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

    // This repo's own sliding window: two HashMap<char,int> frequency maps (window vs.
    // target) plus a running "matched distinct characters" counter, so each character
    // enters and leaves the window exactly once.
    public static List<int> FindAnagramIndicesBySlidingWindow(ScannedText scannedText, AnagramPattern pattern)
    {
        var result = new List<int>();
        if (pattern.Text.Length > scannedText.Text.Length)
        {
            return result;
        }

        var need = BuildRepoFrequencyMap(pattern.Text);
        var state = new AnagramWindowState(need);

        for (var i = 0; i < scannedText.Text.Length; i++)
        {
            if (state.HasAnagramEndingAt(i, scannedText.Text, pattern.Text.Length))
            {
                result.Add(i - pattern.Text.Length + 1);
            }
        }

        return result;
    }

    private static HashMap<char, int> BuildRepoFrequencyMap(string pattern)
    {
        var need = new HashMap<char, int>();
        foreach (var character in pattern)
        {
            need.TryGetValue(character, out var count);
            need.Set(character, count + 1);
        }

        return need;
    }

    private sealed class AnagramWindowState(HashMap<char, int> need)
    {
        private readonly HashMap<char, int> _window = new();
        private int _matched;

        public bool IsFullMatch => _matched == need.Count;

        public bool HasAnagramEndingAt(int endIndex, string text, int windowLength)
        {
            AbsorbEntering(text[endIndex]);

            if (endIndex < windowLength - 1)
            {
                return false;
            }

            return HasFullMatchBeforeEvicting(endIndex, text, windowLength);
        }

        private void AbsorbEntering(char character)
        {
            if (!need.TryGetValue(character, out var needed))
            {
                return;
            }

            _window.TryGetValue(character, out var count);
            _window.Set(character, count + 1);

            if (count + 1 == needed)
            {
                _matched++;
            }
        }

        // The match state is read BEFORE the leaving character is evicted: it is the
        // answer for the window that just ended, and the eviction can only un-match it.
        private bool HasFullMatchBeforeEvicting(int endIndex, string text, int windowLength)
        {
            var isMatch = IsFullMatch;
            ReleaseLeaving(text[endIndex - windowLength + 1]);
            return isMatch;
        }

        private void ReleaseLeaving(char character)
        {
            if (!need.TryGetValue(character, out var needed))
            {
                return;
            }

            _window.TryGetValue(character, out var count);

            if (count == needed)
            {
                _matched--;
            }

            _window.Set(character, count - 1);
        }
    }
}
