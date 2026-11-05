namespace DSAExperimentation.Domain.Language.Scripts.Latin;

// A diacritic, a mark written on a base letter, as the acute accent sits on é. Unicode writes each
// mark as a combining character that follows its letter, and decomposes a precomposed letter such
// as é into the two. The mark is held as that combining character, and written by its code point,
// since a combining character in source sits on the quote before it and cannot be read. Each mark
// is a named value. German's umlaut and the diaeresis are one mark, U+0308: what it does, a changed
// vowel in ü or a vowel said apart from the one before it in Noël, is the language's rule, not the
// mark's.
/// <summary>A mark written on a letter, and the Unicode combining character that writes it.</summary>
/// <param name="Name">The mark's English name, lowercase, such as "acute".</param>
/// <param name="CombiningMark">The combining character, such as U+0301 for the acute accent.</param>
public readonly record struct Diacritic(string Name, char CombiningMark)
{
    /// <summary>The grave accent, as on à; U+0300.</summary>
    public static Diacritic Grave { get; } = new("grave", (char)0x0300);

    /// <summary>The acute accent, as on é; U+0301.</summary>
    public static Diacritic Acute { get; } = new("acute", (char)0x0301);

    /// <summary>The circumflex, as on ê; U+0302.</summary>
    public static Diacritic Circumflex { get; } = new("circumflex", (char)0x0302);

    /// <summary>The tilde, as on ñ; U+0303.</summary>
    public static Diacritic Tilde { get; } = new("tilde", (char)0x0303);

    /// <summary>The macron, as on ā; U+0304.</summary>
    public static Diacritic Macron { get; } = new("macron", (char)0x0304);

    /// <summary>The breve, as on ă; U+0306.</summary>
    public static Diacritic Breve { get; } = new("breve", (char)0x0306);

    /// <summary>The dot above, as on ż; U+0307.</summary>
    public static Diacritic DotAbove { get; } = new("dot above", (char)0x0307);

    /// <summary>The diaeresis, German's umlaut, as on ä and ë; U+0308.</summary>
    public static Diacritic Diaeresis { get; } = new("diaeresis", (char)0x0308);

    /// <summary>The hook above, as on Vietnamese ả; U+0309.</summary>
    public static Diacritic HookAbove { get; } = new("hook above", (char)0x0309);

    /// <summary>The ring above, as on å; U+030A.</summary>
    public static Diacritic RingAbove { get; } = new("ring above", (char)0x030A);

    /// <summary>The double acute accent, as on Hungarian ő; U+030B.</summary>
    public static Diacritic DoubleAcute { get; } = new("double acute", (char)0x030B);

    /// <summary>The caron, as on č; U+030C.</summary>
    public static Diacritic Caron { get; } = new("caron", (char)0x030C);

    /// <summary>The horn, as on Vietnamese ơ; U+031B.</summary>
    public static Diacritic Horn { get; } = new("horn", (char)0x031B);

    /// <summary>The dot below, as on Vietnamese ạ; U+0323.</summary>
    public static Diacritic DotBelow { get; } = new("dot below", (char)0x0323);

    /// <summary>The comma below, as on Romanian ș; U+0326.</summary>
    public static Diacritic CommaBelow { get; } = new("comma below", (char)0x0326);

    /// <summary>The cedilla, as on ç; U+0327.</summary>
    public static Diacritic Cedilla { get; } = new("cedilla", (char)0x0327);

    /// <summary>The ogonek, as on Polish ą; U+0328.</summary>
    public static Diacritic Ogonek { get; } = new("ogonek", (char)0x0328);
}
