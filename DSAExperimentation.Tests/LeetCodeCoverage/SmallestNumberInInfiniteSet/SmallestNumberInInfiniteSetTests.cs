using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestNumberInInfiniteSet;

// LeetCode 2336. Smallest Number in Infinite Set: the infinite set [1, 2, 3, ...] is represented
// implicitly via a single counter (_nextUnused) instead of materialized, so only numbers pulled
// back in via AddBack ever need real storage - exactly the "added-back" subset this repo's own
// Heap<Element,MinHeapOrder<Element>> is built for (peek/pop the smallest of a dynamic collection
// in O(log n)), paired with Set<Element> purely to reject a duplicate AddBack in O(1) instead of
// letting the same value sit in the heap twice. PopSmallest always prefers the heap: every value
// ever pushed there was already < _nextUnused at push time, so it can never exceed the next
// never-yet-produced number - no comparison between the two sources is ever needed.
public sealed partial class SmallestNumberInInfiniteSetTests
{
    [Fact]
    public void PopSmallestAndAddBack_ClassicExample_MatchesLeetCodeTrace()
    {
        var set = new SmallestInfiniteSet();

        set.AddBack(2); // 2 was never popped, so it's already "in" the infinite set - no-op

        Assert.Equal(1, set.PopSmallest());
        Assert.Equal(2, set.PopSmallest());
        Assert.Equal(3, set.PopSmallest());

        set.AddBack(1);

        Assert.Equal(1, set.PopSmallest());
        Assert.Equal(4, set.PopSmallest());
        Assert.Equal(5, set.PopSmallest());
    }

    [Fact]
    public void AddBack_DuplicateOfAlreadyPendingNumber_IsIgnored()
    {
        var set = new SmallestInfiniteSet();

        set.PopSmallest(); // 1
        set.PopSmallest(); // 2
        set.AddBack(1);
        set.AddBack(1); // duplicate - must not double-queue 1

        Assert.Equal(1, set.PopSmallest());
        Assert.Equal(3, set.PopSmallest());
    }

    [Fact]
    public void AddBack_NumberNotYetProduced_IsIgnoredAsAlreadyInSet()
    {
        var set = new SmallestInfiniteSet();

        set.AddBack(5); // never popped yet, so already present

        Assert.Equal(1, set.PopSmallest());
        Assert.Equal(2, set.PopSmallest());
        Assert.Equal(3, set.PopSmallest());
        Assert.Equal(4, set.PopSmallest());
        Assert.Equal(5, set.PopSmallest());
    }

    private sealed class SmallestInfiniteSet
    {
        private readonly Heap<int, MinHeapOrder<int>> _addedBack = new();
        private readonly Set<int> _pending = new();
        private int _nextUnused = 1;

        public int PopSmallest()
        {
            if (_addedBack.TryPop(out var restored))
            {
                _pending.TryRemove(restored);
                return restored;
            }

            return _nextUnused++;
        }

        public void AddBack(int num)
        {
            if (num >= _nextUnused || !_pending.TryAdd(num))
            {
                return;
            }

            _addedBack.Push(num);
        }
    }
}
