using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// All O`one Data Structure (LC 432): a brute-force baseline (a plain Dictionary<string,int> of
// counts, with getMaxKey/getMinKey each doing an O(n) full scan) vs. an ascending doubly linked
// list of count "buckets" spliced directly through DoublyLinkedListNode<T>'s own public
// Previous/Next - the same frequency-bucket idea LfuCache.cs already uses, generalized beyond
// front-insertion-only. Each bucket's own key membership is a SECOND, independent
// DoublyLinkedListNode chain rather than a HashMap<string,bool>: HashMap<TKey,TValue>.Keys is
// documented as an eager List snapshot of every entry, so "grab any one key" through it would
// silently cost O(bucket size) per call - a first version of this benchmark measured exactly
// that mistake (the "primitive" approach lost by 5-10x). getMaxKey/getMinKey are genuinely O(1)
// tail/head lookups this way. Both approaches replay the exact same operation sequence.
[MemoryDiagnoser]
public class AllOneDataStructureBenchmarks
{
    private const int RandomSeed = 432; // LC problem number
    private const string KeyPrefix = "key";
    private const int OpsCapacityMultiplier = 4;
    private const int GetMaxKeyOpType = 2;
    private const int GetMinKeyOpType = 3;
    private const string NoKeySentinel = "";

    [Params(200, 5_000)]
    public int Length;

    private (int Type, string Key)[] _ops = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var keys = Enumerable.Range(0, Length).Select(i => KeyPrefix + i).ToArray();
        var ops = new List<(int Type, string Key)>(Length * OpsCapacityMultiplier);

        foreach (var key in keys)
        {
            ops.Add((0, key));
        }

        for (var round = 0; round < Length; round++)
        {
            ops.Add((0, keys[random.Next(keys.Length)]));
            ops.Add((GetMaxKeyOpType, NoKeySentinel));
            ops.Add((GetMinKeyOpType, NoKeySentinel));
        }

        _ops = [.. ops];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDictionaryScan()
    {
        var counts = new Dictionary<string, int>();
        var checksum = 0;

        foreach (var (type, key) in _ops)
        {
            switch (type)
            {
                case 0:
                    counts[key] = counts.GetValueOrDefault(key) + 1;
                    break;
                case GetMaxKeyOpType:
                    checksum += MaxKey(counts).Length;
                    break;
                case GetMinKeyOpType:
                    checksum += MinKey(counts).Length;
                    break;
            }
        }

        return checksum;
    }

    [Benchmark]
    public int BucketedLinkedListOnePass()
    {
        var allOne = new AllOne();
        var checksum = 0;

        foreach (var (type, key) in _ops)
        {
            switch (type)
            {
                case 0:
                    allOne.Inc(key);
                    break;
                case GetMaxKeyOpType:
                    checksum += allOne.GetMaxKey().Length;
                    break;
                case GetMinKeyOpType:
                    checksum += allOne.GetMinKey().Length;
                    break;
            }
        }

        return checksum;
    }

    private static string MaxKey(Dictionary<string, int> counts)
    {
        var best = NoKeySentinel;
        var bestCount = int.MinValue;

        foreach (var (key, count) in counts)
        {
            if (count > bestCount)
            {
                best = key;
                bestCount = count;
            }
        }

        return best;
    }

    private static string MinKey(Dictionary<string, int> counts)
    {
        var best = NoKeySentinel;
        var bestCount = int.MaxValue;

        foreach (var (key, count) in counts)
        {
            if (count < bestCount)
            {
                best = key;
                bestCount = count;
            }
        }

        return best;
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

        public string PeekAnyKey() => _keysHead.Next == _keysTail ? NoKeySentinel : _keysHead.Next!.Value.Key;
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
            var newBucketNode = FindOrCreateBucket(anchor, newCount);

            if (hasExisting)
            {
                DetachFromBucket(anchor, keyEntryNode);
            }

            AttachToBucket(newBucketNode, key, keyEntryNode);
        }

        public string GetMaxKey() => _tail.Previous == _head ? NoKeySentinel : _tail.Previous!.Value.PeekAnyKey();

        public string GetMinKey() => _head.Next == _tail ? NoKeySentinel : _head.Next!.Value.PeekAnyKey();

        private DoublyLinkedListNode<Bucket> FindOrCreateBucket(DoublyLinkedListNode<Bucket> anchor, int newCount)
        {
            var candidate = anchor.Next!;
            return candidate != _tail && candidate.Value.Count == newCount
                ? candidate
                : InsertBucketAfter(anchor, newCount);
        }

        private static void DetachFromBucket(
            DoublyLinkedListNode<Bucket> anchor, DoublyLinkedListNode<KeyEntry> keyEntryNode)
        {
            anchor.Value.RemoveKeyNode(keyEntryNode);

            if (anchor.Value.KeyCount == 0)
            {
                Unlink(anchor);
            }
        }

        private void AttachToBucket(
            DoublyLinkedListNode<Bucket> bucketNode, string key, DoublyLinkedListNode<KeyEntry> keyEntryNode)
        {
            bucketNode.Value.AddKeyNode(keyEntryNode);
            keyEntryNode.Value.CurrentBucket = bucketNode;
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
    }
}
