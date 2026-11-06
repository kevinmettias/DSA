using System.Text.Json.Nodes;

namespace DSAExperimentation.LeetCode.Solutions.GenerateRandomPointInACircle;

// LC 478's randPoint() returns "a random point inside the circle", and "a point on
// the circumference of the circle is considered to be in the circle", so any [x, y]
// no farther from the center than the radius stands, within the 10^-5 LeetCode
// allows its real-valued answers. The check replays the script: the constructor's
// radius and center, then each call's answer against them, null for the constructor
// itself. The recorded answer is one random draw among many, so it does not take part.
internal static class GenerateRandomPointInACircleAnswerCheck
{
    private const string RandPoint = "randPoint";
    private const double Tolerance = 1e-5;

    public static bool IsAccepted(string[] calls, JsonArray arguments, JsonArray answer, JsonArray recorded)
    {
        var isOneAnswerPerCall = answer.Count == calls.Length && answer[0] is null;

        if (!isOneAnswerPerCall || CircleOf(arguments) is not { } circle)
        {
            return false;
        }

        return Enumerable.Range(1, calls.Length - 1).All(call => calls[call] == RandPoint && IsWithin(circle, answer[call]));
    }

    // The constructor's arguments: the radius, then the center's x and y.
    private static Circle? CircleOf(JsonArray arguments)
    {
        if (arguments[0] is not (JsonArray and [JsonValue radius, JsonValue x, JsonValue y]))
        {
            return null;
        }

        var reach = radius.GetValue<double>();
        var centerX = x.GetValue<double>();
        var centerY = y.GetValue<double>();

        return new Circle(reach, centerX, centerY);
    }

    private static bool IsWithin(Circle circle, JsonNode? answered)
    {
        if (PointOf(answered) is not { } point)
        {
            return false;
        }

        var dx = point.X - circle.X;
        var dy = point.Y - circle.Y;
        var reach = circle.Radius + Tolerance;

        return (dx * dx) + (dy * dy) <= reach * reach;
    }

    // An answer of two numbers, x then y; null for any other answer.
    private static (double X, double Y)? PointOf(JsonNode? answered)
    {
        if (answered is not (JsonArray and [JsonValue x, JsonValue y]))
        {
            return null;
        }

        if (!x.TryGetValue<double>(out var across) || !y.TryGetValue<double>(out var up))
        {
            return null;
        }

        return (across, up);
    }

    // The circle a script's constructor describes.
    private readonly record struct Circle(double Radius, double X, double Y);
}
