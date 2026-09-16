using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

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
    // this repo's Stack<char>.
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

    // The composed answer: the classic two-stack cursor design over this repo's own
    // Stack<char>. The left stack holds everything before the cursor, top = the
    // character immediately left of it; the right stack holds everything after,
    // top = the character immediately right. AddText pushes onto left, DeleteText
    // pops from left, and a cursor move transfers characters one at a time between
    // the two stacks - so no character other than the ones actually crossed is ever
    // touched, unlike the positional insert the List baseline is forced into.
    internal sealed class TextEditorByStackBacked : ITextEditor
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

        public int DeleteText(int maxDeletions)
        {
            var deleted = 0;

            while (deleted < maxDeletions && _left.TryPop(out _))
            {
                deleted++;
            }

            return deleted;
        }

        public string CursorLeft(int maxSteps)
        {
            var moved = 0;

            while (moved < maxSteps && _left.TryPop(out var c))
            {
                _right.Push(c);
                moved++;
            }

            return LastCharactersBeforeCursor();
        }

        public string CursorRight(int maxSteps)
        {
            var moved = 0;

            while (moved < maxSteps && _right.TryPop(out var c))
            {
                _left.Push(c);
                moved++;
            }

            return LastCharactersBeforeCursor();
        }

        // Stack<char> exposes only the top, so the window is read by popping it off
        // and pushing it straight back - at most CursorWindow characters, whatever
        // the buffer has grown to.
        private string LastCharactersBeforeCursor()
        {
            var popped = new List<char>(CursorWindow);

            while (popped.Count < CursorWindow && _left.TryPop(out var c))
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
