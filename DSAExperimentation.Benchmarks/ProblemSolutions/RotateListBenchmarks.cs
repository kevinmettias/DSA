using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.RotateList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RotateListSolution's, the same methods
// RotateListTests proves correct. [GlobalSetup] hoists the workload values, but the
// list itself is rebuilt fresh inside each benchmark method rather than cached,
// because the pointer-rewire strategy mutates the list it is handed - a cached
// list would only be valid for the first measured iteration.
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so a
// public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class RotateListBenchmarks
{
    // Both benchmarks rotate by roughly a third of the list so they do equivalent work.
    private const int RotationDivisor = 3;

    [Params(200, 5_000)] public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public object? ArrayRebuild() =>
        RotateListSolution.RotateRightByArrayRebuild(BuildList(_values), Length / RotationDivisor);

    [Benchmark]
    public object? PointerRewire() =>
        RotateListSolution.RotateRightByPointerRewire(BuildList(_values), Length / RotationDivisor);

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
