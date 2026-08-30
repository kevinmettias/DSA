using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignSkiplist;

// LeetCode 1206. Design Skiplist: add/erase/search only ever need "how many of this
// exact value are currently stored," never an ordered walk between values - so this
// repo's own FenwickTree<int,SumOperation<int>>, used as a point-update/point-query
// frequency array over num's bounded domain (0 <= num <= 2*10^4 per LeetCode's own
// constraint), answers every call in O(log 2*10^4) with no probabilistic multi-level
// list needed. Add is FenwickTree's own native +1 delta; Search/Erase both read the
// current count via a single-index Query before Erase turns it into a -1 delta.
public sealed partial class DesignSkiplistTests
{
    [Fact]
    public void Skiplist_LeetCodeExample_MatchesExpectedCallSequence()
    {
        var skiplist = new SkiplistOperations();

        skiplist.Add(1);
        skiplist.Add(2);
        skiplist.Add(3);

        Assert.False(skiplist.Search(0));

        skiplist.Add(4);

        Assert.True(skiplist.Search(1));
        Assert.False(skiplist.Erase(0));
        Assert.True(skiplist.Erase(1));
        Assert.False(skiplist.Search(1));
    }

    [Fact]
    public void Skiplist_DuplicateValues_EraseRemovesOnlyOneOccurrenceAtATime()
    {
        var skiplist = new SkiplistOperations();

        skiplist.Add(5);
        skiplist.Add(5);

        Assert.True(skiplist.Erase(5));
        Assert.True(skiplist.Search(5));

        Assert.True(skiplist.Erase(5));
        Assert.False(skiplist.Search(5));
        Assert.False(skiplist.Erase(5));
    }

    private sealed class SkiplistOperations
    {
        private const int MaxValue = 20_000;

        private readonly FenwickTree<int, SumOperation<int>> _frequencies = new(MaxValue + 1);

        public bool Search(int target) => _frequencies.Query(target, target) > 0;

        public void Add(int num) => _frequencies.Add(num, 1);

        public bool Erase(int num)
        {
            if (!Search(num))
            {
                return false;
            }

            _frequencies.Add(num, -1);
            return true;
        }
    }
}
