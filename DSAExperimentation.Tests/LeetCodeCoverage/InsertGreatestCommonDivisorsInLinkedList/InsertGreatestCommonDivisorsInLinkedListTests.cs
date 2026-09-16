using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.InsertGreatestCommonDivisorsInLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertGreatestCommonDivisorsInLinkedList;

// Harness only. Both strategies are
// InsertGreatestCommonDivisorsInLinkedListSolution's - including the value
// rebuild, which the benchmark used to own privately as its baseline and nothing
// asserted. Beyond LeetCode's two published examples the cases pin the walk's
// boundaries: coprime neighbours (the inserted value is 1), equal neighbours (the
// inserted value is the value itself), and a divisor chain where consecutive gaps
// insert different divisors.
public sealed partial class InsertGreatestCommonDivisorsInLinkedListTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // LC example 1.
            { [18, 6, 10, 3], [18, 6, 6, 2, 10, 1, 3] },

            // LC example 2: a single node has no gap to insert into.
            { [7], [7] },

            // Coprime neighbours.
            { [8, 9], [8, 1, 9] },

            // Equal neighbours: gcd(v, v) is v.
            { [4, 4], [4, 4, 4] },

            // A divisor chain, so the two gaps insert different values.
            { [12, 8, 4], [12, 4, 8, 4, 4] },

            // One value divides the next throughout.
            { [2, 4, 8, 16], [2, 2, 4, 4, 8, 8, 16] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void InsertGreatestCommonDivisorsByValueRebuild_LeetCodeExamples_InsertsGcdBetweenEveryPair(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(InsertGreatestCommonDivisorsInLinkedListSolution.InsertGreatestCommonDivisorsByValueRebuild(
                BuildList(values))));

    [Theory]
    [MemberData(nameof(Examples))]
    public void InsertGreatestCommonDivisorsByNodeSplice_LeetCodeExamples_InsertsGcdBetweenEveryPair(
        int[] values, int[] expected) =>
        Assert.Equal(
            expected,
            ToArray(InsertGreatestCommonDivisorsInLinkedListSolution.InsertGreatestCommonDivisorsByNodeSplice(
                BuildList(values))));

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

    private static int[] ToArray(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values.ToArray();
    }
}
