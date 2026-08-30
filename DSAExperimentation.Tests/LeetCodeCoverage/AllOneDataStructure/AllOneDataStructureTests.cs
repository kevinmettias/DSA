using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllOneDataStructure;

// LeetCode 432. All O`one Data Structure: an ascending doubly linked list of count "buckets" -
// the same frequency-bucket idea LfuCache.cs already uses for O(1) eviction, generalized from
// "insert only at the front" to "insert next to whichever neighbor bucket the moving key
// should land beside" via DoublyLinkedListNode's own public Previous/Next (its own doc comment
// already anticipates a composer splicing directly - the same way LruCache/LfuCache splice
// indirectly through DoublyLinkedList<T>). getMaxKey/getMinKey are then just tail.Previous/
// head.Next - O(1) even across gaps in the count sequence, which a bare incremented/decremented
// int bound (LfuCache's own _minFrequency trick) cannot guarantee once a key's count can both
// increase AND decrease.
//
// Each bucket's own key membership is a SECOND, independent DoublyLinkedListNode chain, not a
// HashMap<string,bool> - HashMap<TKey,TValue>.Keys is documented as an eager List snapshot of
// every entry, so "grab any one key" would silently cost O(bucket size) per call instead of
// O(1), defeating the whole point. A composer needing indexed lookup AND O(1) grab-the-first-
// entry has to hold that entry itself, which is exactly what DoublyLinkedListNode's own public
// Previous/Next are for.
public sealed partial class AllOneDataStructureTests
{
    [Fact]
    public void GetMaxKeyAndGetMinKey_ClassicExample_ReturnsExpectedKeys()
    {
        var allOne = new AllOne();

        allOne.Inc("hello");
        allOne.Inc("hello");
        allOne.Inc("leet");

        Assert.Equal("hello", allOne.GetMaxKey());
        Assert.Equal("leet", allOne.GetMinKey());
    }

    [Fact]
    public void GetMinKey_EmptyStructure_ReturnsEmptyString()
    {
        var allOne = new AllOne();

        Assert.Equal("", allOne.GetMinKey());
        Assert.Equal("", allOne.GetMaxKey());
    }

    [Fact]
    public void Dec_RemovesKeyEntirely_WhenCountReachesZero()
    {
        var allOne = new AllOne();
        allOne.Inc("a");

        allOne.Dec("a");

        Assert.Equal("", allOne.GetMaxKey());
    }

    [Fact]
    public void GetMinKey_AfterFullyRemovingSoleMinimumKey_JumpsAcrossGapToNextRealBucket()
    {
        // "a" is pushed up to count 4 first, so no bucket ever exists at counts 2 or 3; "c"
        // is then inserted (count 1) and fully removed again, leaving only "a" at count 4. A
        // min-tracking scheme that just assumes "the new minimum is oldMinimum + 1" (as
        // LfuCache's own _minFrequency safely can, since frequency there only ever increases)
        // would wrongly look for a bucket at count 2, which was never created.
        var allOne = new AllOne();
        allOne.Inc("a");
        allOne.Inc("a");
        allOne.Inc("a");
        allOne.Inc("a");

        allOne.Inc("c");
        allOne.Dec("c");

        Assert.Equal("a", allOne.GetMinKey());
        Assert.Equal("a", allOne.GetMaxKey());
    }

    [Fact]
    public void Inc_SameKeyRepeatedly_KeepsSingleEntryVisibleAsBothMaxAndMin()
    {
        var allOne = new AllOne();

        for (var i = 0; i < 5; i++)
        {
            allOne.Inc("only");
        }

        Assert.Equal("only", allOne.GetMaxKey());
        Assert.Equal("only", allOne.GetMinKey());
    }

    private sealed class KeyEntry(string key)
    {
        public string Key { get; } = key;

        public DoublyLinkedListNode<Bucket>? CurrentBucket { get; set; }
    }

    private sealed class Bucket
    {
        private readonly DoublyLinkedListNode<KeyEntry> _keysHead = new();
        private readonly DoublyLinkedListNode<KeyEntry> _keysTail = new();

        public int Count { get; }

        public int KeyCount { get; private set; }

        public Bucket(int count)
        {
            Count = count;
            _keysHead.Next = _keysTail;
            _keysTail.Previous = _keysHead;
        }

        public void AddKeyNode(DoublyLinkedListNode<KeyEntry> node)
        {
            var next = _keysHead.Next!;

            node.Previous = _keysHead;
            node.Next = next;
            _keysHead.Next = node;
            next.Previous = node;

            KeyCount++;
        }

        public void RemoveKeyNode(DoublyLinkedListNode<KeyEntry> node)
        {
            node.Previous!.Next = node.Next;
            node.Next!.Previous = node.Previous;

            KeyCount--;
        }

        public string PeekAnyKey() => _keysHead.Next == _keysTail ? "" : _keysHead.Next!.Value.Key;
    }

    private sealed class AllOne
    {
        private readonly HashMap<string, DoublyLinkedListNode<KeyEntry>> _keyNode = new();
        private readonly DoublyLinkedListNode<Bucket> _head = new();
        private readonly DoublyLinkedListNode<Bucket> _tail = new();

        public AllOne()
        {
            _head.Next = _tail;
            _tail.Previous = _head;
        }

        public void Inc(string key)
        {
            var hasExisting = _keyNode.TryGetValue(key, out var keyEntryNode);

            if (!hasExisting)
            {
                keyEntryNode = new DoublyLinkedListNode<KeyEntry> { Value = new KeyEntry(key) };
            }

            var anchor = hasExisting ? keyEntryNode.Value.CurrentBucket! : _head;
            var newCount = (hasExisting ? anchor.Value.Count : 0) + 1;

            var candidate = anchor.Next!;
            var newBucketNode = candidate != _tail && candidate.Value.Count == newCount
                ? candidate
                : InsertBucketAfter(anchor, newCount);

            if (hasExisting)
            {
                anchor.Value.RemoveKeyNode(keyEntryNode);

                if (anchor.Value.KeyCount == 0)
                {
                    Unlink(anchor);
                }
            }

            newBucketNode.Value.AddKeyNode(keyEntryNode);
            keyEntryNode.Value.CurrentBucket = newBucketNode;
            _keyNode.Set(key, keyEntryNode);
        }

        public void Dec(string key)
        {
            if (!_keyNode.TryGetValue(key, out var keyEntryNode))
            {
                return;
            }

            var oldBucketNode = keyEntryNode.Value.CurrentBucket!;
            var newCount = oldBucketNode.Value.Count - 1;
            var anchor = oldBucketNode.Previous!;

            var newBucketNode = newCount == 0
                ? null
                : anchor != _head && anchor.Value.Count == newCount ? anchor : InsertBucketAfter(anchor, newCount);

            oldBucketNode.Value.RemoveKeyNode(keyEntryNode);

            if (oldBucketNode.Value.KeyCount == 0)
            {
                Unlink(oldBucketNode);
            }

            if (newBucketNode is null)
            {
                _keyNode.TryRemove(key);
                return;
            }

            newBucketNode.Value.AddKeyNode(keyEntryNode);
            keyEntryNode.Value.CurrentBucket = newBucketNode;
            _keyNode.Set(key, keyEntryNode);
        }

        public string GetMaxKey() => _tail.Previous == _head ? "" : _tail.Previous!.Value.PeekAnyKey();

        public string GetMinKey() => _head.Next == _tail ? "" : _head.Next!.Value.PeekAnyKey();

        private static void Unlink(DoublyLinkedListNode<Bucket> node)
        {
            node.Previous!.Next = node.Next;
            node.Next!.Previous = node.Previous;
        }

        private static DoublyLinkedListNode<Bucket> InsertBucketAfter(DoublyLinkedListNode<Bucket> anchor, int count)
        {
            var node = new DoublyLinkedListNode<Bucket> { Value = new Bucket(count) };
            var next = anchor.Next!;

            node.Previous = anchor;
            node.Next = next;
            anchor.Next = node;
            next.Previous = node;

            return node;
        }
    }
}
