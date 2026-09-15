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
public readonly record struct TextEditorOp(TextEditorOp.OpKind kind, string text, int count)
{
    public static TextEditorOp AddText(string text) => new(OpKind.AddText, text, 0);

    public static TextEditorOp DeleteText(int k) => new(OpKind.DeleteText, string.Empty, k);

    public static TextEditorOp CursorLeft(int k) => new(OpKind.CursorLeft, string.Empty, k);

    public static TextEditorOp CursorRight(int k) => new(OpKind.CursorRight, string.Empty, k);

    // null for AddText, matching LeetCode's own judge output for a void operation;
    // the deleted count for DeleteText and the reported window for the two cursor
    // moves, each boxed as its own type so the script can assert one expected
    // value per operation without forcing them onto a common shape.
    internal object? Apply(ITextEditor editor)
    {
        switch (kind)
        {
            case OpKind.AddText:
                editor.AddText(text);
                return null;
            case OpKind.DeleteText:
                return editor.DeleteText(count);
            case OpKind.CursorLeft:
                return editor.CursorLeft(count);
            default:
                return editor.CursorRight(count);
        }
    }

    public enum OpKind
    {
        AddText,
        DeleteText,
        CursorLeft,
        CursorRight,
    }
}
