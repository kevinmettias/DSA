using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveNthNodeFromEndOfList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveNthNodeFromEndOfListSolution's, the same
// methods RemoveNthNodeFromEndOfListSolutionTests proves correct. [GlobalSetup] hoists the
// workload values, but the list itself is rebuilt fresh inside each benchmark
// method rather than cached, because both strategies mutate/replace it - a cached
// list would only be valid for the first measured iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
public class RemoveNthNodeFromEndOfListBenchmarks
{
    // Both benchmarks remove the node at roughly the middle position so they do
    // equivalent work; this divisor picks that middle position from Length.
    private const int MiddlePositionDivisor = 2;

    private int[] _values = [];
    [Params(200, 5_000)] public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        RemoveNthNodeFromEndOfListSolution.RemoveByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(_values), Length / MiddlePositionDivisor);

    [Benchmark]
    public object? TwoRunner() =>
        RemoveNthNodeFromEndOfListSolution.RemoveByTwoRunner(
            LeetCodeWireFormat.ToLinkedList(_values), Length / MiddlePositionDivisor);
}
