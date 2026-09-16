using DSAExperimentation.LeetCode.DesignATextEditor;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignATextEditor;

// Harness only: both strategies live in DesignATextEditorSolution. LeetCode's own
// shape here is a stateful object across a sequence of calls, so Examples encodes
// a call script instead of a single argument tuple - the same shape
// DesignBrowserHistoryTests and ImplementQueueUsingStacksTests use for their own
// instance-API problems. AddText returns null in LeetCode's judge output and
// deleteText returns a count while the two cursor moves return strings, so the
// expected sequence is object?[] and reads exactly like the published one.
public sealed partial class DesignATextEditorTests
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
        Assert.Equal(expected, RunScript(new DesignATextEditorSolution.TextEditorByListBacked(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TextEditorByStackBacked_LeetCodeExamples_MatchesExpectedSequence(
        TextEditorOp[] operations, object?[] expected) =>
        Assert.Equal(expected, RunScript(new DesignATextEditorSolution.TextEditorByStackBacked(), operations));

    private static object?[] RunScript(
        DesignATextEditorSolution.ITextEditor editor,
        TextEditorOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(editor))];
}
