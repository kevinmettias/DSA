using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.InsertionSortList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the single arm is InsertionSortListSolution's, the same method
// InsertionSortListTests proves correct. Pre-migration this class was an
// untested compile-smoke placeholder (`Baseline() => 1`, `PrimitiveComposed() =>
// 1`) rather than a second strategy to reconcile. [GlobalSetup] hoists the
// shuffled workload values, but the list itself is rebuilt fresh inside the
// benchmark method rather than cached, because insertion sort relinks the input
// list's own nodes in place - a cached list would only be valid for the first
// measured iteration (same shape RotateListBenchmarks already uses).
//
// Returns object, not SinglyLinkedListNode<int> - the node type is internal, so
// a public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class InsertionSortListBenchmarks
{
    private const int ShuffleSeed = 147;

    private int[] _values = [];

    [Params(100, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(1, Length).ToArray();
        var random = new Random(ShuffleSeed);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _values = values;
    }

    [Benchmark(Baseline = true)]
    public object? DummyHeadInsertion() =>
        InsertionSortListSolution.SortByDummyHeadInsertion(BuildList(_values));

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
