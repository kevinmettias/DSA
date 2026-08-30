using RepoIntQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PeekingIterator;

// LeetCode 284. Peeking Iterator: this repo's own Queue<T> already exposes exactly
// the two operations the problem asks for - TryPeek (look without consuming) and
// TryDequeue (look and consume) - so there is no hand-rolled "have I already peeked"
// buffering state left to write.
public sealed partial class PeekingIteratorTests
{
    [Fact]
    public void PeekAndNext_LeetCodeExample_InterleavesCorrectly()
    {
        var iterator = new PeekingIteratorOperations([1, 2, 3]);

        Assert.Equal(1, iterator.Peek());
        Assert.Equal(1, iterator.Next());
        Assert.Equal(2, iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal(3, iterator.Peek());
        Assert.Equal(3, iterator.Next());
        Assert.False(iterator.HasNext());
    }

    [Fact]
    public void Peek_RepeatedCalls_DoNotAdvanceUnderlyingSequence()
    {
        var iterator = new PeekingIteratorOperations([5]);

        Assert.Equal(5, iterator.Peek());
        Assert.Equal(5, iterator.Peek());
        Assert.True(iterator.HasNext());
        Assert.Equal(5, iterator.Next());
        Assert.False(iterator.HasNext());
    }

    private sealed class PeekingIteratorOperations
    {
        private readonly RepoIntQueue _items = new();

        public PeekingIteratorOperations(IEnumerable<int> source)
        {
            foreach (var value in source)
            {
                _items.Enqueue(value);
            }
        }

        public bool HasNext() => _items.Count > 0;

        public int Peek()
        {
            _items.TryPeek(out var value);
            return value;
        }

        public int Next()
        {
            _items.TryDequeue(out var value);
            return value;
        }
    }
}
