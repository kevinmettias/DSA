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

        FillEmptySuffixBorders(dp, word1.Length, word2.Length);

        for (var i = word1.Length - 1; i >= 0; i--)
        {
            for (var j = word2.Length - 1; j >= 0; j--)
            {
                var insertOrReplace = Math.Min(dp[i, j + 1], dp[i + 1, j + 1]);
                var lettersMatch = word1[i] == word2[j];
                dp[i, j] = lettersMatch
                    ? DiagonalCost(dp, i, j)
                    : CostAfterOneEdit(dp, i, j, insertOrReplace);
            }
        }

        return dp[0, 0];
    }

    // The two empty-suffix borders: turning the whole of one word into the empty
    // suffix of the other costs one delete per character still standing.
    private static void FillEmptySuffixBorders(int[,] dp, int firstLength, int secondLength)
    {
        for (var i = 0; i <= firstLength; i++)
        {
            dp[i, secondLength] = firstLength - i;
        }

        for (var j = 0; j <= secondLength; j++)
        {
            dp[firstLength, j] = secondLength - j;
        }
    }

    // The diagonal entry: both suffixes advanced by one, the free match-through.
    private static int DiagonalCost(int[,] dp, int word1Index, int word2Index) =>
        dp[word1Index + 1, word2Index + 1];

    // One edit charged, plus the cheaper of the remaining delete and the insert-or-
    // replace the caller has already minimized in.
    private static int CostAfterOneEdit(int[,] dp, int word1Index, int word2Index, int insertOrReplace) =>
        1 + Math.Min(dp[word1Index + 1, word2Index], insertOrReplace);

    // Memoizer caches the same recurrence, called top-down from (0, 0) instead of
    // filled bottom-up, so only the suffix pairs the walk actually visits get
    // computed.
    public static int MinDistanceByMemoizedRecurrence(string word1, string word2) =>
        Memoizer.Memoize<(int First, int Second), int>((0, 0), new EditsFromSuffixPair(word1, word2));

    // The recurrence, as a named type: either suffix exhausted costs one delete per
    // character still standing on the other side, next characters that agree are free
    // to match through, and otherwise one edit is charged on top of the cheapest of
    // the insert, delete and replace moves left.
    private sealed class EditsFromSuffixPair(string word1, string word2)
        : IRecurrence<(int First, int Second), int>
    {
        public int Replay((int First, int Second) state, IRecurrence<(int First, int Second), int> rest)
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
                return rest.Replay((i + 1, j + 1), rest);
            }

            return OneEditPlusCheapestMove(i, j, rest);
        }

        // One edit charged, plus the cheaper of the remaining delete and the
        // insert-or-replace - the same charge the tabulation arm's CostAfterOneEdit
        // makes, with the three suffixes it chooses between reached through the memo.
        private static int OneEditPlusCheapestMove(
            int word1Index, int word2Index, IRecurrence<(int First, int Second), int> rest)
        {
            var insert = rest.Replay((word1Index, word2Index + 1), rest);
            var replace = rest.Replay((word1Index + 1, word2Index + 1), rest);
            var delete = rest.Replay((word1Index + 1, word2Index), rest);
            var insertOrReplace = Math.Min(insert, replace);

            return 1 + Math.Min(delete, insertOrReplace);
        }
    }
}
