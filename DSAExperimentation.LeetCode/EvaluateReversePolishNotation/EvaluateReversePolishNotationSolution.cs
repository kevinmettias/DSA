using IntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.EvaluateReversePolishNotation;

// LeetCode 150. Evaluate Reverse Polish Notation: evaluate an arithmetic
// expression given in postfix (reverse Polish) form.
//
// There is exactly one strategy: pre-migration the test's private helper was
// the only real implementation - the benchmark's two [Benchmark] arms were
// untested placeholders (`=> 1`), not a second arm to reconcile. Operands are
// pushed onto this repo's own Stack<int> as they are read; each operator pops
// its two operands, applies itself, and pushes the result back, so the final
// stack holds exactly the expression's value.
internal static class EvaluateReversePolishNotationSolution
{
    public static int EvalByOperandStack(string[] tokens)
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
            stack.Push(Apply(token, left, right));
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
