using DSAExperimentation.LeetCode.Catalog.Questions;

namespace DSAExperimentation.LeetCode.Catalog.Bounds;

// One range a question's constraints state, read out of a constraint line by LeetCodeBounds. It
// answers two questions: whether a value is in range, and whether a whole case is.
/// <summary>A numeric range a question's constraints place on one or more of its inputs.</summary>
/// <param name="Lower">The smallest value the range admits, or the value it starts just above.</param>
/// <param name="IsLowerInclusive">Whether <paramref name="Lower"/> itself is in range (&lt;=) or not (&lt;).</param>
/// <param name="Subjects">What the range constrains, as the statement writes it: "nums.length", "nums[i]", "m".</param>
/// <param name="Upper">The largest value the range admits, or the value it stops just below.</param>
/// <param name="IsUpperInclusive">Whether <paramref name="Upper"/> itself is in range (&lt;=) or not (&lt;).</param>
public sealed record LeetCodeBound(
    double Lower,
    bool IsLowerInclusive,
    IReadOnlyList<string> Subjects,
    double Upper,
    bool IsUpperInclusive)
{
    /// <summary>Judges one case's inputs against the range.</summary>
    /// <param name="testCase">The case whose inputs to judge.</param>
    /// <param name="parameters">The question's judge inputs, which name each subject's input.</param>
    /// <returns>
    /// Violated when some value is out of range; Satisfied when every subject was read and is in range;
    /// NotJudged when some subject could not be read from the inputs.
    /// </returns>
    public LeetCodeBoundCheck Check(LeetCodeCase testCase, IReadOnlyList<LeetCodeParameter> parameters)
    {
        var verdicts = new List<LeetCodeBoundCheck>(Subjects.Count);

        foreach (var subject in Subjects)
        {
            var values = BoundSubject.ValuesOf(subject, testCase, parameters);
            verdicts.Add(JudgeValues(values));
        }

        return CombinedVerdict(verdicts);
    }

    private LeetCodeBoundCheck JudgeValues(IReadOnlyList<double>? values)
    {
        if (values is null)
        {
            return LeetCodeBoundCheck.NotJudged;
        }

        foreach (var value in values)
        {
            if (!IsInRange(value))
            {
                return LeetCodeBoundCheck.Violated;
            }
        }

        return LeetCodeBoundCheck.Satisfied;
    }

    private static LeetCodeBoundCheck CombinedVerdict(List<LeetCodeBoundCheck> verdicts)
    {
        if (verdicts.Contains(LeetCodeBoundCheck.Violated))
        {
            return LeetCodeBoundCheck.Violated;
        }

        var isEverySubjectJudged = verdicts.TrueForAll(verdict => verdict == LeetCodeBoundCheck.Satisfied);
        return isEverySubjectJudged ? LeetCodeBoundCheck.Satisfied : LeetCodeBoundCheck.NotJudged;
    }

    /// <summary>Whether a value lies inside the range.</summary>
    /// <param name="value">The value to test.</param>
    /// <returns>Whether the value is within both limits.</returns>
    public bool IsInRange(double value)
    {
        var isAboveLower = IsLowerInclusive ? value >= Lower : value > Lower;
        var isBelowUpper = IsUpperInclusive ? value <= Upper : value < Upper;
        return isAboveLower && isBelowUpper;
    }
}
