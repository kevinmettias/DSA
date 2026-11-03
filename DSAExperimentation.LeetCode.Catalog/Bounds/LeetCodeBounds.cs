using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DSAExperimentation.LeetCode.Catalog.Bounds;

// The one constraint shape that can be read without understanding the question: a number, then
// one or more inputs, then a number, joined by < or <= - "2 <= nums.length <= 10^4",
// "1 <= m, n <= 200", "-2^31 <= x <= 2^31 - 1". About half of all constraint lines have it, and at
// least one line of almost every question does. A bound whose limit names another input
// ("1 <= k <= nums.length") or whose middle is a chain ("0 <= start <= end <= 10^4") is a rule
// about the question, not a range, and TryParse declines it rather than guess.
/// <summary>Reads the numeric ranges out of a question's constraint lines.</summary>
public static partial class LeetCodeBounds
{
    private const string InclusiveOperator = "<=";
    private const char SubjectSeparator = ',';
    private const string LowerGroup = "lower";
    private const string LowerOperatorGroup = "lowerOperator";
    private const string SubjectsGroup = "subjects";
    private const string UpperOperatorGroup = "upperOperator";
    private const string UpperGroup = "upper";

    /// <summary>Reads every two-sided numeric range out of a question's constraint lines, skipping the rest.</summary>
    /// <param name="constraints">The question's constraint lines, with powers written as ^.</param>
    /// <returns>The ranges, in the order their lines appear.</returns>
    public static IReadOnlyList<LeetCodeBound> Of(IReadOnlyList<string> constraints)
    {
        var bounds = new List<LeetCodeBound>();

        foreach (var constraint in constraints)
        {
            if (TryParse(constraint, out var bound))
            {
                bounds.Add(bound);
            }
        }

        return bounds;
    }

    /// <summary>Reads a bound out of one constraint line, if the line is a two-sided numeric range.</summary>
    /// <param name="constraint">One constraint line as the question states it, with powers written as ^.</param>
    /// <param name="bound">The bound the line states, when this returns true.</param>
    /// <returns>Whether the line is a two-sided numeric range.</returns>
    public static bool TryParse(string constraint, [NotNullWhen(true)] out LeetCodeBound? bound)
    {
        bound = null;
        var match = TwoSidedRange().Match(constraint);

        if (!match.Success || !TryReadLimits(match, out var lower, out var upper))
        {
            return false;
        }

        var subjects = SubjectsOf(match.Groups[SubjectsGroup].Value);

        if (subjects.Count == 0)
        {
            return false;
        }

        bound = BoundFrom(match, lower, upper, subjects);
        return true;
    }

    // A limit, an operator, the subjects, an operator, a limit. The limits and subjects exclude
    // <, > and =, which is what refuses a chain of three comparisons or an equality.
    [GeneratedRegex(@"^\s*(?<lower>[^<>=]+?)\s*(?<lowerOperator><=?)\s*(?<subjects>[^<>=]+?)\s*(?<upperOperator><=?)\s*(?<upper>[^<>=]+?)\s*\.?\s*$")]
    private static partial Regex TwoSidedRange();

    private static bool TryReadLimits(Match match, out double lower, out double upper)
    {
        upper = 0;
        return BoundExpression.TryEvaluate(match.Groups[LowerGroup].Value, out lower)
            && BoundExpression.TryEvaluate(match.Groups[UpperGroup].Value, out upper);
    }

    private static List<string> SubjectsOf(string subjects)
        => subjects
            .Split(SubjectSeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(subject => SubjectShape().IsMatch(subject))
            .ToList();

    // An input's name, then any run of .member and [index] accessors: m, nums[i], grid[i][j],
    // words[i].length. Anything else in the middle of a range is prose and refuses the bound.
    [GeneratedRegex(@"^[A-Za-z_]\w*(?:\.[A-Za-z_]\w*|\[\w+\])*$")]
    private static partial Regex SubjectShape();

    private static LeetCodeBound BoundFrom(Match match, double lower, double upper, List<string> subjects)
        => new(
            lower,
            match.Groups[LowerOperatorGroup].Value == InclusiveOperator,
            subjects,
            upper,
            match.Groups[UpperOperatorGroup].Value == InclusiveOperator);
}
