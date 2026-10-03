using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.RemoveLinkedListElements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveLinkedListElementsSolution's, the same
// methods RemoveLinkedListElementsSolutionTests proves correct. [GlobalSetup] hoists the
// workload values, but the list itself is rebuilt fresh inside each benchmark
// method rather than cached, because the dummy-head-splice strategy splices nodes
// out of the list it is handed - a cached list would only be valid for the first
// measured iteration. The rebuild is timed on purpose, and ArrayRebuild, which only
// reads its list, pays it too so both arms carry the same cost.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
public class RemoveLinkedListElementsBenchmarks
{
    private const int TargetValue = 0;
    private const int NonTargetValue = 1;
    private const int MatchRunLength = 3;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _values = Enumerable.Range(0, Length)
            .Select(index => IsTargetValue(index) ? TargetValue : NonTargetValue)
            .ToArray();

    // Every run of TargetValue is MatchRunLength long, so the value at an index is
    // the target when the index starts a run.
    private static bool IsTargetValue(int index) => index % MatchRunLength == 0;

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        RemoveLinkedListElementsSolution.RemoveElementsByArrayRebuild(
            LeetCodeWireFormat.ToLinkedList(_values), TargetValue);

    [Benchmark]
    public object? DummyHeadSplice() =>
        RemoveLinkedListElementsSolution.RemoveElementsByDummyHeadSplice(
            LeetCodeWireFormat.ToLinkedList(_values), TargetValue);
}
