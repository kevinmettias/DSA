using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeXAndYEqual;

// LeetCode 2998. Minimum Number of Operations to Make X and Y Equal: from x, one
// operation is /11 (if divisible), /5 (if divisible), -1, or +1; find the fewest
// operations to reach y.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class MinimumNumberOfOperationsToMakeXAndYEqualSolution
{
    // Headroom above max(x, y) the mutation-queue search is allowed to explore -
    // generous relative to the +-10 a single "round up to the next multiple of 5
    // or 11" step ever needs, since every divide only shrinks the value further.
    private const int SearchPadding = 50;

    // The textbook BFS: BCL Queue + HashSet, generating each of x's (at most) 4
    // candidate operations on the fly within a bounded range - the arm the
    // memoized recurrence below has to justify itself against.
    public static int MinOperationsByMutationQueue(int x, int y)
    {
        if (x == y)
        {
            return 0;
        }

        var bound = Math.Max(x, y) + SearchPadding;
        var visited = new HashSet<int> { x };
        var queue = new Queue<(int Value, int Operations)>();
        queue.Enqueue((x, 0));

        while (queue.Count > 0)
        {
            var (value, operations) = queue.Dequeue();

            foreach (var neighbor in CandidateOperations(value, bound))
            {
                if (neighbor == y)
                {
                    return operations + 1;
                }

                if (visited.Add(neighbor))
                {
                    queue.Enqueue((neighbor, operations + 1));
                }
            }
        }

        throw new InvalidOperationException("y is unreachable from x within the search bound");
    }

    private static IEnumerable<int> CandidateOperations(int value, int bound)
    {
        if (value % 11 == 0)
        {
            yield return value / 11;
        }

        if (value % 5 == 0)
        {
            yield return value / 5;
        }

        if (value + 1 <= bound)
        {
            yield return value + 1;
        }

        if (value - 1 >= 0)
        {
            yield return value - 1;
        }
    }

    // This repo's own Memoizer: the state is just the current value (y is closed
    // over), and the recurrence at each value tries "walk straight down to y" plus
    // "round to the nearest multiple of 11 (or 5) from either side, then divide" -
    // the same tuple/int-state Memoizer.Memoize<TState,TResult> composition
    // CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo and
    // NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo already prove
    // out for unrelated counting recurrences.
    public static int MinOperationsByMemoizedReduce(int x, int y) =>
        Memoizer.Memoize<int, int>(x, (value, recurse) => OperationsFrom(value, y, recurse));

    private static int OperationsFrom(int value, int y, Func<int, int> recurse)
    {
        if (value <= y)
        {
            return y - value;
        }

        var best = value - y; // decrement straight down, no divide at all

        best = Math.Min(best, OperationsViaDivisor(value, y, 11, recurse));
        best = Math.Min(best, OperationsViaDivisor(value, y, 5, recurse));

        return best;
    }

    private static int OperationsViaDivisor(int value, int y, int divisor, Func<int, int> recurse)
    {
        var remainder = value % divisor;
        var down = value - remainder;
        var costDown = remainder + 1 + recurse(down / divisor);

        if (remainder == 0)
        {
            return costDown;
        }

        var up = down + divisor;
        var costUp = (divisor - remainder) + 1 + recurse(up / divisor);

        return Math.Min(costDown, costUp);
    }
}
