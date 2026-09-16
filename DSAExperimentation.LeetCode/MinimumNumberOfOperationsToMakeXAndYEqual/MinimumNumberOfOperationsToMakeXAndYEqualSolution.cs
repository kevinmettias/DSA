using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MinimumNumberOfOperationsToMakeXAndYEqual;

// LeetCode 2998. Minimum Number of Operations to Make X and Y Equal: from startValue,
// one operation is /11 (if divisible), /5 (if divisible), -1, or +1; find the fewest
// operations to reach targetValue.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class MinimumNumberOfOperationsToMakeXAndYEqualSolution
{
    // Headroom above Math.Max(startValue, targetValue) the mutation-queue search is
    // allowed to explore - generous relative to the +-10 a single "round up to the
    // next multiple of 5 or 11" step ever needs, since every divide only shrinks the
    // value further.
    private const int SearchPadding = 50;

    // The textbook BFS: BCL Queue + HashSet, generating each of startValue's (at most)
    // 4 candidate operations on the fly within a bounded range - the arm the memoized
    // recurrence below has to justify itself against.
    public static int MinOperationsByMutationQueue(int startValue, int targetValue)
    {
        if (startValue == targetValue)
        {
            return 0;
        }

        var bound = Math.Max(startValue, targetValue) + SearchPadding;
        var visited = new HashSet<int> { startValue };
        var queue = new Queue<(int Value, int Operations)>();
        queue.Enqueue((startValue, 0));

        return FewestOperationsInFrontier(queue, visited, bound, targetValue)
            ?? throw new InvalidOperationException(
                "targetValue is unreachable from startValue within the search bound");
    }

    // The BFS itself: every dequeued value's candidate operations are explored, the first
    // one equal to targetValue ends the search, and a value not yet visited joins the
    // frontier one operation deeper. Null when the frontier drains first - a case the
    // padding bound above leaves unreachable, so the caller treats it as a broken
    // precondition.
    private static int? FewestOperationsInFrontier(
        Queue<(int Value, int Operations)> queue, HashSet<int> visited, int bound, int targetValue)
    {
        while (queue.Count > 0)
        {
            var (value, operations) = queue.Dequeue();

            foreach (var neighbor in CandidateOperations(value, bound))
            {
                if (neighbor == targetValue)
                {
                    return operations + 1;
                }

                if (visited.Add(neighbor))
                {
                    queue.Enqueue((neighbor, operations + 1));
                }
            }
        }

        return null;
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

    // This repo's own Memoizer: the state is just the current value (targetValue is
    // closed over), and the recurrence at each value tries "walk straight down to
    // targetValue" plus "round to the nearest multiple of 11 (or 5) from either side,
    // then divide" - the same tuple/int-state Memoizer.Memoize<TState,TResult>
    // composition CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo and
    // NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo already prove
    // out for unrelated counting recurrences.
    public static int MinOperationsByMemoizedReduce(int startValue, int targetValue) =>
        Memoizer.Memoize(startValue, new OperationsFromValue(targetValue));

    // The recurrence, as a named type, over the value still to work down towards
    // targetValue: walk straight down to it, or round to a multiple of 11 (or 5) from
    // either side and divide, whichever costs fewer operations in total.
    private sealed class OperationsFromValue(int targetValue) : IRecurrence<int, int>
    {
        public int Replay(int value, IRecurrence<int, int> rest)
        {
            if (value <= targetValue)
            {
                return targetValue - value;
            }

            var best = value - targetValue; // decrement straight down, no divide at all

            var viaEleven = OperationsViaDivisor(value, 11, rest);
            best = Math.Min(best, viaEleven);
            var viaFive = OperationsViaDivisor(value, 5, rest);
            best = Math.Min(best, viaFive);

            return best;
        }

        private int OperationsViaDivisor(int value, int divisor, IRecurrence<int, int> rest)
        {
            var remainder = value % divisor;
            var down = value - remainder;
            var costDown = remainder + 1 + rest.Replay(down / divisor, rest);

            if (remainder == 0)
            {
                return costDown;
            }

            var up = down + divisor;
            var costUp = (divisor - remainder) + 1 + rest.Replay(up / divisor, rest);

            return Math.Min(costDown, costUp);
        }
    }
}
