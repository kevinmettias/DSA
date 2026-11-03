using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.Dataset;

public sealed partial class PythonLiteralTests
{
    public static TheoryData<string, string> Translations =>
        new()
        {
            { "[0, 1]", "[0,1]" },
            { "True", "true" },
            { "None", "null" },
            { "'abc'", "\"abc\"" },
            { "[['abc', 'bca'], ['x']]", "[[\"abc\",\"bca\"],[\"x\"]]" },
            { "(1, 2)", "[1,2]" },
            { "(3,)", "[3]" },
            { "2.0", "2.0" },
            { "1.", "1.0" },
            { ".5", "0.5" },
            { "-.5", "-0.5" },
            { "1e-05", "1e-05" },
            { "123456789012345678901", "123456789012345678901" },
            { "\"it's\"", "\"it's\"" },
            { "'a\\'b'", "\"a'b\"" },
            { "'tab\\there'", "\"tab\\there\"" },
            { "'\\x41'", "\"A\"" },
            { "[None, 2, None]", "[null,2,null]" },
            { "[5,3,6,null,null,4]", "[5,3,6,null,null,4]" },
            { "false", "false" },
            { "{'a': 1}", "{\"a\":1}" },
        };

    public static TheoryData<string> Untranslatable =>
        new()
        {
            "inf",
            "nan",
            "{1, 2}",
            "b'bytes'",
            "'unterminated",
            "",
            "Solution()",
        };

    [Theory]
    [MemberData(nameof(Translations))]
    public void TryToNotation_PythonLiteral_WritesLeetCodesCompactJson(string python, string expected)
    {
        var isTranslated = PythonLiteral.TryToNotation(python, out var notation);

        Assert.True(isTranslated);
        Assert.Equal(expected, notation);
    }

    [Theory]
    [MemberData(nameof(Untranslatable))]
    public void TryToNotation_OutsideTheLiteralGrammar_Declines(string python)
    {
        var isTranslated = PythonLiteral.TryToNotation(python, out _);

        Assert.False(isTranslated);
    }
}
