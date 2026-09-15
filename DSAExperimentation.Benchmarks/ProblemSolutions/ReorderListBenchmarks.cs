using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ReorderList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ReorderListSolution's, the same method
// ReorderListTests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (`Baseline() => 1`, `PrimitiveComposed() => 1`)
// rather than a second strategy to reconcile. [GlobalSetup] hoists the
// workload values, but the list itself is rebuilt fresh inside the benchmark
// method rather than cached, because the strategy mutates in place - a
// cached list would only be valid for the first measured iteration (same
// shape RotateListBenchmarks already uses).
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal,
// so a public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class ReorderListBenchmarks
{
    private int[] _values = [];

    [Params(1_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ReverseAndMergeInPlace()
    {
        var head = BuildList(_values);
        ReorderListSolution.ReorderByReverseAndMergeInPlace(head);
        return head;
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
}
