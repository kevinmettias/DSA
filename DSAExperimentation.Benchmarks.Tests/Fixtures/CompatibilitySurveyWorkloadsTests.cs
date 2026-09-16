using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for CompatibilitySurveyWorkloads (ARCHITECTURE 17.7): a square set of student
// and mentor answer sheets, every answer a yes/no, all drawn from one seeded sequence so the
// chosen pairing is scored against the same surveys on every run.
public sealed partial class CompatibilitySurveyWorkloadsTests
{
    private const int GroupSize = 8;
    private const int QuestionCount = 5;
    private const int Seed = 1947; // LC problem number
    private const int MinAnswer = 0;
    private const int MaxAnswer = 1;

    [Fact]
    public void BuildAnswerSheets_GroupSize_ReturnsOneSheetPerStudentAndPerMentor()
    {
        var (students, mentors) = CompatibilitySurveyWorkloads.BuildAnswerSheets(GroupSize, QuestionCount, Seed);

        Assert.Equal(GroupSize, students.Length);
        Assert.Equal(GroupSize, mentors.Length);
        Assert.All(students, sheet => Assert.Equal(QuestionCount, sheet.Length));
        Assert.All(mentors, sheet => Assert.Equal(QuestionCount, sheet.Length));
    }

    [Fact]
    public void BuildAnswerSheets_EveryAnswer_IsOneOfTheTwoSurveyOptions()
    {
        var (students, mentors) = CompatibilitySurveyWorkloads.BuildAnswerSheets(GroupSize, QuestionCount, Seed);

        Assert.All(students, sheet => Assert.All(sheet, answer => Assert.InRange(answer, MinAnswer, MaxAnswer)));
        Assert.All(mentors, sheet => Assert.All(sheet, answer => Assert.InRange(answer, MinAnswer, MaxAnswer)));
    }

    [Fact]
    public void BuildAnswerSheets_SameSeed_ReturnsTheSameSheets()
    {
        var (students, mentors) = CompatibilitySurveyWorkloads.BuildAnswerSheets(GroupSize, QuestionCount, Seed);
        var (repeatStudents, repeatMentors) =
            CompatibilitySurveyWorkloads.BuildAnswerSheets(GroupSize, QuestionCount, Seed);

        Assert.Equal(AnswerText.Of(students), AnswerText.Of(repeatStudents));
        Assert.Equal(AnswerText.Of(mentors), AnswerText.Of(repeatMentors));
    }
}
