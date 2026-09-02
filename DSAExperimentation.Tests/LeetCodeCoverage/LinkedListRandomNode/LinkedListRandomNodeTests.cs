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
// pattern OpenTheLockTests uses for MinTurnsByReduceGraph.
public sealed class LinkedListRandomNodeTests
{
    [Fact]
    public void GetRandomByReservoirSampling_SingleNodeList_AlwaysReturnsThatValue()
    {
        var head = new SinglyLinkedListNode<int>(42);
        var random = new Random(1);

        for (var i = 0; i < 20; i++)
        {
            Assert.Equal(42, LinkedListRandomNodeSolution.GetRandomByReservoirSampling(head, random));
        }
    }

    [Fact]
    public void GetRandomByReservoirSampling_MultiNodeList_EventuallyReturnsEveryValue()
    {
        var third = new SinglyLinkedListNode<int>(3);
        var second = new SinglyLinkedListNode<int>(2) { Next = third };
        var head = new SinglyLinkedListNode<int>(1) { Next = second };
        var random = new Random(1);

        var seen = new HashSet<int>();
        for (var i = 0; i < 200; i++)
        {
            var value = LinkedListRandomNodeSolution.GetRandomByReservoirSampling(head, random);
            Assert.True(value is 1 or 2 or 3);
            seen.Add(value);
        }

        Assert.Equal(3, seen.Count);
    }

    [Fact]
    public void GetRandomByDynamicArrayCache_SingleNodeList_AlwaysReturnsThatValue()
    {
        var head = new SinglyLinkedListNode<int>(42);
        var random = new Random(1);

        for (var i = 0; i < 20; i++)
        {
            Assert.Equal(42, LinkedListRandomNodeSolution.GetRandomByDynamicArrayCache(head, random));
        }
    }

    [Fact]
    public void GetRandomByDynamicArrayCache_MultiNodeList_EventuallyReturnsEveryValue()
    {
        var third = new SinglyLinkedListNode<int>(3);
        var second = new SinglyLinkedListNode<int>(2) { Next = third };
        var head = new SinglyLinkedListNode<int>(1) { Next = second };
        var random = new Random(1);

        var seen = new HashSet<int>();
        for (var i = 0; i < 200; i++)
        {
            var value = LinkedListRandomNodeSolution.GetRandomByDynamicArrayCache(head, random);
            Assert.True(value is 1 or 2 or 3);
            seen.Add(value);
        }

        Assert.Equal(3, seen.Count);
    }
}
