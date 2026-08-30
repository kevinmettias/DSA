using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestWellPerformingInterval;

// LeetCode 1124. Longest Well-Performing Interval: a running "tiring score" (+1 for
// hours[i] > 8, -1 otherwise) turns this into the same prefix-sum + HashMap
// first-occurrence-index pattern ContinuousSubarraySumTests.cs already establishes
// for "%K == 0" - here the target relation is "> 0" instead of "== 0". A
// well-performing interval (i, j] has positive total score iff
// prefixScore[j] > prefixScore[i], so the longest one either starts at index 0 (when
// the running score is already positive) or ends where the running score last matched
// EXACTLY one less than the current score - the earliest such index, since any later
// match of that same score value could only shorten the span.
public sealed class LongestWellPerformingIntervalTests
{
    [Fact]
    public void LongestWpi_ClassicExample_ReturnsThree()
    {
        int[] hours = [9, 9, 6, 0, 6, 6, 9];

        Assert.Equal(3, LongestWpi(hours));
    }

    [Fact]
    public void LongestWpi_AllTiringDays_ReturnsFullLength()
    {
        int[] hours = [9, 9, 9];

        Assert.Equal(3, LongestWpi(hours));
    }

    [Fact]
    public void LongestWpi_NeverMoreTiringThanNot_ReturnsZero()
    {
        int[] hours = [6, 6, 6];

        Assert.Equal(0, LongestWpi(hours));
    }

    private static int LongestWpi(int[] hours)
    {
        var firstIndexByScore = new HashMap<int, int>();
        var score = 0;
        var longest = 0;

        for (var i = 0; i < hours.Length; i++)
        {
            score += hours[i] > 8 ? 1 : -1;

            if (score > 0)
            {
                longest = i + 1;
                continue;
            }

            if (firstIndexByScore.TryGetValue(score - 1, out var priorIndex))
            {
                longest = Math.Max(longest, i - priorIndex);
            }

            if (!firstIndexByScore.HasKey(score))
            {
                firstIndexByScore.Set(score, i);
            }
        }

        return longest;
    }
}
