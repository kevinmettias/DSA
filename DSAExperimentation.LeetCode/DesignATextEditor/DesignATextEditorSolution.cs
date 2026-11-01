using DSAExperimentation.DataStructures.CursorSequence;

namespace DSAExperimentation.LeetCode.DesignATextEditor;

// LeetCode 2296. Design a Text Editor: a text buffer with a cursor, where every
// edit happens at the cursor and each cursor move reports the last ten characters
// to its left.
//
// This is a design problem - LeetCode's own shape is a stateful object with four
// operations, not a single return value - so "every strategy for the problem"
// (ARCHITECTURE.md §17.3) takes the form of two full classes implementing the
// shared ITextEditor surface below, the same shape DesignBrowserHistorySolution
// uses for its own instance-API problem (LC 1472).
internal static class DesignATextEditorSolution
{
    // LeetCode reports the last ten characters left of the cursor, or fewer when
    // fewer exist.
    private const int CursorWindow = 10;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ITextEditor
    {
        void AddText(string text);

        int DeleteText(int maxDeletions);

        string CursorLeft(int maxSteps);

        string CursorRight(int maxSteps);
    }

    // The textbook baseline this composition has to justify itself against: a BCL
    // List<char> plus a cursor index, editing at the cursor with InsertRange and
    // RemoveRange - which shift every character past the cursor on every edit -
    // and slicing the report window with GetRange. Deliberately written without
    // this repo's CursorSequence<char>.
    internal sealed class TextEditorByListBacked : ITextEditor
    {
        private readonly List<char> _buffer = [];
        private int _cursor;

        public void AddText(string text)
        {
            _buffer.InsertRange(_cursor, text);
            _cursor += text.Length;
        }

        public int DeleteText(int maxDeletions)
        {
            var deleted = Math.Min(maxDeletions, _cursor);
            _buffer.RemoveRange(_cursor - deleted, deleted);
            _cursor -= deleted;

            return deleted;
        }

        public string CursorLeft(int maxSteps)
        {
            _cursor = Math.Max(0, _cursor - maxSteps);

            return LastCharactersBeforeCursor();
        }

        public string CursorRight(int maxSteps)
        {
            _cursor = Math.Min(_buffer.Count, _cursor + maxSteps);

            return LastCharactersBeforeCursor();
        }

        private string LastCharactersBeforeCursor()
        {
            var start = Math.Max(0, _cursor - CursorWindow);

            return new string(_buffer.GetRange(start, _cursor - start).ToArray());
        }
    }

    // The composed answer: this repo's own CursorSequence<char>, the classic
    // two-sided cursor design. Every edit happens at the cursor, and a cursor move
    // crosses characters one at a time - so no character other than the ones
    // actually crossed is ever touched, unlike the positional insert the List
    // baseline is forced into - and the report window is read by position just
    // left of the cursor.
    internal sealed class TextEditorByCursorSequence : ITextEditor
    {
        private readonly CursorSequence<char> _text = new();

        public void AddText(string text)
        {
            foreach (var c in text)
            {
                _text.InsertBeforeCursor(c);
            }
        }

        public int DeleteText(int maxDeletions) => _text.RemoveBeforeCursor(maxDeletions);

        public string CursorLeft(int maxSteps)
        {
            _text.MoveLeft(maxSteps);
            return LastCharactersBeforeCursor();
        }

        public string CursorRight(int maxSteps)
        {
            _text.MoveRight(maxSteps);
            return LastCharactersBeforeCursor();
        }

        // At most CursorWindow characters, read by position just left of the cursor,
        // whatever the buffer has grown to.
        private string LastCharactersBeforeCursor()
        {
            var start = Math.Max(0, _text.CountBeforeCursor - CursorWindow);
            var window = new char[_text.CountBeforeCursor - start];

            for (var i = 0; i < window.Length; i++)
            {
                window[i] = _text.GetBeforeCursor(start + i);
            }

            return new string(window);
        }
    }
}
