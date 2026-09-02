using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;
using RepoStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfStudentsUnableToEatLunch;

// LeetCode 1700. Number of Students Unable to Eat Lunch: this repo's own
// Queue<int> (students, FIFO) and Stack<int> (sandwiches, top = index 0)
// simulate the textbook process directly - each round, if the front student's
// preference matches the top sandwich, both are consumed; otherwise the
// student cycles to the back of the queue. Once a full lap of the current
// queue produces no match, no remaining student can ever be served, so the
// loop stops as soon as consecutiveSkips reaches the queue's own size.
public sealed partial class NumberOfStudentsUnableToEatLunchTests
{
    [Fact]
    public void CountStudents_ClassicExample_EveryoneEventuallyEats()
    {
        int[] students = [1, 1, 0, 0];
        int[] sandwiches = [0, 1, 0, 1];

        var actual = CountStudents(students, sandwiches);
        Assert.Equal(0, actual);
    }

    [Fact]
    public void CountStudents_NoStudentWantsTheAvailableSandwich_ReturnsAllRemaining()
    {
        int[] students = [1, 1, 1];
        int[] sandwiches = [0, 0, 0];

        var actual = CountStudents(students, sandwiches);
        Assert.Equal(3, actual);
    }

    private static int CountStudents(int[] students, int[] sandwiches)
    {
        var queue = BuildQueue(students);
        var stack = BuildStack(sandwiches);

        Simulate(queue, stack);

        return queue.Count;
    }

    private static RepoQueue BuildQueue(int[] students)
    {
        var queue = new RepoQueue();
        foreach (var student in students)
        {
            queue.Enqueue(student);
        }

        return queue;
    }

    private static RepoStack BuildStack(int[] sandwiches)
    {
        var stack = new RepoStack();
        for (var i = sandwiches.Length - 1; i >= 0; i--)
        {
            stack.Push(sandwiches[i]);
        }

        return stack;
    }

    // Runs rounds until either the queue empties or a full lap of the current
    // queue produces no match (consecutiveSkips reaches the queue's own size),
    // at which point no remaining student can ever be served.
    private static void Simulate(RepoQueue queue, RepoStack stack)
    {
        var consecutiveSkips = 0;

        while (queue.Count > 0 && consecutiveSkips < queue.Count)
        {
            queue.TryDequeue(out var preference);
            stack.TryPeek(out var top);

            if (preference == top)
            {
                stack.TryPop(out _);
                consecutiveSkips = 0;
            }
            else
            {
                queue.Enqueue(preference);
                consecutiveSkips++;
            }
        }
    }
}
