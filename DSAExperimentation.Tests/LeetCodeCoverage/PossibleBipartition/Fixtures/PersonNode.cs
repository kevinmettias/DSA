namespace DSAExperimentation.Tests.LeetCodeCoverage.PossibleBipartition.Fixtures;

internal sealed class PersonNode(int id)
{
    public int Id { get; } = id;

    public List<PersonNode> Dislikes { get; } = [];

    public override string ToString() => Id.ToString();
}
