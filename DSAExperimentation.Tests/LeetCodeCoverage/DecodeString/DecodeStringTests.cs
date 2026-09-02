using System.Text;
using RepoCountStack = DSAExperimentation.DataStructures.Stack.Stack<int>;
using RepoBuilderStack = DSAExperimentation.DataStructures.Stack.Stack<System.Text.StringBuilder>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DecodeString;

// LeetCode 394. Decode String: a single left-to-right pass over the encoded string,
// pushing onto two of this repo's own Stack<T> instances at every '[' - Stack<int>
// for the pending repeat count, Stack<StringBuilder> for the enclosing scope's
// partial result - and popping/folding them back together at every ']'. The same
// "repo Stack as an explicit parser stack instead of recursion" move
// BasicCalculatorTests already makes for nested '(' / ')' groups. StringBuilder
// (not string concatenation) keeps each ']' fold-back O(repeated length) instead of
// re-copying the whole enclosing scope every time.
public sealed partial class DecodeStringTests
{
    [Theory]
    [InlineData("3[a]2[bc]", "aaabcbc")]
    [InlineData("3[a2[c]]", "accaccacc")]
    [InlineData("2[abc]3[cd]ef", "abcabccdcdcdef")]
    [InlineData("abc", "abc")]
    public void Decode_LeetCodeExamples_ReturnsExpandedString(string encoded, string expected)
        => Assert.Equal(expected, Decode(encoded));

    private static string Decode(string s)
    {
        var state = new DecoderState();

        foreach (var c in s)
        {
            state.ProcessChar(c);
        }

        return state.Result;
    }

    private sealed class DecoderState
    {
        private readonly RepoCountStack _counts = new();
        private readonly RepoBuilderStack _builders = new();
        private StringBuilder _current = new();
        private int _number;

        public string Result => _current.ToString();

        public void ProcessChar(char c)
        {
            if (char.IsDigit(c))
            {
                _number = (_number * 10) + (c - '0');
            }
            else if (c == '[')
            {
                OpenGroup();
            }
            else if (c == ']')
            {
                CloseGroup();
            }
            else
            {
                _current.Append(c);
            }
        }

        private void OpenGroup()
        {
            _counts.Push(_number);
            _builders.Push(_current);
            _current = new StringBuilder();
            _number = 0;
        }

        private void CloseGroup()
        {
            _counts.TryPop(out var repeatCount);
            _builders.TryPop(out var outer);

            for (var r = 0; r < repeatCount; r++)
            {
                outer.Append(_current);
            }

            _current = outer;
        }
    }
}
