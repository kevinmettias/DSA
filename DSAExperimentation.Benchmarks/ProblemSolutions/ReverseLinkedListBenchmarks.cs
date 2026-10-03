using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ReverseLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ReverseLinkedListSolution's, the same method
// ReverseLinkedListSolutionTests proves correct. [GlobalSetup] hoists the workload
// values, but the list itself is rebuilt fresh inside the benchmark method
// rather than cached, because the strategy mutates the list it is handed - a
// cached list would only be valid for the first measured iteration (mirrors
// ReverseLinkedListIIBenchmarks' identical constraint).
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal,
// so a public [Benchmark] method cannot name it as a return type (CS0050).
public class ReverseLinkedListBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark]
    public object? IterativeRewire() =>
        ReverseLinkedListSolution.ReverseListByIterativeRewire(LeetCodeWireFormat.ToLinkedList(_values));
}
