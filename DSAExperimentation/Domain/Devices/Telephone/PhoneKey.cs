using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Devices.Telephone;

// One key of a telephone keypad: its digit, and the letters printed on it in order, so the second
// letter of key 3 is its Letters[1], 'e'. The letters are the one source; Spelling is them as one
// string, spelled once when the key is made, for a caller that walks them as text without allocating.
/// <summary>A keypad key: its digit and the letters on it.</summary>
/// <param name="Digit">The key's digit, '0' through '9'.</param>
/// <param name="Letters">The letters on the key, lowercase, in the order the key lists them; empty for a key with none.</param>
public readonly record struct PhoneKey(char Digit, ImmutableArray<char> Letters)
{
    /// <summary>The key's letters as one string, in order.</summary>
    public string Spelling { get; } = new(Letters.AsSpan());
}
