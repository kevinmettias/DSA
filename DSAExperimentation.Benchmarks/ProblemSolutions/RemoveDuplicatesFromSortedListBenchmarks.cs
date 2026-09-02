using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedListSolution's, the same
// methods RemoveDuplicatesFromSortedListTests proves correct. [GlobalSetup] hoists
// the workload values, but the list itself is rebuilt fresh inside each benchmark
// method rather than cached, because the in-place-scan strategy splices nodes out
// of the list it is handed - a cached list would only be valid for the first
// measured iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class RemoveDuplicatesFromSortedListBenchmarks
{
    private const int DuplicateRunLength = 3;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / DuplicateRunLength).ToArray();

    [Benchmark(Baseline = true)]
    public object? DistinctFilter() =>
        RemoveDuplicatesFromSortedListSolution.DeleteDuplicatesByDistinctFilter(BuildList(_values));

    [Benchmark]
    public object? InPlaceScan() =>
        RemoveDuplicatesFromSortedListSolution.DeleteDuplicatesByInPlaceScan(BuildList(_values));

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
}
