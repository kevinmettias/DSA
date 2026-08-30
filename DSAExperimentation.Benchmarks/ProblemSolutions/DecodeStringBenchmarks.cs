using System.Text;
using BenchmarkDotNet.Attributes;
using RepoCountStack = DSAExperimentation.DataStructures.Stack.Stack<int>;
using RepoBuilderStack = DSAExperimentation.DataStructures.Stack.Stack<System.Text.StringBuilder>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Decode String (LC 394): a recursive-descent decoder relying on the CLR's own call
// stack (no repo primitive - the same "lighter repo-primitive fit"
// BasicCalculatorBenchmarks' RecursiveDescent baseline already accepts) vs. a
// single left-to-right pass over two of this repo's own Stack<T> instances -
// Stack<int> for pending repeat counts, Stack<StringBuilder> for each enclosing
// scope's partial result. The generated input chains many *sequential*, only
// single-level-nested "2[ab]" groups, so recursion depth stays constant as Length
// grows instead of risking a StackOverflowException - the same shape
// BasicCalculatorBenchmarks already uses.
[MemoryDiagnoser]
public class DecodeStringBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _encoded = null!;

    [GlobalSetup]
    public void Setup() => _encoded = BuildEncoded(Length);

    [Benchmark(Baseline = true)]
    public string RecursiveDescent()
    {
        var i = 0;
        return DecodeRecursive(_encoded, ref i).ToString();
    }

    [Benchmark]
    public string StackScan()
    {
        var counts = new RepoCountStack();
        var builders = new RepoBuilderStack();
        var current = new StringBuilder();
        var number = 0;

        foreach (var c in _encoded)
        {
            if (char.IsDigit(c))
            {
                number = (number * 10) + (c - '0');
            }
            else if (c == '[')
            {
                counts.Push(number);
                builders.Push(current);
                current = new StringBuilder();
                number = 0;
            }
            else if (c == ']')
            {
                counts.TryPop(out var repeatCount);
                builders.TryPop(out var outer);

                for (var r = 0; r < repeatCount; r++)
                {
                    outer.Append(current);
                }

                current = outer;
            }
            else
            {
                current.Append(c);
            }
        }

        return current.ToString();
    }

    private static StringBuilder DecodeRecursive(string s, ref int i)
    {
        var builder = new StringBuilder();

        while (i < s.Length && s[i] != ']')
        {
            if (char.IsDigit(s[i]))
            {
                var number = 0;

                while (char.IsDigit(s[i]))
                {
                    number = (number * 10) + (s[i] - '0');
                    i++;
                }

                i++; // skip '['
                var inner = DecodeRecursive(s, ref i);
                i++; // skip ']'

                for (var r = 0; r < number; r++)
                {
                    builder.Append(inner);
                }
            }
            else
            {
                builder.Append(s[i]);
                i++;
            }
        }

        return builder;
    }

    private static string BuildEncoded(int length)
    {
        var builder = new StringBuilder();

        while (builder.Length < length)
        {
            builder.Append("2[ab]");
        }

        return builder.ToString();
    }
}
