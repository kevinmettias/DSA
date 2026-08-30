using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidateStackSequences;

// LeetCode 946. Validate Stack Sequences: greedily push from `pushed` onto this
// repo's own Stack<int>, draining it whenever its top matches the next value
// `popped` expects - the same "push, then drain whatever matches" idiom
// NextGreaterElementITests already uses this Stack<int> for, just draining against a
// target sequence instead of a monotonic condition. Greedy popping is always safe
// here: once the top equals the value popped expects next, there is never a reason
// to delay popping it, since nothing pushed later can ever be needed before it.
public sealed partial class ValidateStackSequencesTests
{
    [Fact]
    public void ValidateStackSequences_MatchesGreedyPopOrder_ReturnsTrue()
    {
        int[] pushed = [1, 2, 3, 4, 5];
        int[] popped = [4, 5, 3, 2, 1];

        var valid = Validate(pushed, popped);

        Assert.True(valid);
    }

    [Fact]
    public void ValidateStackSequences_PopOrderUnreachableFromAnyInterleaving_ReturnsFalse()
    {
        int[] pushed = [1, 2, 3, 4, 5];
        int[] popped = [4, 3, 5, 1, 2];

        var valid = Validate(pushed, popped);

        Assert.False(valid);
    }

    private static bool Validate(int[] pushed, int[] popped)
    {
        var stack = new RepoIntStack();
        var popIndex = 0;

        foreach (var value in pushed)
        {
            stack.Push(value);

            while (stack.TryPeek(out var top) && popIndex < popped.Length && top == popped[popIndex])
            {
                stack.TryPop(out _);
                popIndex++;
            }
        }

        return popIndex == popped.Length;
    }
}
