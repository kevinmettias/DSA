namespace DSAExperimentation.LeetCode.MaximumCompatibilityScoreSum;

// The compatibility score of one student/mentor pair is how many survey questions the
// two answered identically. LC 1947's assignment search only ever reads that
// groupSize-by-groupSize table, never the raw answer sheets, so computing it once up
// front is the whole of this problem's input preparation - the same "Build, then
// search" split Domain.Locks' LockGraph.Build makes for LC 752.
//
// It lives beside the solution rather than in Domain/ because a student/mentor answer
// sheet fixes THIS problem's content and nothing else's (ARCHITECTURE §17.3), and it
// is deliberately not an IEnumerable: it is the parameter type of the prepared-input
// overloads (§17.4), so it must never be bindable by the int[][]-shaped overloads the
// LeetCode-shaped calls go through.
internal sealed class CompatibilityScoreMatrix
{
    private readonly int[][] _scores;

    private CompatibilityScoreMatrix(int[][] scores) => _scores = scores;

    // Square by construction: LC 1947 pairs every student with exactly one mentor.
    public int GroupSize => _scores.Length;

    public int Score(int student, int mentor) => _scores[student][mentor];

    public static CompatibilityScoreMatrix Build(int[][] students, int[][] mentors)
    {
        var groupSize = students.Length;
        var scores = new int[groupSize][];

        for (var student = 0; student < groupSize; student++)
        {
            scores[student] = BuildRow(students[student], mentors, groupSize);
        }

        return new CompatibilityScoreMatrix(scores);
    }

    private static int[] BuildRow(int[] studentAnswers, int[][] mentors, int groupSize)
    {
        var row = new int[groupSize];

        for (var mentor = 0; mentor < groupSize; mentor++)
        {
            row[mentor] = CountMatches(studentAnswers, mentors[mentor]);
        }

        return row;
    }

    private static int CountMatches(int[] studentAnswers, int[] mentorAnswers)
    {
        var matches = 0;

        for (var question = 0; question < studentAnswers.Length; question++)
        {
            if (studentAnswers[question] == mentorAnswers[question])
            {
                matches++;
            }
        }

        return matches;
    }
}
