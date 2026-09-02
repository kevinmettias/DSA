using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheLongestValidObstacleCourseAtEachPosition;

// LeetCode 1964. Find the Longest Valid Obstacle Course at Each Position: the same
// patience-sorting "tails" array LongestIncreasingSubsequenceTests builds via this
// repo's own BinarySearch.LowerBound over a DynamicArraySequence<int>, swapped for
// BinarySearch.UpperBound so equal heights extend a run instead of ending it (a
// "valid course" only requires non-decreasing height, not strictly increasing).
// The insertion position doubles as the answer for that index - it's exactly the
// length of the longest non-decreasing run ending there, for free.
public sealed partial class FindTheLongestValidObstacleCourseAtEachPositionTests
{
    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestObstacleCourseAtEachPosition_LeetCodeExamples_ReturnsExpectedLengths(
        int[] obstacles, int[] expected)
        => Assert.Equal(expected, LongestObstacleCourseAtEachPosition(obstacles));

    public static IEnumerable<object[]> Examples()
    {
        yield return [new[] { 1, 2, 3, 2 }, new[] { 1, 2, 3, 3 }];
        yield return [new[] { 2, 2, 1 }, new[] { 1, 2, 1 }];
        yield return [new[] { 3, 1, 5, 6, 4, 2 }, new[] { 1, 1, 2, 3, 2, 2 }];
    }

    private static int[] LongestObstacleCourseAtEachPosition(int[] obstacles)
    {
        var tails = new DynamicArray<int>();
        var answer = new int[obstacles.Length];

        for (var i = 0; i < obstacles.Length; i++)
        {
            var position = BinarySearch.UpperBound(new DynamicArraySequence<int>(tails), obstacles[i]);
            answer[i] = position + 1;

            if (position == tails.Count)
            {
                tails.Add(obstacles[i]);
            }
            else
            {
                tails.Set(position, obstacles[i]);
            }
        }

        return answer;
    }
}
