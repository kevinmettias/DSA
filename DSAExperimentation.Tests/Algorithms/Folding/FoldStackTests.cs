using DSAExperimentation.Algorithms.Folding;

namespace DSAExperimentation.Tests.Algorithms.Folding;

public sealed partial class FoldStackTests
{
    [Fact]
    public void Top_IsHowManyResultsArePushed()
    {
        var stack = new FoldStack<int>();
        stack.Push(7);
        stack.Push(8);

        Assert.Equal(2, stack.Top);
    }

    // A node's children's results are the ones pushed since the top it noted before folding them.
    [Fact]
    public void Since_Start_ReturnsTheResultsPushedSinceInOrder()
        => Assert.Equal([2, 3], PushedSince(before: [1], since: [2, 3]));

    // Past the initial capacity the array grows, and every earlier result survives the copy.
    [Fact]
    public void Push_PastCapacity_KeepsEveryResult()
    {
        var stack = new FoldStack<int>();

        foreach (var value in Enumerable.Range(0, 100))
        {
            stack.Push(value);
        }

        Assert.Equal(Enumerable.Range(0, 100), stack.Since(0).ToArray());
    }

    [Fact]
    public void PopTo_Start_DropsTheResultsPushedSince()
        => Assert.Equal([1], LeftAfterPopping(before: [1], since: [2, 3]));

    private static int[] PushedSince(int[] before, int[] since)
    {
        var stack = StackOf(before);
        var start = stack.Top;
        PushAll(stack, since);

        return stack.Since(start).ToArray();
    }

    private static int[] LeftAfterPopping(int[] before, int[] since)
    {
        var stack = StackOf(before);
        var start = stack.Top;
        PushAll(stack, since);
        stack.PopTo(start);

        return stack.Since(0).ToArray();
    }

    private static FoldStack<int> StackOf(int[] values)
    {
        var stack = new FoldStack<int>();
        PushAll(stack, values);

        return stack;
    }

    private static void PushAll(FoldStack<int> stack, int[] values)
    {
        foreach (var value in values)
        {
            stack.Push(value);
        }
    }
}
