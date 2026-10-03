using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MergeTwoSortedLists;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeTwoSortedListsSolution's, the same methods
// MergeTwoSortedListsTests proves correct. Both arms relink the nodes they are handed, so the
// lists are rebuilt from the value arrays inside each measurement rather than being merged a
// second time on nodes an earlier iteration already spliced.
[MemoryDiagnoser]
public class MergeTwoSortedListsBenchmarks
{
    private const int RandomSeed = 21; // LC problem number

    private int[] _first = [];
    private int[] _second = [];

    [Params(500, 1000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _first = Enumerable.Range(0, Length).Select(_ => random.Next(1_000_000)).OrderBy(value => value).ToArray();
        _second = Enumerable.Range(0, Length).Select(_ => random.Next(1_000_000)).OrderBy(value => value).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] RecursiveSelection() => MergeWith(MergeTwoSortedListsSolution.MergeByRecursiveSelection);

    [Benchmark]
    public int[] DummyHeadSplice() => MergeWith(MergeTwoSortedListsSolution.MergeByDummyHeadSplice);

    private int[] MergeWith(MergeStrategy merge)
    {
        var merged = merge(
            LeetCodeWireFormat.ToLinkedList(_first),
            LeetCodeWireFormat.ToLinkedList(_second));

        return LeetCodeWireFormat.FromLinkedList(merged);
    }

    private delegate SinglyLinkedListNode<int>? MergeStrategy(
        SinglyLinkedListNode<int>? first, SinglyLinkedListNode<int>? second);
}
