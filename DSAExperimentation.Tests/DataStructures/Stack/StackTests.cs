using DsaStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.DataStructures.Stack;

public sealed partial class StackTests
{
    [Fact]
    public void Push_TryPop_ReturnsValuesInLifoOrder()
    {
        var stack = new DsaStack();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);

        stack.TryPop(out var first);
        stack.TryPop(out var second);
        stack.TryPop(out var third);

        Assert.Equal(3, first);
        Assert.Equal(2, second);
        Assert.Equal(1, third);
    }

    [Fact]
    public void TryPeek_NonEmptyStack_ReturnsTrueAndDoesNotRemoveElement()
    {
        var stack = new DsaStack();
        stack.Push(1);
        stack.Push(2);

        var first = stack.TryPeek(out var firstValue);
        var second = stack.TryPeek(out var secondValue);

        Assert.True(first);
        Assert.True(second);
        Assert.Equal(2, firstValue);
        Assert.Equal(2, secondValue);
        Assert.Equal(2, stack.Count);
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

        stack.TryPop(out _);

        Assert.Equal(1, stack.Count);
    }
}
