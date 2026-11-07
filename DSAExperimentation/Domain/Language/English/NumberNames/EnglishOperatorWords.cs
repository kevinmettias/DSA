namespace DSAExperimentation.Domain.Language.English.NumberNames;

// The words English joins number words with when it reads a number aloud: "minus" before a
// negative number and between terms, "plus" and "times" in an expression, "point" before a
// decimal's digits, "and" before a mixed number's fraction, and "percent" after a percentage. They
// are not number words, so they are not NumberWords, and the composer and the readings share them
// from here. Words are separated by a space, and a tens word is joined to its ones by a hyphen.
/// <summary>The words English reads between and around number words.</summary>
public static class EnglishOperatorWords
{
    /// <summary>The separator between two words, a space.</summary>
    internal const char WordSeparator = ' ';

    /// <summary>The joint between a tens word and its ones, a hyphen: twenty-one.</summary>
    internal const char CompoundJoint = '-';

    /// <summary>Gets "minus", before a negative number and before a subtracted term.</summary>
    public static string Minus => "minus";

    /// <summary>Gets "plus", before an added term.</summary>
    public static string Plus => "plus";

    /// <summary>Gets "times", between two factors.</summary>
    public static string Times => "times";

    /// <summary>Gets "point", before a decimal's digits.</summary>
    public static string Point => "point";

    /// <summary>Gets "and", before a mixed number's fraction.</summary>
    public static string And => "and";

    /// <summary>Gets "percent", after a percentage.</summary>
    public static string Percent => "percent";
}
