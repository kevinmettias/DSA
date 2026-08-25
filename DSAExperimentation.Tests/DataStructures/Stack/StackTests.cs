using DsaStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.DataStructures.Stack;

public sealed partial class StackTests
{
    [Fact]
    public void Push_Pop_ReturnsValuesInLifoOrder()
    {
        var stack = new DsaStack();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        Assert.Equal(3, stack.Pop());
        Assert.Equal(2, stack.Pop());
        Assert.Equal(1, stack.Pop());
    }

    [Fact]
    public void Peek_DoesNotRemoveElement()
    {
        var stack = new DsaStack();
        stack.Push(1);
        stack.Push(2);

        Assert.Equal(2, stack.Peek());
        Assert.Equal(2, stack.Peek());
        Assert.Equal(2, stack.Count);
    }

    [Fact]
    public void Peek_EmptyStack_ThrowsInvalidOperationException()
    {
        var stack = new DsaStack();

        Assert.Throws<InvalidOperationException>(() => stack.Peek());
    }

    [Fact]
    public void Pop_EmptyStack_ThrowsInvalidOperationException()
    {
        var stack = new DsaStack();

        Assert.Throws<InvalidOperationException>(() => stack.Pop());
    }

    [Fact]
    public void TryPeek_EmptyStack_ReturnsFalse()
    {
        var stack = new DsaStack();

        Assert.False(stack.TryPeek(out _));
    }

    [Fact]
    public void TryPop_EmptyStack_ReturnsFalse()
    {
        var stack = new DsaStack();

        Assert.False(stack.TryPop(out _));
    }

    [Fact]
    public void Count_ReflectsPushesAndPops()
    {
        var stack = new DsaStack();

        Assert.Equal(0, stack.Count);

        stack.Push(1);
        stack.Push(2);

        Assert.Equal(2, stack.Count);

        stack.Pop();

        Assert.Equal(1, stack.Count);
    }
}
