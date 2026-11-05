using DSAExperimentation.LeetCode.Solutions.ImplementQueueUsingStacks;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ImplementQueueUsingStacks;

public sealed partial class ImplementQueueUsingStacksCallsTests
{
    [Fact]
    public void Empty_NewQueue_IsTrue() => Assert.True(ImplementQueueUsingStacksCalls.Empty(ImplementQueueUsingStacksSolution.CreateByTwoBclStacks()));

    [Fact]
    public void Empty_QueueHoldingAValue_IsFalse()
    {
        var queue = ImplementQueueUsingStacksSolution.CreateByTwoBclStacks();
        queue.Push(1);

        Assert.False(ImplementQueueUsingStacksCalls.Empty(queue));
    }
}
