using System.Globalization;

namespace DSAExperimentation.LeetCode.Catalog.Bounds;

// The arithmetic a constraint's limit is written in: sums of products of powers, each term a
// plain number - "10^4", "2 * 10^5", "2^31 - 1", "-10^9", "10^9 + 7", "1.5", "10,000". A limit
// written in words or naming an input is not one of these, and TryEvaluate refuses it.
//
// Unary minus binds looser than ^, as the statements mean it: -2^31 is -(2^31).
internal sealed class BoundExpression
{
    private const char Power = '^';
    private const char Plus = '+';
    private const char Minus = '-';
    private const char DecimalPoint = '.';
    private const char ThousandsSeparator = ',';
    private const string Space = " ";
    private const int DigitsPerThousandsGroup = 3;

    // The ways the statements write multiplication: '*', 'x' and the multiplication sign.
    private static readonly char[] Multiplications = ['*', 'x', '\u00d7'];

    private readonly string _text;
    private int _position;

    private bool IsAtEnd => _position == _text.Length;

    private BoundExpression(string text) => _text = text.Replace(Space, string.Empty, StringComparison.Ordinal);

    public static bool TryEvaluate(string text, out double value)
    {
        var expression = new BoundExpression(text);
        return expression.TryReadSum(out value) && expression.IsAtEnd;
    }

    private bool TryReadSum(out double value)
    {
        var sign = ReadSign();

        if (!TryReadProduct(out var first))
        {
            value = 0;
            return false;
        }

        return TryReadTerms(sign * first, out value);
    }

    private bool TryReadTerms(double first, out double value)
    {
        value = first;

        while (TryReadAdditiveOperator(out var termSign))
        {
            if (!TryReadProduct(out var term))
            {
                return false;
            }

            value += termSign * term;
        }

        return true;
    }

    private bool TryReadAdditiveOperator(out double sign)
    {
        if (TryConsume(Plus))
        {
            sign = 1;
            return true;
        }

        sign = -1;
        return TryConsume(Minus);
    }

    private double ReadSign() => TryConsume(Minus) ? -1 : 1;

    private bool TryConsume(char expected) => TryConsumeAny([expected]);

    private bool TryConsumeAny(char[] candidates)
    {
        var isMatch = _position < _text.Length && candidates.Contains(_text[_position]);

        if (isMatch)
        {
            _position++;
        }

        return isMatch;
    }

    private bool TryReadProduct(out double value)
    {
        if (!TryReadPower(out value))
        {
            return false;
        }

        while (TryConsumeAny(Multiplications))
        {
            if (!TryReadPower(out var factor))
            {
                return false;
            }

            value *= factor;
        }

        return true;
    }

    private bool TryReadPower(out double value)
    {
        if (!TryReadNumber(out value))
        {
            return false;
        }

        if (!TryConsume(Power))
        {
            return true;
        }

        var exponentSign = ReadSign();

        if (!TryReadNumber(out var exponent))
        {
            return false;
        }

        value = Math.Pow(value, exponentSign * exponent);
        return true;
    }

    private bool TryReadNumber(out double value)
    {
        var start = _position;
        SkipDigitGroups();

        if (TryConsume(DecimalPoint))
        {
            SkipDigits();
        }

        var digits = _text[start.._position].Replace(ThousandsSeparator.ToString(), string.Empty, StringComparison.Ordinal);
        return double.TryParse(digits, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);
    }

    // Digits, with a comma allowed only where it separates a group of exactly three, so that the
    // comma between two subjects ("1 <= m, n") is never read as part of a number.
    private void SkipDigitGroups()
    {
        SkipDigits();

        while (IsThousandsGroupAhead())
        {
            _position += DigitsPerThousandsGroup + 1;
        }
    }

    private void SkipDigits()
    {
        while (_position < _text.Length && char.IsAsciiDigit(_text[_position]))
        {
            _position++;
        }
    }

    private bool IsThousandsGroupAhead()
    {
        var groupEnd = _position + DigitsPerThousandsGroup;

        if (groupEnd >= _text.Length || _text[_position] != ThousandsSeparator)
        {
            return false;
        }

        var group = _text.AsSpan(_position + 1, DigitsPerThousandsGroup);
        return IsAllDigits(group) && IsBoundaryAfter(groupEnd + 1);
    }

    private static bool IsAllDigits(ReadOnlySpan<char> characters) => !characters.ContainsAnyExceptInRange('0', '9');

    private bool IsBoundaryAfter(int index) => index >= _text.Length || !char.IsAsciiDigit(_text[index]);
}
