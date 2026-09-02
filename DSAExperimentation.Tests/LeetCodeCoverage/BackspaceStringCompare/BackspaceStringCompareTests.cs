using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BackspaceStringCompare;

// LeetCode 844. Backspace String Compare: this repo's Stack<char> replays each
// string's keystrokes, popping on '#' the same way ValidParenthesesTests' Stack<char>
// pops on a closing bracket - the processed result is whatever's left on the stack.
public sealed partial class BackspaceStringCompareTests
{
    [Fact]
    public void BackspaceCompare_BackspacesCancelEarlierLetters_ReturnsTrue()
    {
        var actual = BackspaceCompare("ab#c", "ad#c");

        Assert.True(actual);
    }

    [Fact]
    public void BackspaceCompare_EverythingTypedIsThenDeleted_ReturnsTrue()
    {
        var actual = BackspaceCompare("ab##", "c#d#");

        Assert.True(actual);
    }

    [Fact]
    public void BackspaceCompare_DifferentSurvivingLetters_ReturnsFalse()
    {
        var actual = BackspaceCompare("a#c", "b");

        Assert.False(actual);
    }

    private static bool BackspaceCompare(string s, string t) => Process(s) == Process(t);

    private static string Process(string input)
    {
        var stack = new RepoCharStack();

        foreach (var ch in input)
        {
            if (ch == '#')
            {
                stack.TryPop(out _);
            }
            else
            {
                stack.Push(ch);
            }
        }

        var chars = new char[stack.Count];

        for (var i = chars.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out chars[i]);
        }

        return new string(chars);
    }
}
