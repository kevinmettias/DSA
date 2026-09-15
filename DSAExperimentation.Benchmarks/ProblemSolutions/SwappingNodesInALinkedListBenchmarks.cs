using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.SwappingNodesInALinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SwappingNodesInALinkedListSolution's, the same
// methods SwappingNodesInALinkedListTests proves correct. Materializing the list
// into a BCL buffer to get random access to index n - k, vs. a fast/slow pair of
// SinglyLinkedListNode<int> references that finds the same node in one O(n),
// O(1)-extra-space pass - the same "array baseline vs. linked-list primitive"
// shape SwapNodesInPairsBenchmarks uses for its own problem.
//
// SwapNodesByTwoPointerWalk rewrites the Values of the list it is handed, so - as
// in SwapNodesInPairsBenchmarks - the chain cannot be hoisted into [GlobalSetup]
// and reused across iterations. [GlobalSetup] decides only how large the workload
// is and which position k picks; each [Benchmark] call builds its own list, so both
// arms pay the same construction and the measurement is of the search alone.
//
// Returns object, not SinglyLinkedListNode<int>? - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class SwappingNodesInALinkedListBenchmarks
{
    private const int TargetIndexDivisor = 3; private int[] _values = [];

    private int _k;
    // k picks a node one third of the way into the list

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(1, Length).ToArray();
        _k = Length / TargetIndexDivisor;
    }

    [Benchmark(Baseline = true)]
    public object? ArrayMaterializeSwap() =>
        SwappingNodesInALinkedListSolution.SwapNodesByArrayMaterialize(BuildList(_values), _k);

    [Benchmark]
    public object? LinkedListTwoPointerSwap() =>
        SwappingNodesInALinkedListSolution.SwapNodesByTwoPointerWalk(BuildList(_values), _k);

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
