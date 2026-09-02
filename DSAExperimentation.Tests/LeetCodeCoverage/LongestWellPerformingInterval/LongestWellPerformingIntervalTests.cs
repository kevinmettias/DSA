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
        var tracker = new ScoreTracker();

        for (var i = 0; i < hours.Length; i++)
        {
            tracker.ProcessDay(i, hours[i], firstIndexByScore);
        }

        return tracker.Longest;
    }

    private sealed class ScoreTracker
    {
        private int _score;

        public int Longest { get; private set; }

        public void ProcessDay(int i, int hour, HashMap<int, int> firstIndexByScore)
        {
            _score += hour > 8 ? 1 : -1;

            if (_score > 0)
            {
                Longest = i + 1;
                return;
            }

            if (firstIndexByScore.TryGetValue(_score - 1, out var priorIndex))
            {
                Longest = Math.Max(Longest, i - priorIndex);
            }

            if (!firstIndexByScore.HasKey(_score))
            {
                firstIndexByScore.Set(_score, i);
            }
        }
    }
}
