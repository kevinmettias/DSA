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
            var canPush = pushIndex < pushed.Length;
            var canPop = stack.Count > 0;

            if (canPush && (!canPop || random.Next(BranchChoiceCount) == 0))
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
}
