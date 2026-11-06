using System.Text.Json;
using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.RandomPointInNonOverlappingRectangles;

// LC 497's pick() returns "a random integer point inside the space covered by one of
// the given rectangles", and "a point on the perimeter of a rectangle is included", so
// any integer [x, y] inside or on some rectangle [a, b, x, y] stands. The check replays
// the script: the constructor's rects, then each call's answer against them, null for
// the constructor itself. The recorded answer is one random draw among many, so it does
// not take part.
internal static class RandomPointInNonOverlappingRectanglesAnswerCheck
{
    private const string Pick = "pick";

    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        var isOneAnswerPerCall = answer.Count == calls.Length && answer[0] is null;

        if (!isOneAnswerPerCall || RectsOf(arguments) is not { } rects)
        {
            return false;
        }

        return Enumerable.Range(1, calls.Length - 1).All(call => calls[call] == Pick && IsInsideSome(rects, answer[call]));
    }

    // The constructor's one argument, rects.
    private static int[][]? RectsOf(JsonArray arguments)
    {
        if (arguments[0] is not (JsonArray and [JsonArray rects]))
        {
            return null;
        }

        return rects.Deserialize<int[][]>();
    }

    private static bool IsInsideSome(int[][] rects, JsonNode? answered)
    {
        if (PointOf(answered) is not { } point)
        {
            return false;
        }

        return rects.Any(rect => IsInside(point, rect));
    }

    // An answer of two integers, x then y; null for any other answer.
    private static (int X, int Y)? PointOf(JsonNode? answered)
    {
        if (answered is not (JsonArray and [JsonValue x, JsonValue y]))
        {
            return null;
        }

        if (!x.TryGetValue<int>(out var column) || !y.TryGetValue<int>(out var row))
        {
            return null;
        }

        return (column, row);
    }

    private static bool IsInside((int X, int Y) point, int[] rect)
    {
        if (rect is not [var left, var bottom, var right, var top])
        {
            return false;
        }

        var isWithinColumns = point.X >= left && point.X <= right;
        var isWithinRows = point.Y >= bottom && point.Y <= top;

        return isWithinColumns && isWithinRows;
    }
}
