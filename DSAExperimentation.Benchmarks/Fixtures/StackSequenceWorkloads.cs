namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 946 - how long a push/pop pair to validate, and
// the seed that decides the interleaving. The pop order is produced by simulating a
// random but genuinely legal sequence of pushes and pops, so the exhaustive
// backtracking arm is forced through a real search instead of failing fast on an
// early mismatch.
internal static class StackSequenceWorkloads
{
    private const int BranchChoiceCount = 2;

    public static int[] BuildPushed(int length) => [.. Enumerable.Range(0, length)];

    public static int[] BuildValidPopOrder(int[] pushed, int seed)
    {
        var random = new Random(seed);
        var stack = new Stack<int>();
        var popped = new List<int>();
        var pushIndex = 0;

        while (popped.Count < pushed.Length)
        {
            if (ShouldPush(pushIndex, pushed.Length, stack.Count, random))
            {
                stack.Push(pushed[pushIndex]);
                pushIndex++;
            }
            else
            {
                popped.Add(stack.Pop());
            }
        }

        return [.. popped];
    }

    // The next step is a push whenever one is still possible, and when a pop is also
    // possible the coin decides between them. The draw stays behind the push check so
    // a spent input consumes no randomness.
    private static bool ShouldPush(int pushIndex, int pushedLength, int stackCount, Random random)
        => pushIndex < pushedLength && (stackCount == 0 || random.Next(BranchChoiceCount) == 0);
}
