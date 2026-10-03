using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ReorderList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ReorderListSolution's, the same method
// ReorderListSolutionTests proves correct. Pre-migration this class was an untested
// compile-smoke placeholder (`Baseline() => 1`, `PrimitiveComposed() => 1`)
// rather than a second strategy to reconcile. [GlobalSetup] hoists the
// workload values, but the list itself is rebuilt fresh inside the benchmark
// method rather than cached, because the strategy mutates in place - a
// cached list would only be valid for the first measured iteration (same
// shape RotateListBenchmarks already uses). The rebuild is timed on purpose
// because the strategy mutates its input.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal,
// so a public [Benchmark] method cannot name it as a return type (CS0050).
//
// The node at position i holds i + 1, wrapping back to 1 after LC 143's highest value
// of 1,000 - so the smaller list holds 1..1000 once, and the larger, at LC 143's
// 5 * 10^4-node cap, cycles through them.
public class ReorderListBenchmarks
{
    private const int MaxNodeValue = 1_000;

    private int[] _values = [];

    [Params(1_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = Enumerable.Range(0, Length).Select(position => (position % MaxNodeValue) + 1).ToArray();

    [Benchmark(Baseline = true)]
    public object? ReverseAndMergeInPlace()
    {
        var head = LeetCodeWireFormat.ToLinkedList(_values);
        ReorderListSolution.ReorderByReverseAndMergeInPlace(head);
        return head;
    }
}
