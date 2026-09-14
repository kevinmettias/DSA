using static DSAExperimentation.LeetCode.DesignATextEditor.DesignATextEditorSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignATextEditor;

// Harness only: both strategies live in DesignATextEditorSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes
// a call script instead of a single argument tuple - the same shape
// DesignBrowserHistoryTests and ImplementQueueUsingStacksTests use for their own
// instance-API problems. AddText returns null in LeetCode's judge output and
// deleteText returns a count while the two cursor moves return strings, so the
// expected sequence is object?[] and reads exactly like the published one.
public sealed class DesignATextEditorTests
{
    public static TheoryData<TextEditorOp[], object?[]> Examples =>
        new()
        {
            {
                [
                    TextEditorOp.AddText("leetcode"),
                    TextEditorOp.DeleteText(4),
                    TextEditorOp.AddText("practice"),
                    TextEditorOp.CursorRight(3),
                    TextEditorOp.CursorLeft(8),
                    TextEditorOp.DeleteText(10),
                    TextEditorOp.CursorLeft(2),
                    TextEditorOp.CursorRight(6),
                ],
                [null, 4, null, "etpractice", "leet", 4, "", "practi"]
            },
            {
                // Deleting more than sits left of the cursor clips to what is
                // actually there, and a second delete against an empty left side
                // removes nothing.
                [
                    TextEditorOp.AddText("hi"),
                    TextEditorOp.DeleteText(50),
                    TextEditorOp.DeleteText(1),
                ],
                [null, 2, 0]
            },
            {
                // More than ten characters left of the cursor: only the last ten
                // are reported, and moving right past the end clamps there.
                [
                    TextEditorOp.AddText("abcdefghijklm"),
                    TextEditorOp.CursorLeft(1),
                    TextEditorOp.CursorRight(99),
                    TextEditorOp.CursorLeft(99),
                ],
                [null, "cdefghijkl", "defghijklm", ""]
            },
            {
                // A delete happens at the cursor, not at the end of the buffer:
                // the text right of the cursor survives and can be walked back over.
                [
                    TextEditorOp.AddText("abcdef"),
                    TextEditorOp.CursorLeft(3),
                    TextEditorOp.DeleteText(2),
                    TextEditorOp.CursorRight(3),
                ],
                [null, "abc", 2, "adef"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TextEditorByListBacked_LeetCodeExamples_MatchesExpectedSequence(
        TextEditorOp[] operations, object?[] expected) =>
        RunScript(new TextEditorByListBacked(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void TextEditorByStackBacked_LeetCodeExamples_MatchesExpectedSequence(
        TextEditorOp[] operations, object?[] expected) =>
        RunScript(new TextEditorByStackBacked(), operations, expected);

    private static void RunScript(
        ITextEditor editor, TextEditorOp[] operations, object?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(editor));
        }
    }
}

// One call in a TextEditor script: which operation to invoke, and with what text
// or character count. Pure dispatch, built via the named factories below so a
// script reads like the LeetCode call sequence it replays.
public readonly record struct TextEditorOp
{
    private readonly Kind _kind;
    private readonly string _text;
    private readonly int _count;

    private TextEditorOp(Kind kind, string text, int count)
    {
        _kind = kind;
        _text = text;
        _count = count;
    }

    public static TextEditorOp AddText(string text) => new(Kind.AddText, text, 0);

    public static TextEditorOp DeleteText(int k) => new(Kind.DeleteText, string.Empty, k);

    public static TextEditorOp CursorLeft(int k) => new(Kind.CursorLeft, string.Empty, k);

    public static TextEditorOp CursorRight(int k) => new(Kind.CursorRight, string.Empty, k);

    // null for AddText, matching LeetCode's own judge output for a void operation;
    // the deleted count for DeleteText and the reported window for the two cursor
    // moves, each boxed as its own type so the script can assert one expected
    // value per operation without forcing them onto a common shape.
    internal object? Apply(ITextEditor editor)
    {
        switch (_kind)
        {
            case Kind.AddText:
                editor.AddText(_text);
                return null;
            case Kind.DeleteText:
                return editor.DeleteText(_count);
            case Kind.CursorLeft:
                return editor.CursorLeft(_count);
            default:
                return editor.CursorRight(_count);
        }
    }

    private enum Kind
    {
        AddText,
        DeleteText,
        CursorLeft,
        CursorRight,
    }
}
