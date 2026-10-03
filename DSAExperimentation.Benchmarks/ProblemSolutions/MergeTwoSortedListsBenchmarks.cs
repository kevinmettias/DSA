using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;
using DSAExperimentation.LeetCode.MergeTwoSortedLists;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MergeTwoSortedListsSolution's, the same methods
// MergeTwoSortedListsSolutionTests proves correct. Both arms relink the nodes they are handed, so the
// lists are rebuilt from the value arrays inside each measurement rather than being merged a
// second time on nodes an earlier iteration already spliced. Each list stops at LC 21's
// own bound of 50 nodes, with values drawn from its [-100, 100].
public class MergeTwoSortedListsBenchmarks
{
    private const int RandomSeed = 21; // LC problem number
    private const int MinNodeValue = -100;
    private const int MaxNodeValue = 100;

    private int[] _first = [];
    private int[] _second = [];

    [Params(5, 50)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _first = SeededDraws.Values(Length, MinNodeValue, MaxNodeValue + 1, random).Order().ToArray();
        _second = SeededDraws.Values(Length, MinNodeValue, MaxNodeValue + 1, random).Order().ToArray();
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
