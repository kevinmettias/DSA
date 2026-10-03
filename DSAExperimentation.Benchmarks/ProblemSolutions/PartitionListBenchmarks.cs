using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.PartitionList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PartitionListSolution's, the same methods
// PartitionListSolutionTests proves correct. [GlobalSetup] hoists the workload values, but
// the list itself is rebuilt fresh inside each benchmark method rather than cached,
// because the splice strategy mutates the list it is handed - a cached list would
// only be valid for the first measured iteration. The rebuild is timed on purpose,
// and the array arm, which only reads its list, pays it too so both arms carry the
// same cost.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
//
// The values count down through zero - from Length / 2 - 1 to -Length / 2, so 99 to -100
// at LC 86's 200-node cap, inside its [-100, 100] - and the partition value is zero,
// their midpoint, so half the list moves ahead of the other half.
public class PartitionListBenchmarks
{
    private const int PartitionMidpointDivisor = 2;
    private const int PartitionValue = 0;

    private int[] _values = [];

    [Params(20, 200)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = Enumerable.Range(-Length / PartitionMidpointDivisor, Length).Reverse().ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        PartitionListSolution.PartitionByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(_values), PartitionValue);

    [Benchmark]
    public object? PointerSplice() =>
        PartitionListSolution.PartitionByPointerSplice(
            LeetCodeWireFormat.ToLinkedList(_values), PartitionValue);
}
