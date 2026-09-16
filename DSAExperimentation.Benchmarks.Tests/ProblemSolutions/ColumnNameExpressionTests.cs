using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ColumnNameExpression (ARCHITECTURE 17.9). This one is not a benchmark class:
// it is the pure, stateless generator BasicCalculatorIVBenchmarks' arms are fed, so it has no
// competing arms to reconcile and no [Params] property. The assertion therefore comes from what its
// own comment makes decisive - length distinct base-26 "spreadsheet column" names (a, b, ..., z, aa,
// ab, ...) joined by '+', every one of them valid per LC 770's lowercase-letters-only variable
// grammar however large the requested length grows.
public sealed partial class ColumnNameExpressionTests
{
    private const int SingleLetterAlphabetSize = 26;
    private const int ThirtyNames = 30;
    private const int TwoHundredNames = 200;

    // The whole rendering ThirtyNames' build produces, pinned so a wrong carry cannot hide behind a
    // correct join.
    private const string ThirtyNameExpression =
        "a+b+c+d+e+f+g+h+i+j+k+l+m+n+o+p+q+r+s+t+u+v+w+x+y+z+aa+ab+ac+ad";

    // LC 770's variable grammar: one or more lowercase letters, nothing else.
    private const string LowercaseNamePattern = "^[a-z]+$";

    // The name at index SingleLetterAlphabetSize - the first past the single-letter alphabet, where
    // the base-26 counter carries.
    private const string FirstTwoLetterName = "aa";

    [Fact]
    public void Build_ZeroLength_ReturnsAnEmptyExpression() =>
        Assert.Equal(string.Empty, ColumnNameExpression.Build(0));

    // The first 26 names are the alphabet itself; index 26 is where the base-26 counter carries, and
    // the constant pins the whole rendering so a wrong carry cannot hide behind a correct join.
    [Fact]
    public void Build_ThirtyNames_JoinsBase26NamesWithPlusInOrder() =>
        Assert.Equal(
            ThirtyNameExpression,
            ColumnNameExpression.Build(ThirtyNames));

    [Fact]
    public void Build_TwoHundredNames_EmitsOnlyDistinctLowercaseNames()
    {
        var names = ColumnNameExpression.Build(TwoHundredNames).Split('+');

        Assert.Equal(TwoHundredNames, names.Length);
        Assert.Equal(TwoHundredNames, names.Distinct().Count());
        Assert.All(names, name => Assert.Matches(LowercaseNamePattern, name));
    }

    // The carry is what index 26 exists to exercise, and the generator promises it keeps producing
    // valid names however large the requested length grows - so the last name past the single-letter
    // alphabet is the alphabet's first letter twice, not a digit or a punctuation mark.
    [Fact]
    public void Build_PastTheSingleLetterAlphabet_ContinuesWithTheTwoLetterHandful() =>
        Assert.Equal(
            FirstTwoLetterName,
            ColumnNameExpression.Build(SingleLetterAlphabetSize + 1).Split('+')[SingleLetterAlphabetSize]);
}
