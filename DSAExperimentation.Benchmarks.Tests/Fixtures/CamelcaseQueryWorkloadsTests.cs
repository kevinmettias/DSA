using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CamelcaseQueryWorkloads (ARCHITECTURE 17.7). Both arms are scored on the
// same queries, and the fixture's contract is that EVERY generated query is a genuine match -
// that is what stops either arm short-circuiting on an early uppercase mismatch. What the reading
// depends on is therefore the count, each query's uppercase skeleton, and the lowercase runs
// spliced around it.
public sealed partial class CamelcaseQueryWorkloadsTests
{
    private const string Pattern = "FB";
    private const int Count = 32;
    private const int Seed = 1023; // LC problem number
    private const int MinRunLength = 3;
    private const int MaxRunLength = 7;

    [Fact]
    public void BuildMatchingQueries_Count_ReturnsOneQueryPerRequestedIndex() =>
        Assert.Equal(Count, CamelcaseQueryWorkloads.BuildMatchingQueries(Pattern, Count, Seed).Length);

    [Fact]
    public void BuildMatchingQueries_EveryQuery_CarriesThePatternAsItsUppercaseSkeleton()
    {
        var queries = CamelcaseQueryWorkloads.BuildMatchingQueries(Pattern, Count, Seed);

        Assert.All(queries, query => Assert.Equal(Pattern, UppercaseLetters(query)));
    }

    [Fact]
    public void BuildMatchingQueries_EveryQuery_SplicesOneLowercaseRunAroundEachPatternLetter()
    {
        var queries = CamelcaseQueryWorkloads.BuildMatchingQueries(Pattern, Count, Seed);
        var minimumLength = Pattern.Length + (MinRunLength * (Pattern.Length + 1));
        var maximumLength = Pattern.Length + (MaxRunLength * (Pattern.Length + 1));

        Assert.All(queries, query => Assert.InRange(query.Length, minimumLength, maximumLength));
    }

    [Fact]
    public void BuildMatchingQueries_SameSeed_ReturnsTheSameQueries() =>
        Assert.Equal(
            CamelcaseQueryWorkloads.BuildMatchingQueries(Pattern, Count, Seed),
            CamelcaseQueryWorkloads.BuildMatchingQueries(Pattern, Count, Seed));

    private static string UppercaseLetters(string query) => new([.. query.Where(char.IsUpper)]);
}
