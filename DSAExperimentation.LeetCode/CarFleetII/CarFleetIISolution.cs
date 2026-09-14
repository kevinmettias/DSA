using RepoIndexStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.CarFleetII;

// LeetCode 1776. Car Fleet II: cars[i] is [position, speed] on a single lane, given
// in strictly increasing position order. Report, for every car, the time at which it
// collides with the next car ahead of it - or -1 if it never does. A car that
// collides joins the fleet ahead and continues at that fleet's (slower) speed, so a
// candidate ahead is only reachable while it is still travelling at its own speed.
//
// Both strategies compute the same right-to-left recurrence: for car i, a candidate j
// ahead is usable only if i is strictly faster than j and i's naive catch-up time
// with j happens no later than j's own already-known collision. They differ only in
// how many candidates each car has to look at.
internal static class CarFleetIISolution
{
    // Position and speed's slots in LeetCode's own two-element car array.
    private const int Position = 0;
    private const int Speed = 1;

    // The textbook answer: for each car, scan forward over every car ahead until one
    // is found that it can actually reach. No memory is carried between cars, so a
    // candidate rejected by car i is examined again by car i-1 - O(n^2) worst case.
    // Deliberately plain arrays and loops; it is the arm the sweep below has to
    // justify itself against.
    public static double[] GetCollisionTimesByBruteForce(int[][] cars)
    {
        var answer = new double[cars.Length];

        for (var i = cars.Length - 1; i >= 0; i--)
        {
            answer[i] = FirstReachableCollisionTime(cars, answer, i);
        }

        return answer;
    }

    private static double FirstReachableCollisionTime(int[][] cars, double[] answer, int i)
    {
        for (var j = i + 1; j < cars.Length; j++)
        {
            if (cars[i][Speed] <= cars[j][Speed])
            {
                continue;
            }

            var collisionTime = CatchUpTime(cars, i, j);

            if (answer[j] < 0 || collisionTime <= answer[j])
            {
                return collisionTime;
            }
        }

        return LeetCodeAnswer.None;
    }

    // This repo's own Stack<int> (CarFleet/DailyTemperatures/AsteroidCollision
    // precedent for it over System.Collections.Generic.Stack) holding the indices of
    // the cars ahead that could still block someone. A candidate j on top is popped
    // for good - not merely skipped for this one car - whenever i can never catch it
    // (i is no faster) or whenever i's catch-up time with j falls after j has already
    // collided with something ahead of itself: in both cases j is unreachable for
    // every car further behind too, so discarding it is safe. One push and at most
    // one pop per car, so O(n) amortized.
    public static double[] GetCollisionTimesByMonotonicStack(int[][] cars)
    {
        var answer = new double[cars.Length];
        var candidatesAhead = new RepoIndexStack();
        var state = new CollisionSearchState(cars, answer, candidatesAhead);

        for (var i = cars.Length - 1; i >= 0; i--)
        {
            answer[i] = LeetCodeAnswer.None;

            while (candidatesAhead.TryPeek(out var j))
            {
                if (TryResolveCollision(state, i, j))
                {
                    break;
                }
            }

            candidatesAhead.Push(i);
        }

        return answer;
    }

    // Resolves car i's collision against the current top-of-stack candidate j.
    // Returns true once answer[i] is settled; false means j can never be the blocking
    // car for i (or anyone behind i) and was popped for good, so the caller should
    // keep peeking the next candidate.
    private static bool TryResolveCollision(CollisionSearchState state, int i, int j)
    {
        var (cars, answer, candidatesAhead) = state;

        if (cars[i][Speed] <= cars[j][Speed])
        {
            candidatesAhead.TryPop(out _);
            return false;
        }

        var collisionTime = CatchUpTime(cars, i, j);

        if (answer[j] < 0 || collisionTime <= answer[j])
        {
            answer[i] = collisionTime;
            return true;
        }

        candidatesAhead.TryPop(out _);
        return false;
    }

    // When car i would reach car j if both held their own speeds - only meaningful
    // once i is known to be the faster of the two.
    private static double CatchUpTime(int[][] cars, int i, int j) =>
        (double)(cars[j][Position] - cars[i][Position]) / (cars[i][Speed] - cars[j][Speed]);

    private readonly record struct CollisionSearchState(int[][] Cars, double[] Answer, RepoIndexStack CandidatesAhead);
}
