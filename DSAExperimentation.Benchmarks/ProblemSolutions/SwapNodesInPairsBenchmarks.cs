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
// rebuilds a fresh list from them.
//
// Returns object, not SinglyLinkedListNode<int>? - the node type is internal, so
// a public [Benchmark] method cannot name it as a return type (CS0050).
public class SwapNodesInPairsBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
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
