using BclIntStack = System.Collections.Generic.Stack<int>;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.ValidateStackSequences;

// LeetCode 946. Validate Stack Sequences: could some interleaving of pushes and pops
// of `pushed` produce exactly `popped`?
//
// The two strategies are the two ways to answer that:
//
// - IsValidByBacktrackingSearch treats every moment as a choice - "pop now" or "push
//   the next value" - and undoes a branch that fails, O(2^n) worst case. It is the
//   baseline, so its internals are a plain BCL stack and recursion.
// - IsValidByGreedyStackSweep pushes onto this repo's own Stack<int> and drains it
//   whenever the top matches the value `popped` expects next - the same "push, then
//   drain whatever matches" idiom NextGreaterElementI uses this Stack<int> for, just
//   draining against a target sequence instead of a monotonic condition. Greedy
//   popping is never wrong here: once the top equals the value popped expects next,
//   nothing pushed later can ever be needed before it, so there is never anything to
//   undo and one O(n) sweep settles it.
internal static class ValidateStackSequencesSolution
{
    public static bool IsValidByBacktrackingSearch(int[] pushed, int[] popped) =>
        new BacktrackingMatch(pushed, popped).TryMatch(0, 0, new BclIntStack());

    public static bool IsValidByGreedyStackSweep(int[] pushed, int[] popped)
    {
        var stack = new RepoIntStack();
        var popIndex = 0;

        foreach (var value in pushed)
        {
            stack.Push(value);
            popIndex = DrainMatchingTop(stack, popped, popIndex);
        }

        return popIndex == popped.Length;
    }

    private static int DrainMatchingTop(RepoIntStack stack, int[] popped, int popIndex)
    {
        while (IsTopTheNextExpected(stack, popped, popIndex))
        {
            stack.TryPop(out _);
            popIndex++;
        }

        return popIndex;
    }

    // There is still an expected value to match, and the stack's top is it.
    private static bool IsTopTheNextExpected(RepoIntStack stack, int[] popped, int popIndex)
        => popIndex < popped.Length && stack.TryPeek(out var top) && top == popped[popIndex];

    // Exhaustive push/pop-timing search: at each state try popping first, and fall
    // back to pushing the next value, restoring the stack on a failed branch.
    private sealed class BacktrackingMatch(int[] pushed, int[] popped)
    {
        private readonly int[] _pushed = pushed;
        private readonly int[] _popped = popped;

        public bool TryMatch(int pushIndex, int popIndex, BclIntStack stack) =>
            popIndex == _popped.Length ||
            TryPopBranch(pushIndex, popIndex, stack) ||
            TryPushBranch(pushIndex, popIndex, stack);

        private bool TryPopBranch(int pushIndex, int popIndex, BclIntStack stack)
        {
            if (stack.Count == 0 || stack.Peek() != _popped[popIndex])
            {
                return false;
            }

            return TryMatchAfterPop(pushIndex, popIndex, stack);
        }

        // Pop the matching top, explore from there, and put it back when that branch
        // turned out to be a dead end.
        private bool TryMatchAfterPop(int pushIndex, int popIndex, BclIntStack stack)
        {
            var top = stack.Pop();

            if (TryMatch(pushIndex, popIndex + 1, stack))
            {
                return true;
            }

            stack.Push(top);
            return false;
        }

        private bool TryPushBranch(int pushIndex, int popIndex, BclIntStack stack)
        {
            if (pushIndex >= _pushed.Length)
            {
                return false;
            }

            stack.Push(_pushed[pushIndex]);

            if (TryMatch(pushIndex + 1, popIndex, stack))
            {
                return true;
            }

            stack.Pop();
            return false;
        }
    }
}
