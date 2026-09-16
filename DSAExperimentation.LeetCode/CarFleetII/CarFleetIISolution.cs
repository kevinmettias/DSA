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

    private static double FirstReachableCollisionTime(int[][] cars, double[] answer, int carIndex)
    {
        for (var j = carIndex + 1; j < cars.Length; j++)
        {
            if (cars[carIndex][Speed] <= cars[j][Speed])
            {
                continue;
            }

            var collisionTime = CatchUpTime(cars, carIndex, j);

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

    // Resolves the collision of the car at carIndex against the current top-of-stack
    // candidate at candidateIndex. Returns true once answer[carIndex] is settled; false
    // means that candidate can never be the blocking car for this car (or anyone behind
    // it) and was popped for good, so the caller should keep peeking the next candidate.
    private static bool TryResolveCollision(CollisionSearchState state, int carIndex, int candidateIndex)
    {
        var (cars, answer, candidatesAhead) = state;

        if (cars[carIndex][Speed] <= cars[candidateIndex][Speed])
        {
            candidatesAhead.TryPop(out _);
            return false;
        }

        var collisionTime = CatchUpTime(cars, carIndex, candidateIndex);

        if (answer[candidateIndex] < 0 || collisionTime <= answer[candidateIndex])
        {
            answer[carIndex] = collisionTime;
            return true;
        }

        candidatesAhead.TryPop(out _);
        return false;
    }

    // When the car at carIndex would reach the candidate at candidateIndex if both held
    // their own speeds - only meaningful once the car at carIndex is known to be the
    // faster of the two.
    private static double CatchUpTime(int[][] cars, int carIndex, int candidateIndex) =>
        (double)(cars[candidateIndex][Position] - cars[carIndex][Position])
        / (cars[carIndex][Speed] - cars[candidateIndex][Speed]);

    private readonly record struct CollisionSearchState(int[][] Cars, double[] Answer, RepoIndexStack CandidatesAhead);
}
