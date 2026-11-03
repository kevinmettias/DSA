using IntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.Solutions.EvaluateReversePolishNotation;

// LeetCode 150. Evaluate Reverse Polish Notation: evaluate an arithmetic
// expression given in postfix (reverse Polish) form.
//
// Pre-migration the test's private helper was the only real implementation - the
// benchmark's two [Benchmark] arms were untested placeholders (`=> 1`), not a second
// arm to reconcile. Both strategies are that one evaluation: operands are pushed as
// they are read; each operator pops its two operands, applies itself, and pushes the
// result back, so the final stack holds exactly the expression's value. They differ
// only in the stack: the BCL's Stack<int>, or this repo's own.
internal static class EvaluateReversePolishNotationSolution
{
    // The textbook answer: the operand stack on the BCL's Stack<int>, the arm this repo's
    // stack is measured against (ARCHITECTURE 17.5).
    public static int EvaluateByBclOperandStack(string[] tokens)
    {
        var stack = new Stack<int>();

        foreach (var token in tokens)
        {
            if (int.TryParse(token, out var operand))
            {
                stack.Push(operand);
                continue;
            }

            var right = stack.Pop();
            var left = stack.Pop();
            var value = Apply(token, left, right);
            stack.Push(value);
        }

        return stack.Pop();
    }

    public static int EvaluateByOperandStack(string[] tokens)
    {
        var stack = new IntStack();

        foreach (var token in tokens)
        {
            if (int.TryParse(token, out var operand))
            {
                stack.Push(operand);
                continue;
            }

            stack.TryPop(out var right);
            stack.TryPop(out var left);
            var value = Apply(token, left, right);
            stack.Push(value);
        }

        stack.TryPop(out var result);
        return result;
    }

    private static int Apply(string op, int left, int right) => op switch
    {
        "+" => left + right,
        "-" => left - right,
        "*" => left * right,
        _ => left / right,
    };
}
