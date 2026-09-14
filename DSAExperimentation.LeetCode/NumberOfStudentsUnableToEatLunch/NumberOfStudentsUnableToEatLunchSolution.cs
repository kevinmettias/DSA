using StudentQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;
using SandwichStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.NumberOfStudentsUnableToEatLunch;

// LeetCode 1700. Number of Students Unable to Eat Lunch: students queue in FIFO
// order, sandwiches sit in a LIFO stack (top = index 0). Each round the front
// student either takes the top sandwich, if the two preferences match, or goes to
// the back of the line. Report how many students are still waiting when the
// process stalls.
//
// Both strategies run that exact process, and both stop on the same condition -
// once a full lap of the current line produces no match, no remaining student can
// ever be served, because neither the top sandwich nor the multiset of waiting
// preferences can change again. They differ only in the containers the line and
// the stack of sandwiches are kept in, which is what decides whether sending a
// student to the back costs O(1) or O(n).
internal static class NumberOfStudentsUnableToEatLunchSolution
{
    // The textbook baseline this composition has to justify itself against: a
    // plain BCL List<int> as the line and an index into the sandwich array as the
    // stack. List.RemoveAt(0) shifts every remaining element, so each trip to the
    // back of the line is O(n) and the whole simulation is O(n^2).
    public static int CountStudentsByListSimulation(int[] students, int[] sandwiches)
    {
        var line = new List<int>(students);
        var sandwichIndex = 0;
        var consecutiveSkips = 0;

        while (line.Count > 0 && consecutiveSkips < line.Count)
        {
            var preference = line[0];
            line.RemoveAt(0);

            if (preference == sandwiches[sandwichIndex])
            {
                sandwichIndex++;
                consecutiveSkips = 0;
            }
            else
            {
                line.Add(preference);
                consecutiveSkips++;
            }
        }

        return line.Count;
    }

    // The composed answer: this repo's own Queue<int> for the line (Deque-backed,
    // so both ends are O(1)) and Stack<int> for the sandwiches, which models the
    // pile literally rather than as an index. The simulation is then O(n) overall.
    public static int CountStudentsByQueueStackSimulation(int[] students, int[] sandwiches)
    {
        var line = BuildQueue(students);
        var pile = BuildStack(sandwiches);

        return SimulateLunchLine(line, pile);
    }

    private static StudentQueue BuildQueue(int[] students)
    {
        var line = new StudentQueue();

        foreach (var student in students)
        {
            line.Enqueue(student);
        }

        return line;
    }

    // sandwiches[0] is the top of the pile, so the array is pushed back to front.
    private static SandwichStack BuildStack(int[] sandwiches)
    {
        var pile = new SandwichStack();

        for (var i = sandwiches.Length - 1; i >= 0; i--)
        {
            pile.Push(sandwiches[i]);
        }

        return pile;
    }

    // Runs rounds until either the line empties or a full lap of it produces no
    // match (consecutiveSkips reaches the line's own size), at which point no
    // remaining student can ever be served.
    private static int SimulateLunchLine(StudentQueue line, SandwichStack pile)
    {
        var consecutiveSkips = 0;

        while (line.Count > 0 && consecutiveSkips < line.Count)
        {
            line.TryDequeue(out var preference);
            pile.TryPeek(out var top);

            if (preference == top)
            {
                pile.TryPop(out _);
                consecutiveSkips = 0;
            }
            else
            {
                line.Enqueue(preference);
                consecutiveSkips++;
            }
        }

        return line.Count;
    }
}
