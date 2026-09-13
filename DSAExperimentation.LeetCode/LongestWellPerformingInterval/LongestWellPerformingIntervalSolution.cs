using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.LongestWellPerformingInterval;

// LeetCode 1124. Longest Well-Performing Interval: score each day +1 when
// hours[i] > 8 and -1 otherwise, then find the longest interval whose total score
// is positive. An interval (i, j] qualifies iff prefixScore[j] > prefixScore[i],
// so the longest one either starts at index 0 (whenever the running score is
// already positive) or ends where the running score last matched EXACTLY one less
// than the current score - the earliest such index, since a later match of the same
// score value could only shorten the span.
internal static class LongestWellPerformingIntervalSolution
{
    // LeetCode's definition of a tiring day: strictly more than 8 hours worked.
    private const int TiringThreshold = 8;

    // The textbook answer: every start/end pair, re-summing the +1/-1 score as the
    // interval extends - O(n^2), deliberately without this repo's primitives. The
    // arm the prefix-score strategy below has to justify itself against.
    public static int LongestWpiByBruteForce(int[] hours)
    {
        var longest = 0;

        for (var start = 0; start < hours.Length; start++)
        {
            longest = Math.Max(longest, LongestIntervalFrom(hours, start));
        }

        return longest;
    }

    // The longest positive-score interval that begins at start, which is simply the
    // last end index at which the running score is still positive.
    private static int LongestIntervalFrom(int[] hours, int start)
    {
        var score = 0;
        var longest = 0;

        for (var end = start; end < hours.Length; end++)
        {
            score += Tiring(hours[end]);

            if (score > 0)
            {
                longest = end - start + 1;
            }
        }

        return longest;
    }

    // This repo's own HashMap<int,int> (running score -> earliest index it was first
    // seen at) - the exact "first occurrence index" shape
    // ContinuousSubarraySumSolution uses for "% K == 0", with the target relation
    // "> 0" instead. One pass, O(n).
    public static int LongestWpiByPrefixScoreMap(int[] hours)
    {
        var firstIndexByScore = new HashMap<int, int>();
        var tracker = new ScoreTracker();

        for (var i = 0; i < hours.Length; i++)
        {
            tracker.ProcessDay(i, hours[i], firstIndexByScore);
        }

        return tracker.Longest;
    }

    private static int Tiring(int hour) => hour > TiringThreshold ? 1 : -1;

    // The running score and the best interval seen so far, advanced one day at a
    // time against the first-occurrence map.
    private sealed class ScoreTracker
    {
        private int _score;

        public int Longest { get; private set; }

        public void ProcessDay(int i, int hour, HashMap<int, int> firstIndexByScore)
        {
            _score += Tiring(hour);

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
