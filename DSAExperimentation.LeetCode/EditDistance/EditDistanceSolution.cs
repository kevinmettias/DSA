using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.EditDistance;

// LeetCode 72. Edit Distance: fewest insert/delete/replace operations to turn
// word1 into word2. Both strategies compute the same suffix-pair recurrence -
// distance(i, j) is 0 once both suffixes are exhausted, and otherwise either the
// free match-through when the next characters agree or 1 plus the best of
// insert/delete/replace on the remaining suffixes.
internal static class EditDistanceSolution
{
    // Textbook baseline: BCL 2D array filled bottom-up from the empty-suffix
    // border, walking both suffixes right-to-left so dp[i, j] only ever reads
    // already-filled entries.
    public static int MinDistanceByTabulation(string word1, string word2)
    {
        var dp = new int[word1.Length + 1, word2.Length + 1];

        for (var i = 0; i <= word1.Length; i++)
        {
            dp[i, word2.Length] = word1.Length - i;
        }

        for (var j = 0; j <= word2.Length; j++)
        {
            dp[word1.Length, j] = word2.Length - j;
        }

        for (var i = word1.Length - 1; i >= 0; i--)
        {
            for (var j = word2.Length - 1; j >= 0; j--)
            {
                var insertOrReplace = Math.Min(dp[i, j + 1], dp[i + 1, j + 1]);
                dp[i, j] = word1[i] == word2[j]
                    ? dp[i + 1, j + 1]
                    : 1 + Math.Min(dp[i + 1, j], insertOrReplace);
            }
        }

        return dp[0, 0];
    }

    // Memoizer caches the same recurrence, called top-down from (0, 0) instead of
    // filled bottom-up, so only the suffix pairs the walk actually visits get
    // computed.
    public static int MinDistanceByMemoizedRecurrence(string word1, string word2)
    {
        return Memoizer.Memoize<(int First, int Second), int>((0, 0), DistanceFrom);

        int DistanceFrom((int First, int Second) state, Func<(int First, int Second), int> distance)
        {
            var (i, j) = state;

            if (i == word1.Length)
            {
                return word2.Length - j;
            }

            if (j == word2.Length)
            {
                return word1.Length - i;
            }

            if (word1[i] == word2[j])
            {
                return distance((i + 1, j + 1));
            }

            var insertOrReplace = Math.Min(distance((i, j + 1)), distance((i + 1, j + 1)));
            return 1 + Math.Min(distance((i + 1, j)), insertOrReplace);
        }
    }
}
