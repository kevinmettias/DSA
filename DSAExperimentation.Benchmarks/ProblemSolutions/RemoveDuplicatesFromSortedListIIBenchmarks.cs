using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedListII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedListIISolution's, the same
// methods RemoveDuplicatesFromSortedListIITests proves correct. [GlobalSetup]
// hoists the workload values, but the list itself is rebuilt fresh inside each
// benchmark method rather than cached, because the two-pointer-scan strategy
// splices nodes out of the list it is handed - a cached list would only be valid
// for the first measured iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class RemoveDuplicatesFromSortedListIIBenchmarks
{
    private const int RunLengthDivisor = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / RunLengthDivisor).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayGroupFilter() =>
        RemoveDuplicatesFromSortedListIISolution.DeleteDuplicatesByArrayGroupFilter(BuildList(_values));

    [Benchmark]
    public object? TwoPointerScan() =>
        RemoveDuplicatesFromSortedListIISolution.DeleteDuplicatesByTwoPointerScan(BuildList(_values));

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
