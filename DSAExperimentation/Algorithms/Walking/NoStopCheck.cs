namespace DSAExperimentation.Algorithms.Walking;

// The check for an algebra that never stops: the walk runs to its last event and never asks.
internal readonly struct NoStopCheck : IStopCheck
{
    public static bool IsEnabled
        => false;
}
