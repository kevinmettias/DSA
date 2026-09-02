using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InsertGreatestCommonDivisorsInLinkedList;

// LeetCode 2807. Insert Greatest Common Divisors in Linked List: pointer rewiring over
// this repo's mutable SinglyLinkedListNode<T> (SwapNodesInPairsTests' precedent) - walk
// the list one link at a time and splice a freshly allocated node holding gcd(current,
// next) between them, then continue past both. Reducing two integers via the Euclidean
// algorithm is the same private helper FindGreatestCommonDivisorOfArrayTests already
// reuses inline rather than promoting to a shared production type: no repo container or
// algorithm primitive applies to two-integer GCD itself.
public sealed partial class InsertGreatestCommonDivisorsInLinkedListTests
{
    [Theory]
    [InlineData(new[] { 18, 6, 10, 3 }, new[] { 18, 6, 6, 2, 10, 1, 3 })]
    [InlineData(new[] { 7 }, new[] { 7 })]
    public void InsertGreatestCommonDivisors_LeetCodeExamples_InsertsGcdBetweenEveryPair(
        int[] values, int[] expected)
        => Assert.Equal(expected, ToArray(InsertGreatestCommonDivisors(BuildList(values))));

    [Fact]
    public void InsertGreatestCommonDivisors_CoprimeAdjacentValues_InsertsOne()
        => Assert.Equal([8, 1, 9], ToArray(InsertGreatestCommonDivisors(BuildList([8, 9]))));

    private static SinglyLinkedListNode<int>? InsertGreatestCommonDivisors(SinglyLinkedListNode<int>? head)
    {
        var current = head;

        while (current?.Next is not null)
        {
            var gcdNode = new SinglyLinkedListNode<int>(Gcd(current.Value, current.Next.Value)) { Next = current.Next };
            current.Next = gcdNode;
            current = gcdNode.Next;
        }

        return head;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

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
