using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FrequencyTracker;

// LeetCode 2671. Frequency Tracker: two of this repo's own HashMap<TKey,TValue>
// instances - one tracking each number's current count, one tracking how many
// distinct numbers currently sit at each frequency - the classic "frequency of
// frequencies" trick. Add/DeleteOne move a number's count by one and mirror that
// move into the frequency map (decrement the old frequency's bucket, increment the
// new one); HasFrequency is then just a presence check on the second map.
public sealed class FrequencyTrackerTests
{
    [Fact]
    public void AddThenHasFrequency_LeetCodeExampleOne_MatchesExpectedResults()
    {
        var tracker = new FrequencyTrackerOperations();

        tracker.Add(3);
        tracker.Add(3);
        Assert.True(tracker.HasFrequency(2));

        tracker.Add(4);
        Assert.True(tracker.HasFrequency(1));
    }

    [Fact]
    public void DeleteOne_NumberNeverAdded_IsANoOpAndReportsNoZeroFrequency()
    {
        var tracker = new FrequencyTrackerOperations();

        tracker.DeleteOne(0);

        Assert.False(tracker.HasFrequency(0));
    }

    [Fact]
    public void DeleteOne_DropsBackToAPreviousFrequency_UpdatesBothFrequencyBuckets()
    {
        var tracker = new FrequencyTrackerOperations();

        tracker.Add(5);
        tracker.Add(5);
        Assert.True(tracker.HasFrequency(2));

        tracker.DeleteOne(5);

        Assert.True(tracker.HasFrequency(1));
        Assert.False(tracker.HasFrequency(2));
    }

    private sealed class FrequencyTrackerOperations
    {
        private readonly HashMap<int, int> _countByNumber = new();
        private readonly HashMap<int, int> _countByFrequency = new();

        public void Add(int number)
        {
            _countByNumber.TryGetValue(number, out var oldCount);
            DecrementFrequencyBucket(oldCount);

            var newCount = oldCount + 1;
            _countByNumber.Set(number, newCount);
            IncrementFrequencyBucket(newCount);
        }

        public void DeleteOne(int number)
        {
            if (!_countByNumber.TryGetValue(number, out var oldCount) || oldCount == 0)
            {
                return;
            }

            DecrementFrequencyBucket(oldCount);

            var newCount = oldCount - 1;
            if (newCount == 0)
            {
                _countByNumber.TryRemove(number);
            }
            else
            {
                _countByNumber.Set(number, newCount);
                IncrementFrequencyBucket(newCount);
            }
        }

        public bool HasFrequency(int frequency)
            => _countByFrequency.TryGetValue(frequency, out var count) && count > 0;

        private void IncrementFrequencyBucket(int frequency)
        {
            _countByFrequency.TryGetValue(frequency, out var count);
            _countByFrequency.Set(frequency, count + 1);
        }

        private void DecrementFrequencyBucket(int frequency)
        {
            if (frequency == 0)
            {
                return;
            }

            if (_countByFrequency.TryGetValue(frequency, out var count) && count > 0)
            {
                _countByFrequency.Set(frequency, count - 1);
            }
        }
    }
}
