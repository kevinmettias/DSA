using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RotateList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RotateListSolution's, the same methods
// RotateListSolutionTests proves correct. [GlobalSetup] hoists the workload values, but the
// list itself is rebuilt fresh inside each benchmark method rather than cached,
// because the pointer-rewire strategy mutates the list it is handed - a cached
// list would only be valid for the first measured iteration. The rebuild is timed
// on purpose, and ArrayRebuild, which only reads its list, pays it too so both arms
// carry the same cost.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
//
// Length stops at LC 61's 500 nodes. The node at position i holds i - 100, wrapping
// back to -100 after 100, so every value stays inside LC 61's [-100, 100].
public class RotateListBenchmarks
{
    // Both benchmarks rotate by roughly a third of the list so they do equivalent work.
    private const int RotationDivisor = 3;
    private const int MinNodeValue = -100;
    private const int NodeValueCount = 201;

    private int[] _values = [];

    [Params(200, 500)] public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = Enumerable.Range(0, Length).Select(position => MinNodeValue + (position % NodeValueCount)).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        RotateListSolution.RotateRightByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(_values), Length / RotationDivisor);

    [Benchmark]
    public object? PointerRewire() =>
        RotateListSolution.RotateRightByPointerRewire(
            LeetCodeWireFormat.ToLinkedList(_values), Length / RotationDivisor);
}
