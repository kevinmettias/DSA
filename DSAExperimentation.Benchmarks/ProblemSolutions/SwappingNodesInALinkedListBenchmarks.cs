using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.SwappingNodesInALinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SwappingNodesInALinkedListSolution's, the same
// methods SwappingNodesInALinkedListSolutionTests proves correct. Materializing the list
// into a BCL buffer to get random access to index n - kthPosition, vs. a fast/slow
// pair of SinglyLinkedListNode<int> references that finds the same node in one O(n),
// O(1)-extra-space pass - the same "array baseline vs. linked-list primitive"
// shape SwapNodesInPairsBenchmarks uses for its own problem.
//
// SwapNodesByTwoPointerWalk rewrites the Values of the list it is handed, so - as
// in SwapNodesInPairsBenchmarks - the chain cannot be hoisted into [GlobalSetup]
// and reused across iterations. [GlobalSetup] decides only how large the workload
// is and which position kthPosition picks; each [Benchmark] call builds its own list.
// The rebuild is timed on purpose because the two-pointer strategy mutates its
// input, and both arms pay the same construction, so what differs between them is
// the search alone.
//
// The node at 1-based position p holds p mod 101, so the values cycle through
// LC 1721's [0, 100].
public class SwappingNodesInALinkedListBenchmarks
{
    private const int TargetIndexDivisor = 3;

    // One past LC 1721's largest node value, so a position's remainder is a legal value.
    private const int NodeValueSpan = 101;

    private int[] _values = [];

    private int _kthPosition;

    // kthPosition picks a node one third of the way into the list

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(1, Length).Select(position => position % NodeValueSpan).ToArray();
        _kthPosition = Length / TargetIndexDivisor;
    }

    [Benchmark(Baseline = true)]
    public SinglyLinkedListNode<int>? ArrayMaterializeSwap() =>
        SwappingNodesInALinkedListSolution.SwapNodesByArrayMaterialize(
            LeetCodeWireFormat.ToLinkedList(_values), _kthPosition);

    [Benchmark]
    public SinglyLinkedListNode<int>? LinkedListTwoPointerSwap() =>
        SwappingNodesInALinkedListSolution.SwapNodesByTwoPointerWalk(
            LeetCodeWireFormat.ToLinkedList(_values), _kthPosition);
}
