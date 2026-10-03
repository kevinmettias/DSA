namespace DSAExperimentation.Tests.Algorithms.StringMatching.Fixtures;

// Ordinary character equality that counts how often it was asked. The linear-time
// string scans take their comparer as a parameter, so this is how a test sees the
// amount of work a scan did rather than only the answer it returned.
internal sealed class CountingCharComparer : IEqualityComparer<char>
{
    public int Comparisons { get; private set; }

    public bool Equals(char x, char y)
    {
        Comparisons++;
        return x == y;
    }

    public int GetHashCode(char obj) => obj.GetHashCode();
}
