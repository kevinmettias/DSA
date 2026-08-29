using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EditDistance;

// LeetCode 72. Edit Distance: Memoizer supplies the cache for the classic
// suffix-pair recurrence over insert/delete/replace choices.
public sealed partial class EditDistanceTests
{
    [Theory]
    [InlineData("horse", "ros", 3)]
    [InlineData("intention", "execution", 5)]
    public void MinDistance_LeetCodeExamples_ReturnsEditDistance(string word1, string word2, int expected)
        => Assert.Equal(expected, MinDistance(word1, word2));

    private static int MinDistance(string word1, string word2)
    {
        return Memoizer.Memoize<(int First, int Second), int>((0, 0), DistanceFrom);

        int DistanceFrom((int First, int Second) state, Func<(int First, int Second), int> distance)
        {
            var (i, j) = state;
            if (i == word1.Length) return word2.Length - j;
            if (j == word2.Length) return word1.Length - i;
            if (word1[i] == word2[j]) return distance((i + 1, j + 1));
            return 1 + Math.Min(distance((i + 1, j)), Math.Min(distance((i, j + 1)), distance((i + 1, j + 1))));
        }
    }
}
