using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FindAllAnagramsInAString;

// LeetCode 438. Find All Anagrams in a String: every start index of a substring of s
// that is an anagram of p.
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
    public static List<int> FindAnagramIndicesByBruteForceRebuild(string s, string p)
    {
        var result = new List<int>();
        if (p.Length > s.Length)
        {
            return result;
        }

        var target = BuildFrequencyMap(p);

        for (var start = 0; start <= s.Length - p.Length; start++)
        {
            var window = BuildFrequencyMap(s.Substring(start, p.Length));
            if (FrequenciesEqual(window, target))
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

    private static bool FrequenciesEqual(Dictionary<char, int> window, Dictionary<char, int> target)
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
    public static List<int> FindAnagramIndicesBySlidingWindow(string s, string p)
    {
        var result = new List<int>();
        if (p.Length > s.Length)
        {
            return result;
        }

        var need = BuildRepoFrequencyMap(p);
        var state = new AnagramWindowState(need);

        for (var i = 0; i < s.Length; i++)
        {
            if (state.Advance(i, s, p.Length))
            {
                result.Add(i - p.Length + 1);
            }
        }

        return result;
    }

    private static HashMap<char, int> BuildRepoFrequencyMap(string p)
    {
        var need = new HashMap<char, int>();
        foreach (var c in p)
        {
            need.TryGetValue(c, out var count);
            need.Set(c, count + 1);
        }

        return need;
    }

    private sealed class AnagramWindowState(HashMap<char, int> need)
    {
        private readonly HashMap<char, int> _window = new();
        private int _matched;

        public bool IsFullMatch => _matched == need.Count;

        public bool Advance(int i, string s, int windowLength)
        {
            AbsorbEntering(s[i]);

            if (i < windowLength - 1)
            {
                return false;
            }

            var isMatch = IsFullMatch;
            ReleaseLeaving(s[i - windowLength + 1]);
            return isMatch;
        }

        private void AbsorbEntering(char c)
        {
            if (!need.TryGetValue(c, out var needed))
            {
                return;
            }

            _window.TryGetValue(c, out var count);
            _window.Set(c, count + 1);

            if (count + 1 == needed)
            {
                _matched++;
            }
        }

        private void ReleaseLeaving(char c)
        {
            if (!need.TryGetValue(c, out var needed))
            {
                return;
            }

            _window.TryGetValue(c, out var count);

            if (count == needed)
            {
                _matched--;
            }

            _window.Set(c, count - 1);
        }
    }
}
