using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RemoveZeroSumConsecutiveNodesFromLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// RemoveZeroSumConsecutiveNodesFromLinkedListSolution's, the same methods
// RemoveZeroSumConsecutiveNodesFromLinkedListTests proves correct. [GlobalSetup]
// hoists the workload values, but the list itself is rebuilt fresh inside each
// benchmark method rather than cached, because both strategies splice nodes out of
// the list they are handed - a cached list would only be valid for the first
// measured iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class RemoveZeroSumConsecutiveNodesFromLinkedListBenchmarks
{
    // Small values around zero, so zero-sum runs are dense enough to exercise the
    // splicing rather than just the walk.
    private const int NodeValueMagnitude = 3;
    private const int NodeValueExclusiveUpperBound = 4;

    // LC problem number is not used as the seed here; 1 keeps the previous
    // workload's values identical to what this benchmark measured before migration.
    private const int ValueSeed = 1;

    private int[] _values = [];

    [Params(300, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(ValueSeed);
        _values = Enumerable.Range(0, Length)
            .Select(_ => random.Next(-NodeValueMagnitude, NodeValueExclusiveUpperBound))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public object? NestedRescan() =>
        RemoveZeroSumConsecutiveNodesFromLinkedListSolution
            .RemoveZeroSumSublistsByNestedRescan(BuildList(_values));

    [Benchmark]
    public object? PrefixSumMap() =>
        RemoveZeroSumConsecutiveNodesFromLinkedListSolution
            .RemoveZeroSumSublistsByPrefixSumMap(BuildList(_values));

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
