namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3577. Complexity[0] is fixed to the minimum
// possible value and every other entry is drawn strictly above it, so every
// generated instance is solvable and the backtracking arm explores its full,
// uncollapsed (n-1)! search tree rather than pruning out early.
internal static class ComputerUnlockingWorkloads
{
    private const int MinComplexity = 1;
    private const int MaxComplexityBound = 1_000;

    public static int[] BuildSolvable(int computerCount, int seed)
    {
        var random = new Random(seed);
        var complexity = new int[computerCount];
        complexity[0] = MinComplexity;

        for (var i = 1; i < computerCount; i++)
        {
            complexity[i] = random.Next(MinComplexity + 1, MaxComplexityBound);
        }

        return complexity;
    }
}
