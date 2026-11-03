using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Harness.Tests;

// Every benchmark's arms are compared through these renderings, so what they tell apart - and what
// they deliberately do not - decides whether "the arms agree" means anything. Each test below
// pairs answers that must render alike with ones that must not.
public sealed partial class AnswerGraphTextTests
{
    [Fact]
    public void Of_ArrayAndListOfTheSameValues_RenderAlike() =>
        Assert.Equal(AnswerGraphText.Of(new[] { 1, 2, 3 }), AnswerGraphText.Of(new List<int> { 1, 2, 3 }));

    // Assert.Equal on two jagged arrays compares the inner arrays by reference; the rendering
    // compares them by content, and still tells a different row apart.
    [Fact]
    public void Of_JaggedArrays_CompareByContentRowByRow()
    {
        int[][] first = [[1, 2], [3]];
        int[][] same = [[1, 2], [3]];
        int[][] different = [[1], [2, 3]];

        Assert.Equal(AnswerGraphText.Of(first), AnswerGraphText.Of(same));
        Assert.NotEqual(AnswerGraphText.Of(first), AnswerGraphText.Of(different));
    }

    // Object graphs render by their fields, so two lists built separately with the same values
    // render alike, and one value changed anywhere along them renders differently.
    [Fact]
    public void Of_LinkedLists_CompareByEveryNodesValue()
    {
        var rendered = AnswerGraphText.Of(LeetCodeWireFormat.ToLinkedList([1, 2, 3]));

        Assert.Equal(rendered, AnswerGraphText.Of(LeetCodeWireFormat.ToLinkedList([1, 2, 3])));
        Assert.NotEqual(rendered, AnswerGraphText.Of(LeetCodeWireFormat.ToLinkedList([1, 2, 4])));
    }

    // A cycle is rendered as a back-reference to where it rejoins, so a list whose tail rejoins
    // the head renders differently from one whose tail rejoins the middle, and both terminate.
    [Fact]
    public void Of_CyclicLists_TellApartWhereTheCycleRejoins()
    {
        var toHead = LeetCodeWireFormat.ToLinkedList([1, 2, 3])!;
        TailOf(toHead).Next = toHead;
        var toMiddle = LeetCodeWireFormat.ToLinkedList([1, 2, 3])!;
        TailOf(toMiddle).Next = toMiddle.Next;

        Assert.NotEqual(AnswerGraphText.Of(toHead), AnswerGraphText.Of(toMiddle));
    }

    // Arms that order the same floating-point operations differently differ in the last bits;
    // twelve significant digits absorb that and nothing coarser.
    [Fact]
    public void Of_Doubles_AgreeToTwelveSignificantDigits()
    {
        Assert.Equal(AnswerGraphText.Of(0.1 + 0.2), AnswerGraphText.Of(0.3));
        Assert.NotEqual(AnswerGraphText.Of(0.3), AnswerGraphText.Of(0.3000001));
    }

    [Fact]
    public void OfUnordered_SameElementsInAnotherOrder_RenderAlike()
    {
        int[][] first = [[1, 2], [3, 4]];
        int[][] reordered = [[3, 4], [1, 2]];
        int[][] innerReordered = [[2, 1], [3, 4]];

        Assert.Equal(AnswerGraphText.OfUnordered(first), AnswerGraphText.OfUnordered(reordered));
        Assert.NotEqual(AnswerGraphText.OfUnordered(first), AnswerGraphText.OfUnordered(innerReordered));
    }

    private static SinglyLinkedListNode<int> TailOf(SinglyLinkedListNode<int> head)
    {
        var node = head;

        while (node.Next is not null)
        {
            node = node.Next;
        }

        return node;
    }

    // Walk is private to AnswerGraphText: each Step renders one pending item from a stack of its own
    // rather than recursing. So Step is driven through Of, its only caller.
    public sealed partial class WalkTests
    {
        // Each node of a list renders one level deeper than the last, so a walk that recursed per level
        // would overflow the thread's stack long before the deepest.
        private const int DeeperThanAThreadsStack = 100_000;

        [Fact]
        public void Step_ListDeeperThanAThreadsStack_RendersItsDeepestNode()
        {
            var values = Enumerable.Range(0, DeeperThanAThreadsStack).ToArray();
            int[] lastChanged = [.. values[..^1], -1];
            var rendered = AnswerGraphText.Of(LeetCodeWireFormat.ToLinkedList(values));
            var renderedWithLastChanged = AnswerGraphText.Of(LeetCodeWireFormat.ToLinkedList(lastChanged));

            Assert.NotEqual(rendered, renderedWithLastChanged);
        }
    }
}
