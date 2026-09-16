using DSAExperimentation.DataStructures.AhoCorasick;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ConstructStringWithMinimumCost;

// LeetCode 3213. Construct String with Minimum Cost: build target by repeatedly
// appending some words[i] at cost costs[i]; return the cheapest total cost, or
// -1 if target can't be built at all.
//
// Both strategies are the same DP - dp[i] is the min cost to build target's
// first i characters, dp[0] = 0, and a word matching target at [i, i+len) offers
// dp[i+len] = min(dp[i+len], dp[i] + cost] - and differ only in how they find
// every (position, matching word) pair the DP transitions over.
internal static class ConstructStringWithMinimumCostSolution
{
    private const int Unreached = int.MaxValue;

    // The textbook version: at every position, try every word by direct
    // character comparison. O(target.Length * words.Length * word.Length) -
    // the arm the automaton strategy below has to beat.
    public static int MinCostByBruteForceDp(string target, string[] words, int[] costs)
    {
        var dp = new int[target.Length + 1];
        Array.Fill(dp, Unreached);
        dp[0] = 0;

        for (var i = 0; i < target.Length; i++)
        {
            if (dp[i] == Unreached)
            {
                continue;
            }

            for (var w = 0; w < words.Length; w++)
            {
                RelaxWithWordAt(dp, target, i, (words[w], costs[w]));
            }
        }

        return IsTargetUnreachable(dp, target) ? LeetCodeAnswer.None : CostToBuildTarget(dp);
    }

    // One word offered at one position: skip it when it overruns the target or does not
    // match there, otherwise let the cost already paid for `start` plus this word's own
    // cost relax the slot the word would end at.
    private static void RelaxWithWordAt(int[] dp, string target, int start, (string Word, int Cost) candidate)
    {
        var end = start + candidate.Word.Length;

        if (end > target.Length || !IsMatchAt(target, start, candidate.Word))
        {
            return;
        }

        var total = dp[start] + candidate.Cost;

        if (total < dp[end])
        {
            dp[end] = total;
        }
    }

    // dp[i] is the min cost to build target's first i characters, so the slot
    // for the whole target is the only one a finished answer is read from, and
    // Unreached there is the "cannot be built at all" sentinel.
    private static bool IsTargetUnreachable(int[] dp, string target) => dp[target.Length] == Unreached;

    private static int CostToBuildTarget(int[] dp) => dp[^1];

    private static bool IsMatchAt(string target, int start, string word)
    {
        for (var offset = 0; offset < word.Length; offset++)
        {
            if (target[start + offset] != word[offset])
            {
                return false;
            }
        }

        return true;
    }

    // Every (position, matching word) pair the DP above needs is exactly what
    // this repo's own AhoCorasick.FindAll already reports for a multi-pattern
    // scan, so building the automaton once from the distinct words (cheapest
    // cost wins on a duplicate) turns the whole DP into one pass over its
    // matches, sorted by Start - which is also the order the DP needs them in,
    // since a match can only ever feed a LATER dp slot than the one it reads.
    public static int MinCostByAhoCorasickDp(string target, string[] words, int[] costs)
    {
        var minCostByWord = new HashMap<string, int>();

        for (var w = 0; w < words.Length; w++)
        {
            if (!minCostByWord.TryGetValue(words[w], out var existingCost) || costs[w] < existingCost)
            {
                minCostByWord.Set(words[w], costs[w]);
            }
        }

        var uniqueWords = new List<string>(minCostByWord.Keys);
        var uniqueCosts = new int[uniqueWords.Count];

        for (var w = 0; w < uniqueWords.Count; w++)
        {
            minCostByWord.TryGetValue(uniqueWords[w], out uniqueCosts[w]);
        }

        var automaton = new AhoCorasick(uniqueWords);

        return MinCostByAhoCorasickDp(target, automaton, uniqueWords, uniqueCosts);
    }

    public static int MinCostByAhoCorasickDp(
        string target, AhoCorasick automaton, IReadOnlyList<string> uniqueWords, int[] uniqueCosts)
    {
        var dp = new int[target.Length + 1];
        Array.Fill(dp, Unreached);
        dp[0] = 0;

        foreach (var match in automaton.FindAll(target))
        {
            if (dp[match.Start] == Unreached)
            {
                continue;
            }

            var end = match.Start + uniqueWords[match.PatternIndex].Length;
            var candidate = dp[match.Start] + uniqueCosts[match.PatternIndex];

            if (candidate < dp[end])
            {
                dp[end] = candidate;
            }
        }

        return IsTargetUnreachable(dp, target) ? LeetCodeAnswer.None : CostToBuildTarget(dp);
    }
}
