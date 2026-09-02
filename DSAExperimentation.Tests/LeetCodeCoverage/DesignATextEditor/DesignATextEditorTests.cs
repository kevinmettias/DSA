using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignATextEditor;

// LeetCode 2296. Design a Text Editor: the classic two-stack cursor design - a
// "left of cursor" Stack<char> (top = char immediately left of the cursor) and a
// "right of cursor" Stack<char> (top = char immediately right), both this repo's
// own Stack<T>. AddText pushes onto left; CursorLeft/CursorRight move characters
// one at a time between the two stacks; DeleteText pops from left. No shifting of
// unrelated characters is ever needed, unlike an array/List positional insert.
public sealed partial class DesignATextEditorTests
{
    [Fact]
    public void FullOperationSequence_LeetCodeExample_MatchesExpectedOutputs()
    {
        var editor = new TextEditor();

        editor.AddText("leetcode");
        Assert.Equal(4, editor.DeleteText(4));

        editor.AddText("practice");
        Assert.Equal("etpractice", editor.CursorRight(3));
        Assert.Equal("leet", editor.CursorLeft(8));
        Assert.Equal(4, editor.DeleteText(10));
        Assert.Equal("", editor.CursorLeft(2));
        Assert.Equal("practi", editor.CursorRight(6));
    }

    [Fact]
    public void DeleteText_MoreThanAvailable_ClipsToCharactersActuallyPresent()
    {
        var editor = new TextEditor();
        editor.AddText("hi");

        Assert.Equal(2, editor.DeleteText(50));
        Assert.Equal(0, editor.DeleteText(1));
    }

    private sealed class TextEditor
    {
        private readonly RepoStack _left = new();
        private readonly RepoStack _right = new();

        public void AddText(string text)
        {
            foreach (var c in text)
            {
                _left.Push(c);
            }
        }

        public int DeleteText(int k)
        {
            var deleted = 0;

            while (deleted < k && _left.TryPop(out _))
            {
                deleted++;
            }

            return deleted;
        }

        public string CursorLeft(int k)
        {
            var moved = 0;

            while (moved < k && _left.TryPop(out var c))
            {
                _right.Push(c);
                moved++;
            }

            return LastTenBeforeCursor();
        }

        public string CursorRight(int k)
        {
            var moved = 0;

            while (moved < k && _right.TryPop(out var c))
            {
                _left.Push(c);
                moved++;
            }

            return LastTenBeforeCursor();
        }

        private string LastTenBeforeCursor()
        {
            var popped = new List<char>(10);

            while (popped.Count < 10 && _left.TryPop(out var c))
            {
                popped.Add(c);
            }

            for (var i = popped.Count - 1; i >= 0; i--)
            {
                _left.Push(popped[i]);
            }

            popped.Reverse();
            return new string(popped.ToArray());
        }
    }
}
