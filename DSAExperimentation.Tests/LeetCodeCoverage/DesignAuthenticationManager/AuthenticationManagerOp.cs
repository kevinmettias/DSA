using DSAExperimentation.LeetCode.DesignAuthenticationManager;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAuthenticationManager;

// One call in an AuthenticationManager script: which operation to invoke, on which
// token, at what time. Pure dispatch, built via the named factories below so a
// script reads like the LeetCode call sequence it replays.
public readonly record struct AuthenticationManagerOp(
    AuthenticationManagerOp.OpKind kind, string tokenId, int currentTime)
{
    public static AuthenticationManagerOp Generate(string tokenId, int currentTime) =>
        new(OpKind.Generate, tokenId, currentTime);

    public static AuthenticationManagerOp Renew(string tokenId, int currentTime) =>
        new(OpKind.Renew, tokenId, currentTime);

    public static AuthenticationManagerOp CountUnexpiredTokens(int currentTime) =>
        new(OpKind.Count, string.Empty, currentTime);

    // null for the two void operations, matching LeetCode's own judge output, so a
    // script runner can assert against one expected value per operation uniformly.
    internal int? Apply(DesignAuthenticationManagerSolution.IAuthenticationManager manager)
    {
        switch (kind)
        {
            case OpKind.Generate:
                manager.Generate(tokenId, currentTime);
                return null;
            case OpKind.Renew:
                manager.Renew(tokenId, currentTime);
                return null;
            default:
                return manager.CountUnexpiredTokens(currentTime);
        }
    }

    public enum OpKind
    {
        Generate,
        Renew,
        Count,
    }
}
