using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.DeleteTheMiddleNodeOfALinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeleteTheMiddleNodeOfALinkedListSolution's, the
// same methods DeleteTheMiddleNodeOfALinkedListTests proves correct.
// CountThenRebuild counts the list and then copies every node but the middle -
// two traversals and n-1 allocations - while SlowFastPointers finds the middle's
// predecessor in one traversal and splices it out in place with no new nodes.
// [GlobalSetup] prepares only the value array: the list itself is rebuilt inside
// each [Benchmark] call rather than shared, because deleting is destructive and
// one pre-built list would let the first iteration's splice make every later
// iteration measure an already-shortened list - the same "fresh copy per
// invocation" discipline SortAnArrayBenchmarks uses for its in-place sort.
[MemoryDiagnoser]
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
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueRangeExclusive)).ToArray();
    }

    // Returns object, not SinglyLinkedListNode<int> - the node type is internal,
    // so a public [Benchmark] method cannot name it as a return type (CS0050).
    [Benchmark(Baseline = true)]
    public object? CountThenRebuild() =>
        DeleteTheMiddleNodeOfALinkedListSolution.DeleteMiddleByCountThenRebuild(BuildList(_values));

    [Benchmark]
    public object? SlowFastPointers() =>
        DeleteTheMiddleNodeOfALinkedListSolution.DeleteMiddleBySlowFastPointers(BuildList(_values));

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        for (var i = 1; i < values.Length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[i]);
            tail = tail.Next;
        }

        return head;
    }
}
