using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ReverseLinkedListII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ReverseLinkedListIISolution's, the same methods
// ReverseLinkedListIITests proves correct. [GlobalSetup] hoists the workload
// values, but the list itself is rebuilt fresh inside each benchmark method
// rather than cached, because the head-insertion strategy mutates the list it is
// handed - a cached list would only be valid for the first measured iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class ReverseLinkedListIIBenchmarks
{
    private const int QuarterLengthDivisor = 4;
    private const int ThreeQuarterLengthNumerator = 3;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        ReverseLinkedListIISolution.ReverseBetweenByArrayRebuild(
            BuildList(_values), Length / QuarterLengthDivisor, Length * ThreeQuarterLengthNumerator / QuarterLengthDivisor);

    [Benchmark]
    public object? HeadInsertion() =>
        ReverseLinkedListIISolution.ReverseBetweenByHeadInsertion(
            BuildList(_values), Length / QuarterLengthDivisor, Length * ThreeQuarterLengthNumerator / QuarterLengthDivisor);

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }
}
