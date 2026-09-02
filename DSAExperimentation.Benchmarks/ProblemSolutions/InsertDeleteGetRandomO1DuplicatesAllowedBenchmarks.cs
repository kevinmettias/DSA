using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Insert Delete GetRandom O(1) - Duplicates allowed (LC 381): a plain List<int>
// baseline (Remove does a linear IndexOf scan to find SOME occurrence of the target
// value, then an indexed removal that shifts every later element) vs. this repo's
// HashMap<int, DoublyLinkedList<int>> (value -> its occurrence nodes) +
// HashMap<int, DoublyLinkedListNode<int>> (array position -> the occurrence node
// currently representing it) + DynamicArray<int> composition (LruCache/LfuCache's
// intrusive-node precedent). _insertOrder is drawn from a narrow value range so most
// values collide with earlier ones, forcing real duplicate chains for Remove to walk
// past in the baseline instead of an artificially duplicate-free input.
[MemoryDiagnoser]
public class InsertDeleteGetRandomO1DuplicatesAllowedBenchmarks
{
    // A small, fixed value range - not scaled with Count - so the average duplicate
    // cluster size grows linearly with Count instead of staying constant, which is what
    // makes the baseline's per-removal IndexOf scan genuinely O(Count) (and the whole
    // removal phase O(Count^2)) rather than an accidentally-cheap O(1)-ish scan.
    private const int DistinctValues = 10;

    [Params(200, 20_000)]
    public int Count;

    private int[] _insertOrder = null!;
    private int[] _removalOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);

        _insertOrder = Enumerable.Range(0, Count).Select(_ => random.Next(DistinctValues)).ToArray();
        _removalOrder = (int[])_insertOrder.Clone();
        Shuffle(_removalOrder, random);
    }

    private static void Shuffle(int[] items, Random random)
    {
        for (var i = items.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }

    [Benchmark(Baseline = true)]
    public int ListBased()
    {
        var values = new List<int>();

        foreach (var value in _insertOrder)
        {
            values.Add(value);
        }

        foreach (var value in _removalOrder)
        {
            var index = values.IndexOf(value);
            if (index >= 0)
            {
                values.RemoveAt(index);
            }
        }

        return values.Count;
    }

    [Benchmark]
    public int LinkedOccurrencesComposed()
    {
        var values = new DynamicArray<int>();
        var occurrencesByValue = new HashMap<int, DoublyLinkedList<int>>();
        var nodeByPosition = new HashMap<int, DoublyLinkedListNode<int>>();

        foreach (var value in _insertOrder)
        {
            AddOneOccurrence(value, values, occurrencesByValue, nodeByPosition);
        }

        foreach (var value in _removalOrder)
        {
            RemoveOneOccurrence(value, values, occurrencesByValue, nodeByPosition);
        }

        return values.Count;
    }

    private static void AddOneOccurrence(
        int value,
        DynamicArray<int> values,
        HashMap<int, DoublyLinkedList<int>> occurrencesByValue,
        HashMap<int, DoublyLinkedListNode<int>> nodeByPosition)
    {
        if (!occurrencesByValue.TryGetValue(value, out var occurrences))
        {
            occurrences = new DoublyLinkedList<int>();
            occurrencesByValue.Set(value, occurrences);
        }

        var position = values.Count;
        values.Add(value);

        var node = new DoublyLinkedListNode<int> { Value = position };
        occurrences.AddFront(node);
        nodeByPosition.Set(position, node);
    }

    private static void RemoveOneOccurrence(
        int value,
        DynamicArray<int> values,
        HashMap<int, DoublyLinkedList<int>> occurrencesByValue,
        HashMap<int, DoublyLinkedListNode<int>> nodeByPosition)
    {
        if (!occurrencesByValue.TryGetValue(value, out var occurrences) || occurrences.Count == 0)
        {
            return;
        }

        var removedPosition = PopOccurrence(value, occurrences, occurrencesByValue, nodeByPosition);
        SwapOutPosition(values, nodeByPosition, removedPosition);
    }

    private static int PopOccurrence(
        int value,
        DoublyLinkedList<int> occurrences,
        HashMap<int, DoublyLinkedList<int>> occurrencesByValue,
        HashMap<int, DoublyLinkedListNode<int>> nodeByPosition)
    {
        var removedNode = occurrences.PopBack();
        var removedPosition = removedNode.Value;
        nodeByPosition.TryRemove(removedPosition);

        if (occurrences.Count == 0)
        {
            occurrencesByValue.TryRemove(value);
        }

        return removedPosition;
    }

    private static void SwapOutPosition(DynamicArray<int> values, HashMap<int, DoublyLinkedListNode<int>> nodeByPosition, int removedPosition)
    {
        var lastPosition = values.Count - 1;
        var lastValue = values.Get(lastPosition);
        values.Set(removedPosition, lastValue);

        if (removedPosition != lastPosition)
        {
            MoveTrackedNode(nodeByPosition, lastPosition, removedPosition);
        }

        values.RemoveAt(lastPosition);
    }

    private static void MoveTrackedNode(HashMap<int, DoublyLinkedListNode<int>> nodeByPosition, int fromPosition, int toPosition)
    {
        nodeByPosition.TryGetValue(fromPosition, out var movedNode);
        movedNode.Value = toPosition;
        nodeByPosition.TryRemove(fromPosition);
        nodeByPosition.Set(toPosition, movedNode);
    }
}
