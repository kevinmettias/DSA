using DSAExperimentation.Domain.Language.English;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Domain.Devices.Telephone;

// A keypad layout read both ways: which key a digit names, and which key and how many presses type a
// letter by multi-tap. The keys are the one source; the press for each letter is built from them once
// into a table over the 26 English letters, so a letter is looked up in either case and a layout
// cannot disagree with its own presses. A key is found by its digit's slot, so a layout listed out of
// digit order is a broken layout, which a test catches.
/// <summary>A keypad layout's keys by digit, and the key press that types each letter.</summary>
/// <typeparam name="TLayout">The layout, such as <see cref="E161Keypad"/>.</typeparam>
public static class Keypad<TLayout>
    where TLayout : struct, IKeypadLayout
{
    private const string NotOnTheKeypadMessage = "Expected a letter printed on one of the keypad's keys.";

    private static readonly KeyPress?[] PressesByLetter = BuildPresses();

    /// <summary>The key a digit names.</summary>
    /// <param name="digit">A digit, '0' through '9'.</param>
    /// <returns>The key, with its letters.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="digit"/> is not a decimal digit.</exception>
    public static PhoneKey KeyFor(char digit) => TLayout.Keys[DecimalDigits.IndexOf(digit)];

    /// <summary>The key and press count that type a letter by multi-tap.</summary>
    /// <param name="letter">An English letter in either case.</param>
    /// <returns>The key the letter is on, and how many presses reach it.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not printed on any key.</exception>
    public static KeyPress PressFor(char letter)
    {
        if (!EnglishLetters.TryIndexOf(letter, out var slot) || PressesByLetter[slot] is not { } press)
        {
            throw new ArgumentOutOfRangeException(nameof(letter), letter, NotOnTheKeypadMessage);
        }

        return press;
    }

    private static KeyPress?[] BuildPresses()
    {
        var presses = new KeyPress?[EnglishLetters.Size];

        foreach (var key in TLayout.Keys)
        {
            for (var position = 0; position < key.Letters.Length; position++)
            {
                presses[EnglishLetters.IndexOf(key.Letters[position])] = new KeyPress(key, position + 1);
            }
        }

        return presses;
    }
}
