using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SortList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is SortListSolution's, the same method
// SortListTests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (`Baseline() => 1`, `PrimitiveComposed() => 1`)
// rather than a second strategy to reconcile. The strategy only reads the list
// it is handed (it rebuilds a fresh one from the sorted array rather than
// relinking in place), so - unlike InsertionSortListBenchmarks -
// [GlobalSetup] can build the list once and every iteration reuses it, mirroring
// ConvertSortedListToBinarySearchTreeBenchmarks.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so
// a public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class SortListBenchmarks
{
    private const int ShuffleSeed = 148;

    private SinglyLinkedListNode<int>? _head;

    [Params(100, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(1, Length).ToArray();
        var random = new Random(ShuffleSeed);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _head = BuildList(values);
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    [Benchmark(Baseline = true)]
    public object? MergeSortOverSequence() => SortListSolution.SortByMergeSortOverSequence(_head);
}
