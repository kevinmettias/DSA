namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1947 - how many student/mentor pairs to assign and
// how long each survey is, drawn from a fixed seed so every run measures the same
// answer sheets. What those answers MEAN, and the score table built from them, is the
// problem's own model (LeetCode.MaximumCompatibilityScoreSum's
// CompatibilityScoreMatrix); only the size and the seed are decided here.
internal static class CompatibilitySurveyWorkloads
{
    // LC 1947's answers are yes/no.
    private const int AnswerOptionCount = 2;

    public static (int[][] Students, int[][] Mentors) BuildAnswerSheets(
        int groupSize, int questionCount, int seed)
    {
        var random = new Random(seed);
        var students = new int[groupSize][];
        var mentors = new int[groupSize][];

        for (var i = 0; i < groupSize; i++)
        {
            students[i] = BuildAnswers(random, questionCount);
            mentors[i] = BuildAnswers(random, questionCount);
        }

        return (students, mentors);
    }

    private static int[] BuildAnswers(Random random, int questionCount)
    {
        var answers = new int[questionCount];

        for (var question = 0; question < questionCount; question++)
        {
            answers[question] = random.Next(0, AnswerOptionCount);
        }

        return answers;
    }
}
