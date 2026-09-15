using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.ExpressionAddOperators;

// LeetCode 282. Expression Add Operators: every way to interleave +, -, * between
// num's digits (no leading zeros in a multi-digit operand) so the resulting
// expression evaluates to target.
//
// Both strategies explore the identical implicit graph of partial expressions -
// ExprState carries the expression built so far, so no two distinct operand/operator
// choices can ever produce an equal node - the difference is only whether the walk is
// a hand-rolled recursive backtrack or DepthFirstSearch.Traverse's engine.
internal static class ExpressionAddOperatorsSolution
{
    // The textbook recursive backtrack: BCL recursion and string concatenation, the
    // arm the composed DFS-engine strategy below has to justify itself against.
    public static List<string> AddOperatorsByBacktracking(string num, int target)
    {
        var results = new List<string>();
        Backtrack(new ExprProblem(num, target), new ExprState(0, 0, 0, string.Empty), results);
        return results;
    }

    // DepthFirstSearch.Traverse (Algorithms.Traversal.DepthFirst) is already "collect
    // every node reachable from a root via an arbitrary successor function" - exactly
    // ARCHITECTURE.md's own "chess position, generated on demand" illustration for
    // this primitive - so the whole enumeration is one call; this strategy only
    // supplies the successor function and filters the results down to target matches.
    public static List<string> AddOperatorsByDepthFirstSearchTraverse(string num, int target)
    {
        var start = new ExprState(0, 0, 0, string.Empty);
        var visited = DepthFirstSearch.Traverse(start, state => Successors(num, state));

        return visited
            .Where(state => state.Position == num.Length && state.Value == target)
            .Select(state => state.Expression)
            .ToList();
    }

    private static IEnumerable<ExprState> Successors(string num, ExprState state)
    {
        if (state.Position == num.Length)
        {
            yield break;
        }

        for (var length = 1; state.Position + length <= num.Length; length++)
        {
            var operand = num.Substring(state.Position, length);
            if (length > 1 && operand[0] == '0')
            {
                // A longer operand starting here would carry the same leading zero.
                yield break;
            }

            foreach (var candidate in CandidatesForOperand(state, operand))
            {
                yield return candidate;
            }
        }
    }

    private static IEnumerable<ExprState> CandidatesForOperand(ExprState state, string operand)
    {
        var operandValue = long.Parse(operand);
        var nextPosition = state.Position + operand.Length;

        if (state.Position == 0)
        {
            // The first operand has no operator in front of it.
            yield return new ExprState(nextPosition, operandValue, operandValue, operand);
            yield break;
        }

        yield return new ExprState(nextPosition, state.Value + operandValue, operandValue, $"{state.Expression}+{operand}");
        yield return new ExprState(nextPosition, state.Value - operandValue, -operandValue, $"{state.Expression}-{operand}");
        yield return new ExprState(
            nextPosition,
            state.Value - state.LastOperand + (state.LastOperand * operandValue),
            state.LastOperand * operandValue,
            $"{state.Expression}*{operand}");
    }

    private static void Backtrack(ExprProblem problem, ExprState state, List<string> results)
    {
        if (state.Position == problem.Num.Length)
        {
            if (state.Value == problem.Target)
            {
                results.Add(state.Expression);
            }

            return;
        }

        for (var length = 1; state.Position + length <= problem.Num.Length; length++)
        {
            var operand = problem.Num.Substring(state.Position, length);
            if (length > 1 && operand[0] == '0')
            {
                // A longer operand starting here would carry the same leading zero.
                break;
            }

            BacktrackForOperand(problem, state, operand, results);
        }
    }

    private static void BacktrackForOperand(
        ExprProblem problem, ExprState state, string operand, List<string> results)
    {
        var operandValue = long.Parse(operand);
        var nextPosition = state.Position + operand.Length;

        if (state.Position == 0)
        {
            // The first operand has no operator in front of it.
            Backtrack(problem, new ExprState(nextPosition, operandValue, operandValue, operand), results);
            return;
        }

        Backtrack(problem, new ExprState(
            nextPosition, state.Value + operandValue, operandValue, $"{state.Expression}+{operand}"), results);
        Backtrack(problem, new ExprState(
            nextPosition, state.Value - operandValue, -operandValue, $"{state.Expression}-{operand}"), results);
        Backtrack(
            problem,
            new ExprState(
                nextPosition,
                state.Value - state.LastOperand + (state.LastOperand * operandValue),
                state.LastOperand * operandValue,
                $"{state.Expression}*{operand}"),
            results);
    }

    // Expression carries the full path so far: it guarantees every node is distinct,
    // so Traverse's visited-set guard never merges two different operand/operator
    // choices and this stays a true full enumeration.
    private readonly record struct ExprState(int Position, long Value, long LastOperand, string Expression);

    // The two values that never change as the search descends - the digit string being
    // split and the value the expression has to reach. ExprState above is where the
    // search currently stands; this is the problem it is walking.
    private readonly record struct ExprProblem(string Num, int Target);
}
