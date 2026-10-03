using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.IntersectionOfTwoLinkedLists;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is IntersectionOfTwoLinkedListsSolution's own
// two-pointer walk, the same method IntersectionOfTwoLinkedListsSolutionTests proves
// correct. Chain construction is charged to [GlobalSetup], not to the walk
// being measured.
public class IntersectionOfTwoLinkedListsBenchmarks
{
    private SinglyLinkedListNode<int> _headA = null!;

    private SinglyLinkedListNode<int> _headB = null!;
    [Params(100, 5_000)]
    public int PrefixLength { get; set; }

    [GlobalSetup]
    public void Setup() =>
        (_headA, _headB) = IntersectionOfTwoLinkedListsWorkloads.Build(PrefixLength);

    // Returns the node the walk found, as object because SinglyLinkedListNode<T>
    // is internal and a public [Benchmark] method cannot name it (CS0050).
    [Benchmark]
    public object? TwoPointerWalk() =>
        IntersectionOfTwoLinkedListsSolution.GetIntersectionNodeByTwoPointerWalk(_headA, _headB);
}
