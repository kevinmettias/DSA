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
    private const int DecimalBase = 10;
    private const string EncodedTile = "2[ab]";

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
        var state = new ScanState();
        return RunScan(_encoded, state).ToString();
    }

    private static StringBuilder RunScan(string encoded, ScanState state)
    {
        foreach (var c in encoded)
        {
            if (char.IsDigit(c))
            {
                state.Number = (state.Number * DecimalBase) + (c - '0');
            }
            else if (c == '[')
            {
                PushGroup(state);
            }
            else if (c == ']')
            {
                PopGroup(state);
            }
            else
            {
                state.Current.Append(c);
            }
        }

        return state.Current;
    }

    private static void PushGroup(ScanState state)
    {
        state.Counts.Push(state.Number);
        state.Builders.Push(state.Current);
        state.Current = new StringBuilder();
        state.Number = 0;
    }

    private static void PopGroup(ScanState state)
    {
        state.Counts.TryPop(out var repeatCount);
        state.Builders.TryPop(out var outer);

        for (var r = 0; r < repeatCount; r++)
        {
            outer.Append(state.Current);
        }

        state.Current = outer;
    }

    private sealed class ScanState
    {
        public RepoCountStack Counts { get; } = new();
        public RepoBuilderStack Builders { get; } = new();
        public StringBuilder Current { get; set; } = new();
        public int Number { get; set; }
    }

    private static StringBuilder DecodeRecursive(string s, ref int i)
    {
        var builder = new StringBuilder();

        while (i < s.Length && s[i] != ']')
        {
            if (char.IsDigit(s[i]))
            {
                AppendRepeatedGroup(s, ref i, builder);
            }
            else
            {
                builder.Append(s[i]);
                i++;
            }
        }

        return builder;
    }

    // The digit branch from DecodeRecursive's scan loop: parses the repeat count,
    // recurses into the bracketed group, then tiles the decoded inner text that many
    // times onto the caller's builder.
    private static void AppendRepeatedGroup(string s, ref int i, StringBuilder builder)
    {
        var number = 0;

        while (char.IsDigit(s[i]))
        {
            number = (number * DecimalBase) + (s[i] - '0');
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

    private static string BuildEncoded(int length)
    {
        var builder = new StringBuilder();

        while (builder.Length < length)
        {
            builder.Append(EncodedTile);
        }

        return builder.ToString();
    }
}
