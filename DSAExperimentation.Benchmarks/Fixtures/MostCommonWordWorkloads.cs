namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 819: the tokens a paragraph splits into, each drawn
// from a pool of five-letter words ("worda", "wordb", ...) far smaller than the token
// count, so words repeat and a real most-common word emerges. Joined by single spaces
// the tokens make a paragraph of 6 * tokenCount - 1 characters, so 166 tokens is the
// most LC 819's 1,000-character paragraph holds. The pool's first bannedCount words
// are the banned list.
//
// LC 819 also guarantees the answer is unique, which random draws alone do not: after
// drawing, the most common allowed word takes the last occurrence of every allowed
// word tied with it, so it gains one per rival and leads alone.
internal static class MostCommonWordWorkloads
{
    private const string WordPoolPrefix = "word";

    public static (string[] Tokens, string[] Banned) Build(int tokenCount, int poolSize, int bannedCount, int seed)
    {
        var random = new Random(seed);
        var pool = Enumerable.Range(0, poolSize).Select(PoolWord).ToArray();
        var tokens = Enumerable.Range(0, tokenCount).Select(_ => pool[random.Next(poolSize)]).ToArray();

        PlantUniqueLeader(tokens, pool[bannedCount..]);

        return (tokens, pool[..bannedCount]);
    }

    // The pool's index-th word: the prefix and one letter, so the pool holds at most 26.
    private static string PoolWord(int index) => WordPoolPrefix + (char)('a' + index);

    private static void PlantUniqueLeader(string[] tokens, string[] allowed)
    {
        var counts = allowed.ToDictionary(word => word, word => tokens.Count(token => token == word));

        // A stable sort, so a tie for the top count goes to the earliest allowed word.
        var leader = allowed.OrderByDescending(word => counts[word]).First();
        var leaderCount = counts[leader];
        var rivals = allowed.Where(word => word != leader && counts[word] == leaderCount && leaderCount > 0);

        foreach (var rival in rivals)
        {
            var lastOccurrence = Array.LastIndexOf(tokens, rival);
            tokens[lastOccurrence] = leader;
        }
    }
}
