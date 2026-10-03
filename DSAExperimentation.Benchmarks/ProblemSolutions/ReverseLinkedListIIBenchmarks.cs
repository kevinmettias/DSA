using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.ReverseLinkedListII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseLinkedListIISolution's, the same methods
// ReverseLinkedListIISolutionTests proves correct. [GlobalSetup] hoists the workload
// values, but the list itself is rebuilt fresh inside each benchmark method
// rather than cached, because the head-insertion strategy mutates the list it is
// handed - a cached list would only be valid for the first measured iteration. The
// rebuild is timed on purpose, and ArrayRebuild, which only reads its list, pays it
// too so both arms carry the same cost.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
public class ReverseLinkedListIIBenchmarks
{
    private const int QuarterLengthDivisor = 4;
    private const int ThreeQuarterLengthNumerator = 3;

    private int[] _values = [];

    [Params(200, 500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        ReverseLinkedListIISolution.ReverseBetweenByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(_values),
            Length / QuarterLengthDivisor,
            Length * ThreeQuarterLengthNumerator / QuarterLengthDivisor);

    [Benchmark]
    public object? HeadInsertion() =>
        ReverseLinkedListIISolution.ReverseBetweenByHeadInsertion(
            LeetCodeWireFormat.ToLinkedList(_values),
            Length / QuarterLengthDivisor,
            Length * ThreeQuarterLengthNumerator / QuarterLengthDivisor);
}
