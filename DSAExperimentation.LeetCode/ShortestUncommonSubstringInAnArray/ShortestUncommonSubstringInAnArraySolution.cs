using DSAExperimentation.DataStructures.AhoCorasick;

namespace DSAExperimentation.LeetCode.ShortestUncommonSubstringInAnArray;

// LeetCode 3076. Shortest Uncommon Substring in an Array: for each word, the
// shortest (then lexicographically smallest) substring that occurs in that word
// and nowhere else in the array - or "" if every one of its substrings occurs
// elsewhere too. Both strategies rank the same candidate list (every distinct
// substring of the word, ordered shortest-first then lexicographically) and
// differ only in how they check "does this candidate occur in some other word".
internal static class ShortestUncommonSubstringInAnArraySolution
{
    // The textbook check: BCL string.Contains against every other word, one
    // candidate at a time. The arm the Aho-Corasick strategy has to beat.
    public static string[] FindShortestUncommonSubstringsByBruteForce(string[] arr)
    {
        var answer = new string[arr.Length];

        for (var i = 0; i < arr.Length; i++)
        {
            answer[i] = ShortestByBruteForce(arr, i);
        }

        return answer;
    }

    private static string ShortestByBruteForce(string[] arr, int index)
    {
        foreach (var candidate in CandidateSubstrings(arr[index]))
        {
            var occursElsewhere = false;

            for (var j = 0; j < arr.Length && !occursElsewhere; j++)
            {
                occursElsewhere = j != index && arr[j].Contains(candidate, StringComparison.Ordinal);
            }

            if (!occursElsewhere)
            {
                return candidate;
            }
        }

        return string.Empty;
    }

    // Composed: build one AhoCorasick automaton (DataStructures/AhoCorasick/
    // AhoCorasick.cs) per word, whose patterns are that word's own candidate
    // substrings, then run every OTHER word through FindAll once. FindAll
    // reports every pattern that occurs anywhere in the scanned text in
    // O(text.Length + matches found), so one pass per other word marks every
    // candidate that fails - versus one Contains scan per (candidate, other
    // word) pair above.
    public static string[] FindShortestUncommonSubstringsByAhoCorasick(string[] arr)
    {
        var answer = new string[arr.Length];

        for (var i = 0; i < arr.Length; i++)
        {
            answer[i] = ShortestByAhoCorasick(arr, i);
        }

        return answer;
    }

    private static string ShortestByAhoCorasick(string[] arr, int index)
    {
        var candidates = CandidateSubstrings(arr[index]);

        if (candidates.Count == 0)
        {
            return string.Empty;
        }

        var automaton = new AhoCorasick(candidates);
        var occursElsewhere = MarkCandidatesFoundElsewhere(automaton, arr, index, candidates.Count);

        return FirstUnmarkedCandidate(candidates, occursElsewhere);
    }

    // One FindAll pass per other word: it reports every pattern occurring
    // anywhere in that word, so each match marks its own candidate as failing.
    private static bool[] MarkCandidatesFoundElsewhere(
        AhoCorasick automaton, string[] arr, int index, int candidateCount)
    {
        var occursElsewhere = new bool[candidateCount];

        for (var j = 0; j < arr.Length; j++)
        {
            if (j == index)
            {
                continue;
            }

            foreach (var match in automaton.FindAll(arr[j]))
            {
                occursElsewhere[match.PatternIndex] = true;
            }
        }

        return occursElsewhere;
    }

    // Candidates are already ranked shortest-first then lexicographically, so the
    // first one no other word contains is the answer.
    private static string FirstUnmarkedCandidate(List<string> candidates, bool[] occursElsewhere)
    {
        for (var c = 0; c < candidates.Count; c++)
        {
            if (!occursElsewhere[c])
            {
                return candidates[c];
            }
        }

        return string.Empty;
    }

    // Every distinct substring of word, ordered shortest-first and
    // lexicographically smallest within a length - the order both strategies
    // return the first surviving candidate from.
    private static List<string> CandidateSubstrings(string word)
    {
        var substrings = new HashSet<string>();

        for (var start = 0; start < word.Length; start++)
        {
            for (var length = 1; start + length <= word.Length; length++)
            {
                var substring = word.Substring(start, length);
                substrings.Add(substring);
            }
        }

        return substrings
            .OrderBy(candidate => candidate.Length)
            .ThenBy(candidate => candidate, StringComparer.Ordinal)
            .ToList();
    }
}
