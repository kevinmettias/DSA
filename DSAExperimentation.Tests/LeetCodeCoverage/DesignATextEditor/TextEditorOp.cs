using DSAExperimentation.LeetCode.DesignATextEditor;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignATextEditor;

// One call in a TextEditor script: which operation to invoke, and with what text
// or character count. Pure dispatch, built via the named factories below so a
// script reads like the LeetCode call sequence it replays.
public readonly record struct TextEditorOp(TextEditorOp.OpKind kind, string text, int count)
{
    public static TextEditorOp AddText(string text) => new(OpKind.AddText, text, 0);

    public static TextEditorOp DeleteText(int maxDeletions) => new(OpKind.DeleteText, string.Empty, maxDeletions);

    public static TextEditorOp CursorLeft(int maxSteps) => new(OpKind.CursorLeft, string.Empty, maxSteps);

    public static TextEditorOp CursorRight(int maxSteps) => new(OpKind.CursorRight, string.Empty, maxSteps);

    // null for AddText, matching LeetCode's own judge output for a void operation;
    // the deleted count for DeleteText and the reported window for the two cursor
    // moves, each boxed as its own type so the script can assert one expected
    // value per operation without forcing them onto a common shape.
    internal object? Apply(DesignATextEditorSolution.ITextEditor editor)
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
