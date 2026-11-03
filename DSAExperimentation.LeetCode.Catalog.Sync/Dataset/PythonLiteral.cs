using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// LeetCodeDataset writes its values the way Python prints them - True/False/None, single-quoted
// strings, tuples, spaces after commas - where LeetCode writes JSON. This translates one Python
// literal into LeetCode's notation token by token: numbers are copied as written, so an integer
// past 2^53 keeps every digit, and strings are decoded from Python's escapes and re-encoded as
// JSON's. The result is compact, as LeetCode writes it: "[[1, 2], (3,)]" becomes "[[1,2],[3]]".
//
// Anything outside that grammar - a set, inf or nan, a bytes or raw string, a bare name - makes
// TryToNotation return false rather than produce JSON that means something else; the importer
// drops such a case. The output is parsed once more as JSON before it is returned, which is what
// refuses a dict whose keys are not strings.
internal sealed class PythonLiteral
{
    // How many hex digits follow each of Python's code-point escapes: \xHH, \uHHHH and \UHHHHHHHH.
    private const int ByteEscapeDigits = 2;
    private const int ShortUnicodeEscapeDigits = 4;
    private const int LongUnicodeEscapeDigits = 8;

    // A backslash and the character after it.
    private const int EscapeLength = 2;

    private const string NegativePoint = "-.";
    private const string NegativeZeroPoint = "-0.";

    private static readonly JsonSerializerOptions StringEncoding = new() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    // Python's three constants, and JSON's: the dataset writes most values as Python prints them,
    // but a tree's level order in LeetCode's own notation ("[5,3,null,4]"), and a few answers as
    // "false". Measured on the first import, 2,262 cases were lost to "null" alone.
    private static readonly Dictionary<string, string> Keywords = new(StringComparer.Ordinal)
    {
        ["True"] = "true",
        ["False"] = "false",
        ["None"] = "null",
        ["true"] = "true",
        ["false"] = "false",
        ["null"] = "null",
    };

    private readonly string _text;
    private readonly StringBuilder _json = new();
    private int _position;

    private PythonLiteral(string text) => _text = text;

    public static bool TryToNotation(string text, out string notation)
    {
        var literal = new PythonLiteral(text);
        notation = string.Empty;

        if (!literal.TryTranslateAll() || !IsJson(literal._json.ToString()))
        {
            return false;
        }

        notation = literal._json.ToString();
        return true;
    }

    private bool TryTranslateAll()
    {
        while (_position < _text.Length)
        {
            if (!TryTranslateNext())
            {
                return false;
            }
        }

        return _json.Length > 0;
    }

    private bool TryTranslateNext()
    {
        var current = _text[_position];

        if (char.IsWhiteSpace(current))
        {
            _position++;
            return true;
        }

        if (current is '\'' or '"')
        {
            return TryTranslateString(current);
        }

        if (IsNumberStart(current))
        {
            return TryTranslateNumber();
        }

        if (char.IsAsciiLetter(current))
        {
            return TryTranslateKeyword();
        }

        return TryTranslatePunctuation(current);
    }

    private bool TryTranslateString(char quote)
    {
        _position++;
        var value = new StringBuilder();

        while (_position < _text.Length && _text[_position] != quote)
        {
            if (!TryAppendCharacter(value))
            {
                return false;
            }
        }

        if (_position == _text.Length)
        {
            return false;
        }

        _position++;
        var encoded = JsonSerializer.Serialize(value.ToString(), StringEncoding);
        _json.Append(encoded);
        return true;
    }

    private bool TryAppendCharacter(StringBuilder value)
    {
        if (_text[_position] != '\\')
        {
            value.Append(_text[_position]);
            _position++;
            return true;
        }

        return TryAppendEscape(value);
    }

    // Python's escapes as its repr writes them. An escape Python does not define keeps its
    // backslash, which is what Python itself does with one.
    private bool TryAppendEscape(StringBuilder value)
    {
        if (_position + 1 >= _text.Length)
        {
            return false;
        }

        var escape = _text[_position + 1];
        _position += EscapeLength;

        switch (escape)
        {
            case 'n':
                value.Append('\n');
                return true;
            case 't':
                value.Append('\t');
                return true;
            case 'r':
                value.Append('\r');
                return true;
            case 'x':
                return TryAppendCodePoint(value, ByteEscapeDigits);
            case 'u':
                return TryAppendCodePoint(value, ShortUnicodeEscapeDigits);
            case 'U':
                return TryAppendCodePoint(value, LongUnicodeEscapeDigits);
            case '\\' or '\'' or '"':
                value.Append(escape);
                return true;
            default:
                value.Append('\\').Append(escape);
                return true;
        }
    }

    private bool TryAppendCodePoint(StringBuilder value, int hexDigits)
    {
        if (!TryConsumeCodePoint(hexDigits, out var codePoint))
        {
            return false;
        }

        var character = char.ConvertFromUtf32(codePoint);
        value.Append(character);
        return true;
    }

    // The escape's hex digits, consumed when there are enough of them and they name a Unicode
    // scalar value.
    private bool TryConsumeCodePoint(int hexDigits, out int codePoint)
    {
        codePoint = 0;

        if (_position + hexDigits > _text.Length)
        {
            return false;
        }

        var digits = _text.AsSpan(_position, hexDigits);
        var isCodePoint = int.TryParse(digits, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)
            && Rune.IsValid(codePoint);

        if (isCodePoint)
        {
            _position += hexDigits;
        }

        return isCodePoint;
    }

    // A digit, or a point or minus sign that begins one: "5", ".5", "-5" and "-.5".
    private bool IsNumberStart(char current) => current switch
    {
        '-' => IsUnsignedNumberAt(_position + 1),
        '.' => IsDigitAt(_position + 1),
        _ => char.IsAsciiDigit(current),
    };

    private bool IsUnsignedNumberAt(int index) => IsDigitAt(index) || (IsCharacterAt(index, '.') && IsDigitAt(index + 1));

    private bool IsCharacterAt(int index, char expected) => index < _text.Length && _text[index] == expected;

    private bool TryTranslateNumber()
    {
        var start = _position;

        while (_position < _text.Length && IsNumberCharacter(_text[_position]))
        {
            _position++;
        }

        var number = _text[start.._position];

        if (!double.TryParse(number, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            return false;
        }

        _json.Append(AsJsonNumber(number));
        return true;
    }

    private static bool IsNumberCharacter(char character)
        => char.IsAsciiDigit(character) || character is '-' or '+' or '.' or 'e' or 'E';

    // Python prints "1." and ".5"; JSON needs a digit on both sides of the point.
    private static string AsJsonNumber(string number)
    {
        var withLeadingDigit = number.StartsWith('.') ? $"0{number}" : number.Replace(NegativePoint, NegativeZeroPoint, StringComparison.Ordinal);
        return withLeadingDigit.EndsWith('.') ? $"{withLeadingDigit}0" : withLeadingDigit;
    }

    private bool TryTranslateKeyword()
    {
        var start = _position;

        while (_position < _text.Length && char.IsAsciiLetterOrDigit(_text[_position]))
        {
            _position++;
        }

        if (!Keywords.TryGetValue(_text[start.._position], out var json))
        {
            return false;
        }

        _json.Append(json);
        return true;
    }

    private bool TryTranslatePunctuation(char current)
    {
        switch (current)
        {
            case '[' or '(':
                _json.Append('[');
                break;
            case ']' or ')':
                DropTrailingComma();
                _json.Append(']');
                break;
            case '{' or ':' or ',':
                _json.Append(current);
                break;
            case '}':
                DropTrailingComma();
                _json.Append('}');
                break;
            default:
                return false;
        }

        _position++;
        return true;
    }

    // Python allows "(3,)" and "[1, 2,]"; JSON does not.
    private void DropTrailingComma()
    {
        if (_json.Length > 0 && _json[^1] == ',')
        {
            _json.Length--;
        }
    }

    private static bool IsJson(string candidate)
    {
        try
        {
            using var document = JsonDocument.Parse(candidate);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private bool IsDigitAt(int index) => index < _text.Length && char.IsAsciiDigit(_text[index]);
}
