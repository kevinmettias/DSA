using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Baseball Game (LC 682): a hand-rolled array plus a manually-tracked top-of-record
// index (the "obvious" approach before reaching for a stack abstraction) vs. this
// repo's own Stack<int> (ValidParenthesesBenchmarks/NextGreaterElementIBenchmarks
// precedent) driving the same push/peek/pop replay of the round's record. _ops never
// emits "C" so both strategies grow monotonically - the array baseline needs no
// underflow guard to stay a fair, equivalent comparison against Stack<int>'s TryPop.
[MemoryDiagnoser]
public class BaseballGameBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string[] _ops = null!;

    [GlobalSetup]
    public void Setup() => _ops = BuildOperations(Length);

    [Benchmark(Baseline = true)]
    public int ManualArrayCursor()
    {
        var record = new int[_ops.Length];
        var top = 0;

        foreach (var op in _ops)
        {
            switch (op)
            {
                case "D":
                    record[top] = record[top - 1] * 2;
                    top++;
                    break;
                case "+":
                    record[top] = record[top - 1] + record[top - 2];
                    top++;
                    break;
                default:
                    record[top] = int.Parse(op);
                    top++;
                    break;
            }
        }

        var total = 0;

        for (var i = 0; i < top; i++)
        {
            total += record[i];
        }

        return total;
    }

    [Benchmark]
    public int StackReplay()
    {
        var record = new RepoIntStack();

        foreach (var op in _ops)
        {
            switch (op)
            {
                case "D":
                    record.TryPeek(out var last);
                    record.Push(last * 2);
                    break;
                case "+":
                    record.TryPop(out var top);
                    record.TryPop(out var second);
                    var sum = top + second;
                    record.Push(second);
                    record.Push(top);
                    record.Push(sum);
                    break;
                default:
                    record.Push(int.Parse(op));
                    break;
            }
        }

        var total = 0;

        while (record.TryPop(out var value))
        {
            total += value;
        }

        return total;
    }

    private static string[] BuildOperations(int length)
    {
        var ops = new string[length];

        for (var i = 0; i < length; i++)
        {
            ops[i] = (i % 5) switch
            {
                2 => "D",
                3 => "+",
                _ => ((i % 50) + 1).ToString(),
            };
        }

        return ops;
    }
}
