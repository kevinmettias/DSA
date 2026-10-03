using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.SwapNodesInPairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SwapNodesInPairsSolution's, the same methods
// SwapNodesInPairsSolutionTests proves correct. SwapPairsByPointerRewiring relinks the
// input nodes' own Next pointers in place, so - like MergeKSortedListsBenchmarks'
// heap merge - the list cannot be hoisted into [GlobalSetup] and reused across
// iterations: a swap from one iteration would leave the next iteration a
// different (and differently rooted) structure than the one being measured.
// [GlobalSetup] therefore only seeds the raw values, and each [Benchmark] call
// rebuilds a fresh list from them. The rebuild is timed on purpose, and
// ArrayRoundTrip, which only reads its list, pays it too so both arms carry the same
// cost.
//
// Returns object, not SinglyLinkedListNode<int>? - the node type is internal, so
// a public [Benchmark] method cannot name it as a return type (CS0050). Length stops
// at LC 24's 100 nodes, valued 1..100 inside its [0, 100].
public class SwapNodesInPairsBenchmarks
{
    private int[] _values = [];

    [Params(10, 100)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRoundTrip() =>
        SwapNodesInPairsSolution.SwapPairsByArrayRoundTrip(LeetCodeWireFormat.ToLinkedList(_values));

    [Benchmark]
    public object? PointerRewiring() =>
        SwapNodesInPairsSolution.SwapPairsByPointerRewiring(LeetCodeWireFormat.ToLinkedList(_values));
}
