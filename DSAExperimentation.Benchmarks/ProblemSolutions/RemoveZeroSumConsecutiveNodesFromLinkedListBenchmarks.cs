using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveZeroSumConsecutiveNodesFromLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// RemoveZeroSumConsecutiveNodesFromLinkedListSolution's, the same methods
// RemoveZeroSumConsecutiveNodesFromLinkedListSolutionTests proves correct. [GlobalSetup]
// hoists the workload values, but the list itself is rebuilt fresh inside each
// benchmark method rather than cached, because both strategies splice nodes out of
// the list they are handed - a cached list would only be valid for the first
// measured iteration. The rebuild is timed on purpose, and both arms pay it alike.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
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

    [Params(300, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(ValueSeed);
        _values = SeededDraws.Values(Length, -NodeValueMagnitude, NodeValueExclusiveUpperBound, random);
    }

    [Benchmark(Baseline = true)]
    public object? NestedRescan() =>
        RemoveZeroSumConsecutiveNodesFromLinkedListSolution
            .RemoveZeroSumSublistsByNestedRescan(LeetCodeWireFormat.ToLinkedList(_values));

    [Benchmark]
    public object? PrefixSumMap() =>
        RemoveZeroSumConsecutiveNodesFromLinkedListSolution
            .RemoveZeroSumSublistsByPrefixSumMap(LeetCodeWireFormat.ToLinkedList(_values));
}
