using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.DeleteTheMiddleNodeOfALinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeleteTheMiddleNodeOfALinkedListSolution's, the
// same methods DeleteTheMiddleNodeOfALinkedListSolutionTests proves correct.
// CountThenRebuild counts the list and then copies every node but the middle -
// two traversals and n-1 allocations - while SlowFastPointers finds the middle's
// predecessor in one traversal and splices it out in place with no new nodes.
// [GlobalSetup] prepares only the value array: the list itself is rebuilt inside
// each [Benchmark] call rather than shared, because deleting is destructive and
// one pre-built list would let the first iteration's splice make every later
// iteration measure an already-shortened list - the same "fresh copy per
// invocation" discipline SortAnArrayBenchmarks uses for its in-place sort.
public class DeleteTheMiddleNodeOfALinkedListBenchmarks
{
    private const int RandomSeed = 2095; // LC problem number
    private const int ValueRangeExclusive = 1_000;

    private int[] _values = [];

    [Params(500, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, 0, ValueRangeExclusive, random);
    }

    // Returns object, not SinglyLinkedListNode<int> - the node type is internal,
    // so a public [Benchmark] method cannot name it as a return type (CS0050).
    [Benchmark(Baseline = true)]
    public object? CountThenRebuild() =>
        DeleteTheMiddleNodeOfALinkedListSolution.DeleteMiddleByCountThenRebuild(
            LeetCodeWireFormat.ToLinkedList(_values));

    [Benchmark]
    public object? SlowFastPointers() =>
        DeleteTheMiddleNodeOfALinkedListSolution.DeleteMiddleBySlowFastPointers(
            LeetCodeWireFormat.ToLinkedList(_values));
}
