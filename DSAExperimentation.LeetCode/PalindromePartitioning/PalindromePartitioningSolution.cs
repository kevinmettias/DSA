using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.PalindromePartitioning;

// LeetCode 131. Palindrome Partitioning: every way to cut `text` into a sequence
// of substrings that are all palindromes.
//
// Two strategies. The backtracking walk below asks Candidates for every palindromic
// prefix as it goes, re-testing each candidate substring from scratch. The table arm
// above builds an isPalindrome[i][j] table once, then runs the same
// choose/explore/unchoose walk against it, so each candidate question is O(1) instead
// of O(n). The old benchmark's two [Benchmark] arms were both compile-smoke
// placeholders (`=> 1`), not a second real approach to reconcile against.
internal static class PalindromePartitioningSolution
{
    // The same choose/explore/unchoose walk as the arm below, with one difference:
    // palindromicity is answered from a table built once up front rather than by
    // re-walking each candidate substring. Building the table costs O(n^2) time and
    // space before the search starts; in exchange every "is this prefix a palindrome?"
    // question drops from O(n) to O(1) - the trade the pair exists to compare.
    public static List<List<string>> PartitionByPrecomputedPalindromeTable(string text)
    {
        var isPalindrome = BuildPalindromeTable(text);
        var results = new List<List<string>>();
        var parts = new List<string>();

        PartitionFrom(0, text, isPalindrome, parts, results);

        return results;
    }

    // The walk itself: at each cursor, try every prefix the table marks palindromic, in
    // increasing length, and copy out the partition once the cursor reaches the end.
    private static void PartitionFrom(
        int start, string text, bool[,] isPalindrome, List<string> parts, List<List<string>> results)
    {
        if (start == text.Length)
        {
            results.Add([.. parts]);

            return;
        }

        for (var end = start; end < text.Length; end++)
        {
            if (!isPalindrome[start, end])
            {
                continue;
            }

            parts.Add(text[start..(end + 1)]);
            PartitionFrom(end + 1, text, isPalindrome, parts, results);
            parts.RemoveAt(parts.Count - 1);
        }
    }

    // The table in one backward sweep: a length-one span is always a palindrome, and a
    // longer span is one exactly when its two ends match and the span inside them does,
    // a row already filled by the time a row is reached.
    private static bool[,] BuildPalindromeTable(string text)
    {
        var length = text.Length;
        var isPalindrome = new bool[length, length];

        for (var start = length - 1; start >= 0; start--)
        {
            for (var end = start; end < length; end++)
            {
                if (text[start] == text[end] && (end - start <= 2 || isPalindrome[start + 1, end - 1]))
                {
                    isPalindrome[start, end] = true;
                }
            }
        }

        return isPalindrome;
    }

    public static List<List<string>> PartitionByBacktracking(string text)
    {
        var results = new List<List<string>>();
        var state = new PartitionState();

        Backtrack.Search<PartitionState, string>(
            state,
            x => x.Index == text.Length,
            x => x.Index == text.Length ? NoCandidates() : Candidates(text, x.Index),
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

    // No palindromic prefix is left once the cursor reaches the end of `text`.
    private static IEnumerable<string> NoCandidates() => [];

    private static IEnumerable<string> Candidates(string text, int start)
    {
        for (var end = start; end < text.Length; end++)
        {
            if (IsPalindrome(text, start, end))
            {
                yield return text[start..(end + 1)];
            }
        }
    }

    private static bool IsPalindrome(string text, int left, int right)
    {
        while (left < right)
        {
            if (text[left++] != text[right--])
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
