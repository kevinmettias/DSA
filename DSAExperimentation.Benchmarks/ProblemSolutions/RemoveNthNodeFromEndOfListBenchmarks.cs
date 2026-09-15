using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveNthNodeFromEndOfList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveNthNodeFromEndOfListSolution's, the same
// methods RemoveNthNodeFromEndOfListTests proves correct. [GlobalSetup] hoists the
// workload values, but the list itself is rebuilt fresh inside each benchmark
// method rather than cached, because both strategies mutate/replace it - a cached
// list would only be valid for the first measured iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class RemoveNthNodeFromEndOfListBenchmarks
{
    // Both benchmarks remove the node at roughly the middle position so they do
    // equivalent work; this divisor picks that middle position from Length.
    private const int MiddlePositionDivisor = 2;

    private int[] _values = [];
    [Params(200, 5_000)] public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        RemoveNthNodeFromEndOfListSolution.RemoveByArrayRebuild(
            BuildList(_values), Length / MiddlePositionDivisor);

    [Benchmark]
    public object? TwoRunner() =>
        RemoveNthNodeFromEndOfListSolution.RemoveByTwoRunner(
            BuildList(_values), Length / MiddlePositionDivisor);

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
