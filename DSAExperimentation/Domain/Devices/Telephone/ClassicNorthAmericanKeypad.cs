using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Devices.Telephone;

// The North American keypad before E.161: the same letters on keys 2 to 9 except that Q and Z appear
// nowhere, so 7 is prs and 9 is wxy. A layout, like E.161, but one a letter may be missing from.
/// <summary>The North American keypad before E.161, without Q and Z.</summary>
public readonly struct ClassicNorthAmericanKeypad : IKeypadLayout
{
    /// <summary>Key 7: p, r, s.</summary>
    public static PhoneKey Seven { get; } = new('7', ['p', 'r', 's']);

    /// <summary>Key 9: w, x, y.</summary>
    public static PhoneKey Nine { get; } = new('9', ['w', 'x', 'y']);

    /// <inheritdoc/>
    public static ImmutableArray<PhoneKey> Keys { get; } =
        [E161Keypad.Zero, E161Keypad.One, E161Keypad.Two, E161Keypad.Three, E161Keypad.Four, E161Keypad.Five, E161Keypad.Six, Seven, E161Keypad.Eight, Nine];
}
