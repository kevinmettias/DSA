using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.AllOneDataStructure;

// LeetCode 432. All O`one Data Structure: a key-count multiset supporting O(1)
// Inc/Dec plus O(1) GetMaxKey/GetMinKey.
//
// This is a design problem - LeetCode's own shape is a stateful object with four
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// LRUCacheSolution/MinStackSolution use for their own design problems.
//
// CreateByBucketedLinkedList is an ascending doubly linked list of count "buckets" -
// the same frequency-bucket idea LfuCache.cs already uses for O(1) eviction,
// generalized from "insert only at the front" to "insert next to whichever neighbor
// bucket the moving key should land beside" via DoublyLinkedListNode's own public
// Previous/Next (its own doc comment already anticipates a composer splicing
// directly - the same way LruCache/LfuCache splice indirectly through
// DoublyLinkedList<T>). GetMaxKey/GetMinKey are then just tail.Previous/head.Next -
// O(1) even across gaps in the count sequence, which a bare incremented/decremented
// int bound (LfuCache's own _minFrequency trick) cannot guarantee once a key's count
// can both increase AND decrease.
//
// Each bucket's own key membership is a SECOND, independent DoublyLinkedListNode
// chain, not a HashMap<string,bool> - HashMap<TKey,TValue>.Keys is documented as an
// eager List snapshot of every entry, so "grab any one key" would silently cost
// O(bucket size) per call instead of O(1), defeating the whole point.
//
// CreateByDictionaryScan is the textbook baseline this composition has to justify
// itself against: a plain Dictionary<string,int> of counts, with GetMaxKey/GetMinKey
// each doing an O(n) full scan.
internal static class AllOneDataStructureSolution
{
    public static IAllOne CreateByBucketedLinkedList() => new BucketedLinkedListAllOne();

    public static IAllOne CreateByDictionaryScan() => new DictionaryScanAllOne();

    internal interface IAllOne
    {
        void Inc(string key);

        void Dec(string key);

        string GetMaxKey();

        string GetMinKey();
    }

    private sealed class DictionaryScanAllOne : IAllOne
    {
        private const string NoKeySentinel = "";

        private readonly Dictionary<string, int> _counts = [];

        public void Inc(string key) => _counts[key] = _counts.GetValueOrDefault(key) + 1;

        public void Dec(string key)
        {
            if (!_counts.TryGetValue(key, out var count))
            {
                return;
            }

            if (count == 1)
            {
                _counts.Remove(key);
            }
            else
            {
                _counts[key] = count - 1;
            }
        }

        public string GetMaxKey()
        {
            var best = NoKeySentinel;
            var bestCount = int.MinValue;

            foreach (var (key, count) in _counts)
            {
                if (count > bestCount)
                {
                    best = key;
                    bestCount = count;
                }
            }

            return best;
        }

        public string GetMinKey()
        {
            var best = NoKeySentinel;
            var bestCount = int.MaxValue;

            foreach (var (key, count) in _counts)
            {
                if (count < bestCount)
                {
                    best = key;
                    bestCount = count;
                }
            }

            return best;
        }
    }

    private sealed class BucketedLinkedListAllOne : IAllOne
    {
        private readonly HashMap<string, DoublyLinkedListNode<KeyEntry>> _keyNode = new();
        private readonly DoublyLinkedListNode<Bucket> _head = new();
        private readonly DoublyLinkedListNode<Bucket> _tail = new();

        public BucketedLinkedListAllOne()
        {
            _head.Next = _tail;
            _tail.Previous = _head;
        }

        public void Inc(string key)
        {
            var hasExisting = _keyNode.TryGetValue(key, out var keyEntryNode);
            keyEntryNode = ResolveIncKeyEntryNode(key, hasExisting, keyEntryNode);

            var anchor = hasExisting ? keyEntryNode.Value.CurrentBucket! : _head;
            var newCount = (hasExisting ? anchor.Value.Count : 0) + 1;

            var newBucketNode = FindOrInsertIncBucket(anchor, newCount);

            if (hasExisting)
            {
                DetachFromBucket(anchor, keyEntryNode);
            }

            AttachToBucket(key, keyEntryNode, newBucketNode);
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

            var newBucketNode = FindOrInsertDecBucket(anchor, newCount);

            DetachFromBucket(oldBucketNode, keyEntryNode);

            if (newBucketNode is null)
            {
                _keyNode.TryRemove(key);
                return;
            }

            AttachToBucket(key, keyEntryNode, newBucketNode);
        }

        public string GetMaxKey() => _tail.Previous == _head ? "" : _tail.Previous!.Value.PeekAnyKey();

        public string GetMinKey() => _head.Next == _tail ? "" : _head.Next!.Value.PeekAnyKey();

        private static DoublyLinkedListNode<KeyEntry> ResolveIncKeyEntryNode(
            string key, bool hasExisting, DoublyLinkedListNode<KeyEntry>? keyEntryNode)
            => hasExisting ? keyEntryNode! : new DoublyLinkedListNode<KeyEntry> { Value = new KeyEntry(key) };

        private DoublyLinkedListNode<Bucket> FindOrInsertIncBucket(DoublyLinkedListNode<Bucket> anchor, int newCount)
        {
            var candidate = anchor.Next!;
            return candidate != _tail && candidate.Value.Count == newCount
                ? candidate
                : InsertBucketAfter(anchor, newCount);
        }

        private DoublyLinkedListNode<Bucket>? FindOrInsertDecBucket(DoublyLinkedListNode<Bucket> anchor, int newCount)
            => newCount == 0
                ? null
                : anchor != _head && anchor.Value.Count == newCount ? anchor : InsertBucketAfter(anchor, newCount);

        private static void DetachFromBucket(
            DoublyLinkedListNode<Bucket> bucketNode, DoublyLinkedListNode<KeyEntry> keyEntryNode)
        {
            bucketNode.Value.RemoveKeyNode(keyEntryNode);

            if (bucketNode.Value.KeyCount == 0)
            {
                Unlink(bucketNode);
            }
        }

        private void AttachToBucket(
            string key, DoublyLinkedListNode<KeyEntry> keyEntryNode, DoublyLinkedListNode<Bucket> newBucketNode)
        {
            newBucketNode.Value.AddKeyNode(keyEntryNode);
            keyEntryNode.Value.CurrentBucket = newBucketNode;
            _keyNode.Set(key, keyEntryNode);
        }

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
    }
}
