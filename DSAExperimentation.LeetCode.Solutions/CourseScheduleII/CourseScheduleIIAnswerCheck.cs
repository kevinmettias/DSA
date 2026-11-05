namespace DSAExperimentation.LeetCode.Solutions.CourseScheduleII;

// LC 210 accepts any order that takes each course from 0 to numCourses - 1 once, every course after its
// prerequisites ("If there are many valid answers, return any of them"), and the empty array only when no
// such order exists. The catalog records one order, so an answer stands when it is empty exactly where the
// recorded one is, or else such an order. Kahn's sort takes ready courses in the order they became ready and
// the naive rescan takes the lowest-numbered one, so each breaks ties between ready courses its own way, and
// neither always as the catalog did.
internal static class CourseScheduleIIAnswerCheck
{
    public static bool IsAccepted(int numCourses, int[][] prerequisites, int[] answer, int[] recorded)
    {
        if (answer.Length == 0 || recorded.Length == 0)
        {
            return answer.Length == recorded.Length;
        }

        if (PositionsOf(numCourses, answer) is not { } positions)
        {
            return false;
        }

        // A prerequisite [a, b] asks for course b before course a.
        return prerequisites.All(prerequisite => positions[prerequisite[1]] < positions[prerequisite[0]]);
    }

    // Where each course stands in the order, indexed by the course; null unless the order takes each course
    // from 0 to numCourses - 1 exactly once.
    private static int[]? PositionsOf(int numCourses, int[] order)
    {
        var everyCourse = Enumerable.Range(0, numCourses);

        if (!order.Order().SequenceEqual(everyCourse))
        {
            return null;
        }

        var positions = new int[numCourses];

        for (var position = 0; position < order.Length; position++)
        {
            positions[order[position]] = position;
        }

        return positions;
    }
}
