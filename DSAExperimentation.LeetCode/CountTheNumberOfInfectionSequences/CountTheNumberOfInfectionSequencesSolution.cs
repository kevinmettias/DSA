using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountTheNumberOfInfectionSequences;

// LeetCode 2954. Count the Number of Infection Sequences: n children stand in a
// line, sick lists the ones already infected (sorted, at least one, not all).
// Every second exactly one susceptible child adjacent to an already-infected one
// becomes infected; the answer is the number of distinct orders the remaining
// children can end up infected in, modulo 1e9+7.
//
// The sick children split the line into runs of susceptible children: the run
// before the first sick child and the run after the last can only ever be
// infected from their one infected end, so each has exactly one possible internal
// order. A run strictly between two sick children can be infected from either
// end at every step but the last, giving 2^(length-1) possible internal orders.
// The n - sick.Length infection moves overall then interleave across every run
// (edge runs included) by a multinomial coefficient - the same
// "(size-1)! / product(part sizes!)" shape RoomWaysAlgebra folds bottom-up over a
// tree, computed here directly over the runs of a line instead.
internal static class CountTheNumberOfInfectionSequencesSolution
{
    // Textbook baseline: explicitly enumerate every legal infection order by
    // backtracking one child at a time - at each step, infect one child adjacent
    // to the current infected set, recurse, then undo - and count the completed
    // sequences. Correct but combinatorial in the run lengths, which is exactly
    // what the closed-form strategy below has to be measured against.
    public static long CountSequencesByBruteForceSimulation(int n, int[] sick)
    {
        var infected = new bool[n];

        foreach (var index in sick)
        {
            infected[index] = true;
        }

        return CountCompletions(infected);
    }

    private static long CountCompletions(bool[] infected)
    {
        var frontier = FindFrontier(infected);

        if (frontier.Count == 0)
        {
            return 1;
        }

        var total = 0L;

        foreach (var candidate in frontier)
        {
            infected[candidate] = true;
            total = (total + CountCompletions(infected)) % ModularArithmetic.Modulo;
            infected[candidate] = false;
        }

        return total;
    }

    private static List<int> FindFrontier(bool[] infected)
    {
        var frontier = new List<int>();

        for (var i = 0; i < infected.Length; i++)
        {
            if (!infected[i] && HasInfectedNeighbor(infected, i))
            {
                frontier.Add(i);
            }
        }

        return frontier;
    }

    private static bool HasInfectedNeighbor(bool[] infected, int index) =>
        (index > 0 && infected[index - 1]) || (index < infected.Length - 1 && infected[index + 1]);

    // One O(n) scan measures every run between (and around) the sick children,
    // then a single factorial/inverse-factorial table build - the same
    // RoomWaysPrecomputedFactorialAlgebra/CountAnagramsByModularFactorial shape -
    // turns the whole answer into O(n) more work with no recursion or enumeration.
    public static long CountSequencesByGapCombinatorics(int n, int[] sick)
    {
        var (runLengths, interiorRunLengths, totalMoves) = MeasureRuns(n, sick);
        var (factorial, inverseFactorial) = BuildFactorialTable(totalMoves);

        var answer = factorial[totalMoves];

        foreach (var length in runLengths)
        {
            answer = answer * inverseFactorial[length] % ModularArithmetic.Modulo;
        }

        foreach (var length in interiorRunLengths)
        {
            answer = answer * ModularArithmetic.Power(2, length - 1) % ModularArithmetic.Modulo;
        }

        return answer;
    }

    private static (List<int> RunLengths, List<int> InteriorRunLengths, int TotalMoves) MeasureRuns(int n, int[] sick)
    {
        var runLengths = new List<int> { sick[0] };
        var interiorRunLengths = new List<int>();

        for (var i = 1; i < sick.Length; i++)
        {
            var length = sick[i] - sick[i - 1] - 1;
            runLengths.Add(length);

            if (length > 0)
            {
                interiorRunLengths.Add(length);
            }
        }

        runLengths.Add(n - 1 - sick[^1]);

        return (runLengths, interiorRunLengths, n - sick.Length);
    }

    private static (long[] Factorial, long[] InverseFactorial) BuildFactorialTable(int maxSize)
    {
        var factorial = new long[maxSize + 1];
        var inverseFactorial = new long[maxSize + 1];
        factorial[0] = 1;

        for (var i = 1; i <= maxSize; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        inverseFactorial[maxSize] = ModularArithmetic.Inverse(factorial[maxSize]);

        for (var i = maxSize - 1; i >= 0; i--)
        {
            inverseFactorial[i] = inverseFactorial[i + 1] * (i + 1) % ModularArithmetic.Modulo;
        }

        return (factorial, inverseFactorial);
    }
}
