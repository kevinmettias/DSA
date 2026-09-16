using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.MergeInBetweenLinkedLists;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeInBetweenLinkedListsSolution's, the same methods
// MergeInBetweenLinkedListsTests proves correct. [GlobalSetup] hoists the workload
// values and the splice window, but the lists themselves are rebuilt fresh inside
// each benchmark method rather than cached, because the splice strategy rewires the
// nodes it is handed - a cached chain would only be valid for the first measured
// iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class MergeInBetweenLinkedListsBenchmarks
{
    private const int SecondListLength = 5;
    private const int SecondListValueOffset = 1_000_000;
    private const int SpliceStartDivisor = 3;

    private int[] _list1Values = [];

    private int[] _list2Values = [];
    private int _fromIndex;
    private int _toIndex;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _list1Values = Enumerable.Range(0, Length).ToArray();
        _list2Values = Enumerable.Range(SecondListValueOffset, SecondListLength).ToArray();
        _fromIndex = Length / SpliceStartDivisor;
        _toIndex = _fromIndex + SecondListLength - 1;
    }

    [Benchmark(Baseline = true)]
    public object ArraySpliceRebuild() =>
        MergeInBetweenLinkedListsSolution.MergeInBetweenByArrayRebuild(
            BuildList(_list1Values), _fromIndex, _toIndex, BuildList(_list2Values));

    [Benchmark]
    public object LinkedListSplice() =>
        MergeInBetweenLinkedListsSolution.MergeInBetweenByPointerSplice(
            BuildList(_list1Values), _fromIndex, _toIndex, BuildList(_list2Values));

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }
}
