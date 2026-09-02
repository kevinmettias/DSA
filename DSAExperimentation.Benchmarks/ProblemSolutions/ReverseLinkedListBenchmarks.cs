using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ReverseLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ReverseLinkedListSolution's, the same method
// ReverseLinkedListTests proves correct. [GlobalSetup] hoists the workload
// values, but the list itself is rebuilt fresh inside the benchmark method
// rather than cached, because the strategy mutates the list it is handed - a
// cached list would only be valid for the first measured iteration (mirrors
// ReverseLinkedListIIBenchmarks' identical constraint).
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal,
// so a public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class ReverseLinkedListBenchmarks
{
    private int[] _values = null!;

    [Params(200, 5_000)]
    public int Length;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark]
    public object? IterativeRewire() =>
        ReverseLinkedListSolution.ReverseListByIterativeRewire(BuildList(_values));

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
