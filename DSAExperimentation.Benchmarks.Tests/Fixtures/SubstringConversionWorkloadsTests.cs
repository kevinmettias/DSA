using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for SubstringConversionWorkloads (ARCHITECTURE 17.7). The LC 2977 reading
// depends on the rules being short and on both strings being the requested length, so each
// strategy's per-position window scan has real candidate substrings to test rather than missing on
// every lookup.
public sealed partial class SubstringConversionWorkloadsTests
{
    private const int RuleCount = 60;
    private const int RulesSeed = 2977; // LC problem number
    private const int StringLength = 100;
    private const int StringsSeed = 2977;
    private const int MinRuleLength = 1;
    private const int MaxRuleLength = 3;
    private const int MinRuleCost = 1;
    private const int MaxRuleCost = 999; // one below the fixture's own exclusive ceiling of 1000
    private const char FirstAlphabetLetter = 'a';
    private const char LastAlphabetLetter = 'z';

    [Fact]
    public void BuildRules_RuleCount_ReturnsThatManyOfEachRuleField()
    {
        var (original, changed, cost) = SubstringConversionWorkloads.BuildRules(RulesSeed);

        Assert.Equal(RuleCount, original.Length);
        Assert.Equal(RuleCount, changed.Length);
        Assert.Equal(RuleCount, cost.Length);
    }

    [Fact]
    public void BuildRules_EveryRule_PairsAnOriginalAndAChangedOfTheSameDocumentedLength()
    {
        var (original, changed, _) = SubstringConversionWorkloads.BuildRules(RulesSeed);

        for (var rule = 0; rule < RuleCount; rule++)
        {
            Assert.Equal(original[rule].Length, changed[rule].Length);
            Assert.InRange(original[rule].Length, MinRuleLength, MaxRuleLength);
            Assert.InRange(changed[rule].Length, MinRuleLength, MaxRuleLength);
        }
    }

    [Fact]
    public void BuildRules_EveryRule_CarriesAPositiveCostInsideTheDocumentedBand() =>
        Assert.All(
            SubstringConversionWorkloads.BuildRules(RulesSeed).Cost,
            cost => Assert.InRange(cost, MinRuleCost, MaxRuleCost));

    [Fact]
    public void BuildRules_SameSeed_ReturnsTheSameRules()
    {
        var (original, changed, cost) = SubstringConversionWorkloads.BuildRules(RulesSeed);
        var (repeatOriginal, repeatChanged, repeatCost) = SubstringConversionWorkloads.BuildRules(RulesSeed);

        Assert.Equal(original, repeatOriginal);
        Assert.Equal(changed, repeatChanged);
        Assert.Equal(cost, repeatCost);
    }

    [Fact]
    public void BuildStrings_Length_ReturnsARequestedLengthSourceAndTarget()
    {
        var (source, target) = SubstringConversionWorkloads.BuildStrings(StringLength, StringsSeed);

        Assert.Equal(StringLength, source.Length);
        Assert.Equal(StringLength, target.Length);
    }

    [Fact]
    public void BuildStrings_EveryCharacter_ComesFromTheDocumentedAlphabet()
    {
        var (source, target) = SubstringConversionWorkloads.BuildStrings(StringLength, StringsSeed);

        Assert.All(source, character => Assert.InRange(character, FirstAlphabetLetter, LastAlphabetLetter));
        Assert.All(target, character => Assert.InRange(character, FirstAlphabetLetter, LastAlphabetLetter));
    }

    [Fact]
    public void BuildStrings_SameSeed_ReturnsTheSameStrings() =>
        Assert.Equal(
            SubstringConversionWorkloads.BuildStrings(StringLength, StringsSeed),
            SubstringConversionWorkloads.BuildStrings(StringLength, StringsSeed));
}
