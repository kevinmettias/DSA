using DSAExperimentation.LeetCode.LexicographicallySmallestGeneratedString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicallySmallestGeneratedString;

// Harness only. Both fill strategies are
// LexicographicallySmallestGeneratedStringSolution's - this file just pins
// them to LeetCode's published examples plus hand-verified cases covering
// every branch the published examples miss: a consistent overlapping-'T'
// merge ("TT"/"aa"), an 'F' window that needs its rightmost free character
// bumped ("TF"/"a"), and an 'F' window whose every position is pinned by
// surrounding 'T's, so it cannot be broken at all ("TFTF"/"aaa").
public sealed partial class LexicographicallySmallestGeneratedStringTests
{
    public static TheoryData<GenerationExample> Examples =>
        new()
        {
            { new GenerationExample(Constraints: "TFTF", Template: "ab", Expected: "ababa") },
            { new GenerationExample(Constraints: "TFTF", Template: "abc", Expected: "") },
            { new GenerationExample(Constraints: "F", Template: "d", Expected: "a") },
            { new GenerationExample(Constraints: "TT", Template: "aa", Expected: "aaa") },
            { new GenerationExample(Constraints: "TF", Template: "a", Expected: "ab") },
            { new GenerationExample(Constraints: "TFTF", Template: "aaa", Expected: "") },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateStringByDirectFill_LeetCodeExamples_ReturnsSmallestGeneratedString(
        GenerationExample example)
    {
        var actual = LexicographicallySmallestGeneratedStringSolution.GenerateStringByDirectFill(
            new LexicographicallySmallestGeneratedStringSolution.ConstraintPattern(example.Constraints),
            new LexicographicallySmallestGeneratedStringSolution.TemplateWord(example.Template));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GenerateStringByZFunctionConsistency_LeetCodeExamples_ReturnsSmallestGeneratedString(
        GenerationExample example)
    {
        var actual = LexicographicallySmallestGeneratedStringSolution.GenerateStringByZFunctionConsistency(
            new LexicographicallySmallestGeneratedStringSolution.ConstraintPattern(example.Constraints),
            new LexicographicallySmallestGeneratedStringSolution.TemplateWord(example.Template));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the 'T'/'F' constraint pattern, the template word, and the
    // smallest string satisfying both. The solutions already give the two inputs their
    // own role types (ConstraintPattern, TemplateWord); the row names them for the same
    // reason, so neither can arrive where the other belongs with the compiler silent.
    public readonly record struct GenerationExample(string Constraints, string Template, string Expected);
}
