using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.MergeTwoSortedLists;

// Shape under test: a REPO DATA STRUCTURE on both sides of the call. The solution
// takes and returns SinglyLinkedListNode<int>, which is neither JSON-renderable
// nor comparable by value; the registration keeps the array form on the outside -
// so the case reads exactly like LeetCode's own example - and converts at the
// boundary. That is what keeps the failure message legible: "[1,1,2,3,4,4]", not
// a node's type name.
internal sealed class MergeTwoSortedListsRegistration : ILeetCodeProblemRegistration
{
    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<(int[] First, int[] Second), int[]>("merge-two-sorted-lists")
            .Strategy("DummyHeadSplice", Merge)
            .MatchingAnswersWith(LeetCodeAnswers.SequenceEqual)
            .Case("example-1", ([1, 2, 4], [1, 3, 4]), [1, 1, 2, 3, 4, 4])
            .Case("example-2", ([], []), [])
            .Case("example-3", ([], [0]), [0])

            // An empty FIRST list with a non-trivial second one: the dummy-head
            // splice has to fall straight through to the second list's head
            // without ever advancing the first cursor.
            .Case("first-empty-second-populated", ([], [5, 6]), [5, 6])
            .Case("one-side-entirely-precedes-the-other", ([1, 2, 3], [7, 8, 9]), [1, 2, 3, 7, 8, 9])
            .Workload(
                "interleaved-2000",
                ([.. Enumerable.Range(0, 2000).Select(value => value * 2)],
                 [.. Enumerable.Range(0, 2000).Select(value => (value * 2) + 1)]))
            .Build();

    private static int[] Merge((int[] First, int[] Second) input)
    {
        var first = LeetCodeWireFormat.ToLinkedList(input.First);
        var second = LeetCodeWireFormat.ToLinkedList(input.Second);
        var merged = MergeTwoSortedListsSolution.MergeByDummyHeadSplice(first, second);

        return LeetCodeWireFormat.FromLinkedList(merged);
    }
}
