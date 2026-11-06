namespace DSAExperimentation.Domain.Games.Turns;

/// <summary>Three players.</summary>
public readonly struct ThreePlayers : IPlayerCount
{
    // Internal, so that a solution's own small constants that share a word with it are not read as
    // copies of it (ARCHITECTURE 17.9); Players states it to everyone.
    internal const int Count = 3;

    /// <inheritdoc/>
    public static int Players => Count;
}
