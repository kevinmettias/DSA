using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RotateList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RotateListSolution's, the same methods
// RotateListSolutionTests proves correct. [GlobalSetup] hoists the workload values, but the
// list itself is rebuilt fresh inside each benchmark method rather than cached,
// because the pointer-rewire strategy mutates the list it is handed - a cached
// list would only be valid for the first measured iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
public class RotateListBenchmarks
{
    // Both benchmarks rotate by roughly a third of the list so they do equivalent work.
    private const int RotationDivisor = 3;

    private int[] _values = [];

    [Params(200, 5_000)] public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        RotateListSolution.RotateRightByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(_values), Length / RotationDivisor);

    [Benchmark]
    public object? PointerRewire() =>
        RotateListSolution.RotateRightByPointerRewire(
            LeetCodeWireFormat.ToLinkedList(_values), Length / RotationDivisor);
}
