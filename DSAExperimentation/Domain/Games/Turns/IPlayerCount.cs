namespace DSAExperimentation.Domain.Games.Turns;

// How many players sit at a game whose turn order serves any number of them (round robin,
// simultaneous moves), as a static member of a struct, so the count is a constant in each order
// generic over it and its divisions fold.
/// <summary>A number of players, fixed for a game.</summary>
public interface IPlayerCount
{
    /// <summary>Gets how many players there are, at least 1.</summary>
    static abstract int Players { get; }
}
