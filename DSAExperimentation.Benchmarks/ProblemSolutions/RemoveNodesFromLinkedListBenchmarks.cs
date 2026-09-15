using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.RemoveNodesFromLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveNodesFromLinkedListSolution's, the same methods
// RemoveNodesFromLinkedListTests proves correct. Values are a random permutation so
// no node's removal decision short-circuits the brute-force rescan early.
//
// Only the value permutation is hoisted to [GlobalSetup]; the list itself is built
// inside each [Benchmark] call rather than shared, because both strategies splice
// .Next pointers in place - reusing one pre-built list across iterations would let
// the first iteration's removals leave every later iteration measuring an
// already-non-increasing list. Both arms pay the identical construction cost, so the
// comparison between them is unaffected.
[MemoryDiagnoser]
public class RemoveNodesFromLinkedListBenchmarks
{
    private const int RandomSeed = 2487; private int[] _values = [];

    // LC problem number

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    // Returns object, not SinglyLinkedListNode<int> - the node type is internal, so
    // a public [Benchmark] method cannot name it as a return type (CS0050).
    [Benchmark(Baseline = true)]
    public object? BruteForceScan() =>
        RemoveNodesFromLinkedListSolution.RemoveNodesByBruteForceScan(
            LeetCodeWireFormat.ToLinkedList(_values));

    [Benchmark]
    public object? MonotonicStackSweep() =>
        RemoveNodesFromLinkedListSolution.RemoveNodesByMonotonicStack(
            LeetCodeWireFormat.ToLinkedList(_values));
}
