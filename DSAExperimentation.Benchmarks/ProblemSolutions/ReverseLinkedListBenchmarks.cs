using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Solutions.Conventions;
using DSAExperimentation.LeetCode.Solutions.ReverseLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is ReverseLinkedListSolution's, the same method
// ReverseLinkedListSolutionTests proves correct. [GlobalSetup] hoists the workload
// values, but the list itself is rebuilt fresh inside the benchmark method
// rather than cached, because the strategy mutates the list it is handed - a
// cached list would only be valid for the first measured iteration (mirrors
// ReverseLinkedListIIBenchmarks' identical constraint). The rebuild is timed on
// purpose because the strategy mutates its input.
public class ReverseLinkedListBenchmarks
{
    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark]
    public SinglyLinkedListNode<int>? IterativeRewire() =>
        ReverseLinkedListSolution.ReverseListByIterativeRewire(LeetCodeWireFormat.ToLinkedList(_values));
}
