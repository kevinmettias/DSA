using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode;
using DSAExperimentation.LeetCode.Solutions.SortList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortListSolution's, the same methods
// SortListSolutionTests proves correct - the BCL's Array.Sort, and this repo's
// MergeSort, between the same flatten and rebuild. Pre-migration this class was an
// untested compile-smoke placeholder (`Baseline() => 1`, `PrimitiveComposed() => 1`).
// Each strategy only reads the list it is handed (it rebuilds a fresh one from the sorted array rather than
// relinking in place), so - unlike InsertionSortListBenchmarks -
// [GlobalSetup] can build the list once and every iteration reuses it, mirroring
// ConvertSortedListToBinarySearchTreeBenchmarks.
public class SortListBenchmarks
{
    private const int ShuffleSeed = 148;

    private SinglyLinkedListNode<int>? _head;

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

        _head = LeetCodeWireFormat.ToLinkedList(values);
    }

    [Benchmark(Baseline = true)]
    public SinglyLinkedListNode<int>? ArraySort() => SortListSolution.SortByArraySort(_head);

    [Benchmark]
    public SinglyLinkedListNode<int>? MergeSortOverSequence() => SortListSolution.SortByMergeSortOverSequence(_head);
}
