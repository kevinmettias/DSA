using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.PalindromePartitioning;

// LeetCode 131. Palindrome Partitioning: every way to cut s into a sequence of
// substrings that are all palindromes.
//
// Candidates(s, start) offers every palindromic prefix beginning at start, in
// increasing length; Backtrack.Search's choose/explore/unchoose walk tries each in
// turn, and OnSolution copies out the current partition whenever the cursor reaches
// the end of s. Only one strategy exists here - the original benchmark's two
// [Benchmark] arms were both compile-smoke placeholders (`=> 1`), not a second real
// approach to reconcile against.
internal static class PalindromePartitioningSolution
{
    public static List<List<string>> PartitionByBacktracking(string s)
    {
        var results = new List<List<string>>();
        var state = new PartitionState();

        Backtrack.Search<PartitionState, string>(
            state,
            x => x.Index == s.Length,
            x => x.Index == s.Length ? [] : Candidates(s, x.Index),
            (x, part) =>
            {
                x.Starts.Push(x.Index);
                x.Index += part.Length;
                x.Parts.Add(part);
            },
            (x, _) =>
            {
                x.Index = x.Starts.Pop();
                x.Parts.RemoveAt(x.Parts.Count - 1);
            },
            x => results.Add([.. x.Parts]));

        return results;
    }

    private static IEnumerable<string> Candidates(string s, int start)
    {
        for (var end = start; end < s.Length; end++)
        {
            if (IsPalindrome(s, start, end))
            {
                yield return s[start..(end + 1)];
            }
        }
    }

    private static bool IsPalindrome(string s, int left, int right)
    {
        while (left < right)
        {
            if (s[left++] != s[right--])
            {
                return false;
            }
        }

        return true;
    }

    private sealed class PartitionState
    {
        public int Index { get; set; }

        public Stack<int> Starts { get; } = new();

        public List<string> Parts { get; } = [];
    }
}
