namespace DSAExperimentation.Algorithms.Walking;

// The check for an algebra that can stop: the walk asks its IsDone after each event.
internal readonly struct AlgebraStopCheck : IStopCheck
{
    public static bool IsEnabled
        => true;
}
