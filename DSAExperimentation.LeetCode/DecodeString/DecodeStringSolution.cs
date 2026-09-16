using System.Text;
using RepoCountStack = DSAExperimentation.DataStructures.Stack.Stack<int>;
using RepoBuilderStack = DSAExperimentation.DataStructures.Stack.Stack<System.Text.StringBuilder>;

namespace DSAExperimentation.LeetCode.DecodeString;

// LeetCode 394. Decode String: expand a run-length-encoded string like
// "3[a2[c]]" into "accaccacc".
//
// The two strategies differ only in what they use to track the enclosing
// scopes at each '[': the CLR's own call stack (recursive descent, no repo
// primitive) or two of this repo's own Stack<T> instances - Stack<int> for
// the pending repeat count, Stack<StringBuilder> for the outer partial
// result - walked in a single left-to-right pass. StringBuilder (not string
// concatenation) keeps each ']' fold-back O(repeated length) instead of
// re-copying the whole enclosing scope every time.
internal static class DecodeStringSolution
{
    private const int DecimalBase = 10;

    // The textbook answer: recursion depth tracks nesting depth, not input
    // length, so it is written without this repo's Stack<T> - the arm the
    // composed solution below has to justify itself against.
    public static string DecodeByRecursiveDescent(string encoded)
    {
        var position = 0;
        return DecodeRecursive(encoded, ref position).ToString();
    }

    // Push onto both stacks at every '[', pop and fold at every ']' - the
    // same "repo Stack as an explicit parser stack instead of recursion"
    // move BasicCalculatorSolution makes for nested '(' / ')' groups.
    public static string DecodeByStackScan(string encoded)
    {
        var state = new ScanState();

        foreach (var character in encoded)
        {
            ConsumeCharacter(state, character);
        }

        return state.Current.ToString();
    }

    // One character of the scan: digits build up the pending repeat count, '[' opens a
    // nested scope, ']' folds the finished one back into its enclosing scope, and
    // anything else is literal text for the scope currently being built.
    private static void ConsumeCharacter(ScanState state, char character)
    {
        if (char.IsDigit(character))
        {
            state.Number = (state.Number * DecimalBase) + (character - '0');
        }
        else if (character == '[')
        {
            PushGroup(state);
        }
        else if (character == ']')
        {
            PopGroup(state);
        }
        else
        {
            state.Current.Append(character);
        }
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

    private static StringBuilder DecodeRecursive(string encoded, ref int position)
    {
        var builder = new StringBuilder();

        while (position < encoded.Length && encoded[position] != ']')
        {
            if (char.IsDigit(encoded[position]))
            {
                AppendRepeatedGroup(encoded, ref position, builder);
            }
            else
            {
                builder.Append(encoded[position]);
                position++;
            }
        }

        return builder;
    }

    // The digit branch from DecodeRecursive's scan loop: parses the repeat
    // count, recurses into the bracketed group, then tiles the decoded inner
    // text that many times onto the caller's builder.
    private static void AppendRepeatedGroup(string encoded, ref int position, StringBuilder builder)
    {
        var number = 0;

        while (char.IsDigit(encoded[position]))
        {
            number = (number * DecimalBase) + (encoded[position] - '0');
            position++;
        }

        position++; // skip '['
        var inner = DecodeRecursive(encoded, ref position);
        position++; // skip ']'

        for (var r = 0; r < number; r++)
        {
            builder.Append(inner);
        }
    }

    private sealed class ScanState
    {
        public RepoCountStack Counts { get; } = new();
        public RepoBuilderStack Builders { get; } = new();
        public StringBuilder Current { get; set; } = new();
        public int Number { get; set; }
    }
}
