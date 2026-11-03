using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Devices.Telephone;

// The letters ITU-T E.161 puts on keys 2 to 9, as LC 17 reads them: 2 is abc, 3 def, 4 ghi, 5 jkl,
// 6 mno, 7 pqrs, 8 tuv and 9 wxyz, and 0 and 1 carry none. Keypads print the letters in capitals; the
// keys hold them lowercase, as LC 17's strings are, and Keypad looks a letter up in either case.
/// <summary>The ITU-T E.161 keypad: every letter, Q and Z included.</summary>
public readonly struct E161Keypad : IKeypadLayout
{
    /// <summary>Key 0, no letters.</summary>
    public static PhoneKey Zero { get; } = new('0', []);

    /// <summary>Key 1, no letters.</summary>
    public static PhoneKey One { get; } = new('1', []);

    /// <summary>Key 2: a, b, c.</summary>
    public static PhoneKey Two { get; } = new('2', ['a', 'b', 'c']);

    /// <summary>Key 3: d, e, f.</summary>
    public static PhoneKey Three { get; } = new('3', ['d', 'e', 'f']);

    /// <summary>Key 4: g, h, i.</summary>
    public static PhoneKey Four { get; } = new('4', ['g', 'h', 'i']);

    /// <summary>Key 5: j, k, l.</summary>
    public static PhoneKey Five { get; } = new('5', ['j', 'k', 'l']);

    /// <summary>Key 6: m, n, o.</summary>
    public static PhoneKey Six { get; } = new('6', ['m', 'n', 'o']);

    /// <summary>Key 7: p, q, r, s.</summary>
    public static PhoneKey Seven { get; } = new('7', ['p', 'q', 'r', 's']);

    /// <summary>Key 8: t, u, v.</summary>
    public static PhoneKey Eight { get; } = new('8', ['t', 'u', 'v']);

    /// <summary>Key 9: w, x, y, z.</summary>
    public static PhoneKey Nine { get; } = new('9', ['w', 'x', 'y', 'z']);

    /// <inheritdoc/>
    public static ImmutableArray<PhoneKey> Keys { get; } = [Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine];
}
