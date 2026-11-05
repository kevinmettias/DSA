namespace DSAExperimentation.LeetCode.Solutions.ImplementQueueUsingStacks;

// LeetCode's empty call on the queue, which IStackQueue answers as IsEmpty, the predicate's name its sibling
// design problems use too (ARCHITECTURE 17.13).
internal static class ImplementQueueUsingStacksCalls
{
    public static bool Empty(ImplementQueueUsingStacksSolution.IStackQueue queue) => queue.IsEmpty();
}
