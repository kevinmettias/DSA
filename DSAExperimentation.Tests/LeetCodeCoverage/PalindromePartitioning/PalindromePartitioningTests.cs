using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PalindromePartitioning;

public sealed partial class PalindromePartitioningTests
{
    [Fact]
    public void Partition_Aab_ReturnsBothPalindromePartitions()
    {
        var partitions = Partition("aab").Select(p => string.Join("|", p)).ToArray();
        Assert.Contains("a|a|b", partitions);
        Assert.Contains("aa|b", partitions);
    }

    private static List<List<string>> Partition(string s)
    {
        var results = new List<List<string>>(); var state = new State();
        Backtrack.Search<State, string>(state, x => x.Index == s.Length, x => x.Index == s.Length ? [] : Candidates(s, x.Index), (x, part) => { x.Starts.Push(x.Index); x.Index += part.Length; x.Parts.Add(part); }, (x, _) => { x.Index = x.Starts.Pop(); x.Parts.RemoveAt(x.Parts.Count - 1); }, x => results.Add([.. x.Parts]));
        return results;
    }
    private static IEnumerable<string> Candidates(string s, int start) { for (var end = start; end < s.Length; end++) if (IsPalindrome(s, start, end)) yield return s[start..(end + 1)]; }
    private static bool IsPalindrome(string s, int l, int r) { while (l < r) if (s[l++] != s[r--]) return false; return true; }
    private sealed class State { public int Index { get; set; } public Stack<int> Starts { get; } = new(); public List<string> Parts { get; } = []; }
}
