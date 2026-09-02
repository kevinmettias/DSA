using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.DeleteNodesFromLinkedListPresentInArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeleteNodesFromLinkedListPresentInArraySolution's,
// the same methods DeleteNodesFromLinkedListPresentInArrayTests proves correct.
// Set construction is hoisted to [GlobalSetup] via SetFilter's prepared-input
// overload; the list itself is rebuilt inside each [Benchmark] call rather than
// shared, because both strategies splice .Next pointers in place - reusing one
// pre-built list across iterations would let the first iteration's deletions
// silently make every later iteration measure an already-filtered list.
[MemoryDiagnoser]
public class DeleteNodesFromLinkedListPresentInArrayBenchmarks
{
    private const int RandomSeed = 3217;
    private const int DeletedValueFraction = 3;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int[] _headValues = null!;
    private Set<int> _numsSet = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _headValues = Enumerable.Range(0, Length).Select(_ => random.Next(1, Length + 1)).ToArray();
        _nums = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).Take(Length / DeletedValueFraction).ToArray();
        _numsSet = new Set<int>(_nums);
    }

    // Returns object, not SinglyLinkedListNode<int> - the node type is internal,
    // so a public [Benchmark] method cannot name it as a return type (CS0050).
    [Benchmark(Baseline = true)]
    public object? ArrayScan() =>
        DeleteNodesFromLinkedListPresentInArraySolution.ModifiedListByArrayScan(_nums, BuildList(_headValues));

    [Benchmark]
    public object? SetFilter() =>
        DeleteNodesFromLinkedListPresentInArraySolution.ModifiedListBySetFilter(_numsSet, BuildList(_headValues));

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        for (var i = 1; i < values.Length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[i]);
            tail = tail.Next;
        }

        return head;
    }
}
