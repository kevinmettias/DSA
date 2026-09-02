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
    private const string DoubleOp = "D";
    private const string SumOp = "+";
    private const int DoublingMultiplier = 2;
    private const int SecondToLastOffset = 2;
    private const int OpCyclePeriod = 5;
    private const int DoubleOpRemainder = 2;
    private const int SumOpRemainder = 3;
    private const int MaxBaseScore = 50;

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
            top = ApplyOperation(op, record, top);
        }

        return SumRecord(record, top);
    }

    private static int ApplyOperation(string op, int[] record, int top)
    {
        switch (op)
        {
            case DoubleOp:
                record[top] = record[top - 1] * DoublingMultiplier;
                return top + 1;
            case SumOp:
                record[top] = record[top - 1] + record[top - SecondToLastOffset];
                return top + 1;
            default:
                record[top] = int.Parse(op);
                return top + 1;
        }
    }

    private static int SumRecord(int[] record, int count)
    {
        var total = 0;

        for (var i = 0; i < count; i++)
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
            ApplyStackOperation(op, record);
        }

        return DrainStackTotal(record);
    }

    private static void ApplyStackOperation(string op, RepoIntStack record)
    {
        switch (op)
        {
            case DoubleOp:
                record.TryPeek(out var last);
                record.Push(last * DoublingMultiplier);
                break;
            case SumOp:
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

    private static int DrainStackTotal(RepoIntStack record)
    {
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
            ops[i] = (i % OpCyclePeriod) switch
            {
                DoubleOpRemainder => DoubleOp,
                SumOpRemainder => SumOp,
                _ => ((i % MaxBaseScore) + 1).ToString(),
            };
        }

        return ops;
    }
}
