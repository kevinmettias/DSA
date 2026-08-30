using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Count, int Value)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RLEIterator;

// LeetCode 900. RLE Iterator: the (count, value) run pairs are only ever consumed in
// order, front to back, so they compose this repo's own FIFO Queue<T> directly - the
// same "queue holds a struct payload" move ImplementStackUsingQueuesTests already
// makes with Queue<int>. A partially-consumed run is tracked as instance state
// instead, since Queue<T> deliberately offers no push-to-front (MinStack/DailyTemperatures'
// Stack<T> precedent: this repo's collection types expose exactly the access pattern
// their name promises, nothing more).
public sealed partial class RLEIteratorTests
{
    [Fact]
    public void Next_LeetCodeExample_ReturnsExpectedSequence()
    {
        var iterator = new RLEIteratorOperations([3, 8, 0, 9, 2, 5]);

        Assert.Equal(8, iterator.Next(2));
        Assert.Equal(8, iterator.Next(1));
        Assert.Equal(5, iterator.Next(1));
        Assert.Equal(-1, iterator.Next(2));
    }

    [Fact]
    public void Next_RequestMoreThanEncodingHolds_ReturnsNegativeOne()
    {
        var iterator = new RLEIteratorOperations([1, 4]);

        Assert.Equal(-1, iterator.Next(5));
    }

    private sealed class RLEIteratorOperations
    {
        private readonly RepoQueue _runs = new();
        private int _remaining;
        private int _value;

        public RLEIteratorOperations(int[] encoding)
        {
            for (var i = 0; i < encoding.Length; i += 2)
            {
                _runs.Enqueue((encoding[i], encoding[i + 1]));
            }
        }

        public int Next(int n)
        {
            while (n > 0)
            {
                if (_remaining == 0)
                {
                    if (!_runs.TryDequeue(out var run))
                    {
                        return -1;
                    }

                    _remaining = run.Count;
                    _value = run.Value;
                }

                var consumed = Math.Min(n, _remaining);
                _remaining -= consumed;
                n -= consumed;
            }

            return _value;
        }
    }
}
