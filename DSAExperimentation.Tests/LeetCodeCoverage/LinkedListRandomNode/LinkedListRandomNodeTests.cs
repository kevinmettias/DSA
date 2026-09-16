using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.LinkedListRandomNode;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LinkedListRandomNode;

// Harness only: both strategies live in LinkedListRandomNodeSolution.
// SinglyLinkedListNode<int> is internal, so it cannot appear in a public
// TheoryData<...> member (CS0053) - the same reason BinarySearchTreeIteratorTests
// and BalancedBinaryTreeTests use one [Fact] per case rather than [Theory]. Each
// scenario is asserted against both strategies so a failure names which one broke;
// GetRandomByDynamicArrayCache is exercised through its LeetCode-shaped overload,
// which itself delegates to the prepared-cache overload (CacheValues + the
// DynamicArray<int> overload), so both are covered transitively - the same
// pattern OpenTheLockTests uses for MinTurnsByReduceGraph. The repeated-draw
// cases share one assertion helper, which takes the strategy as the named
// IRandomDraw type below rather than as a bare Func.
public sealed class LinkedListRandomNodeTests
{
    [Fact]
    public void GetRandomByReservoirSampling_SingleNodeList_AlwaysReturnsThatValue()
    {
        var head = new SinglyLinkedListNode<int>(42);
        var random = new Random(1);

        for (var i = 0; i < 20; i++)
        {
            var value = LinkedListRandomNodeSolution.GetRandomByReservoirSampling(head, random);

            Assert.Equal(42, value);
        }
    }

    [Fact]
    public void GetRandomByReservoirSampling_MultiNodeList_EventuallyReturnsEveryValue()
        => AssertEventuallyReturnsEveryValue(new ReservoirSamplingDraw());

    [Fact]
    public void GetRandomByDynamicArrayCache_SingleNodeList_AlwaysReturnsThatValue()
    {
        var head = new SinglyLinkedListNode<int>(42);
        var random = new Random(1);

        for (var i = 0; i < 20; i++)
        {
            var value = LinkedListRandomNodeSolution.GetRandomByDynamicArrayCache(head, random);

            Assert.Equal(42, value);
        }
    }

    [Fact]
    public void GetRandomByDynamicArrayCache_MultiNodeList_EventuallyReturnsEveryValue()
        => AssertEventuallyReturnsEveryValue(new DynamicArrayCacheDraw());

    // The two strategies differ only in how one draw is taken, so the sampling shape
    // - a three-node list, drawn from many times - is written once and handed the
    // strategy under test.
    private static void AssertEventuallyReturnsEveryValue(IRandomDraw draw)
    {
        var third = new SinglyLinkedListNode<int>(3);
        var second = new SinglyLinkedListNode<int>(2) { Next = third };
        var head = new SinglyLinkedListNode<int>(1) { Next = second };
        var random = new Random(1);

        var seen = new HashSet<int>();
        for (var i = 0; i < 200; i++)
        {
            var value = draw.Draw(head, random);
            Assert.True(value is 1 or 2 or 3);
            seen.Add(value);
        }

        Assert.Equal(3, seen.Count);
    }

    // One drawing strategy: given the list and a source of randomness, return the value of
    // one node chosen from it. The shared assertion helper above used to hand the strategy
    // over as a bare delegate, which named neither the decision nor its inputs; this type
    // states in one named method what a draw is, and is the place the contract - a value
    // belonging to the list it was handed - is written down. Nested because both it and its
    // two implementations are only ever used inside this test class.
    private interface IRandomDraw
    {
        int Draw(SinglyLinkedListNode<int> head, Random random);
    }

    // GetRandomByReservoirSampling, as one draw from the list.
    private sealed class ReservoirSamplingDraw : IRandomDraw
    {
        public int Draw(SinglyLinkedListNode<int> head, Random random) =>
            LinkedListRandomNodeSolution.GetRandomByReservoirSampling(head, random);
    }

    // GetRandomByDynamicArrayCache, the same draw answered from the cached value array.
    private sealed class DynamicArrayCacheDraw : IRandomDraw
    {
        public int Draw(SinglyLinkedListNode<int> head, Random random) =>
            LinkedListRandomNodeSolution.GetRandomByDynamicArrayCache(head, random);
    }
}
