namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// Tracks, one character at a time, whether a scan of a Python or JSON literal is inside a string
// or a bracket, so that DatasetInput can tell a comma between assignments from one inside a value.
// The character that opens or closes a string or bracket counts as inside it. A class, not a
// struct: it is state that advances, and a copy of it would advance on its own.
internal sealed class NestingScanner
{
    private int _depth;
    private char? _quote;
    private bool _isEscaped;

    public bool IsAtTopLevel => _depth == 0 && _quote is null;

    public void Advance(char character)
    {
        if (_quote is { } quote)
        {
            AdvanceInString(character, quote);
            return;
        }

        switch (character)
        {
            case '\'' or '"':
                _quote = character;
                break;
            case '[' or '(' or '{':
                _depth++;
                break;
            case ']' or ')' or '}':
                _depth--;
                break;
        }
    }

    private void AdvanceInString(char character, char quote)
    {
        if (_isEscaped)
        {
            _isEscaped = false;
            return;
        }

        if (character == '\\')
        {
            _isEscaped = true;
            return;
        }

        if (character == quote)
        {
            _quote = null;
        }
    }
}
