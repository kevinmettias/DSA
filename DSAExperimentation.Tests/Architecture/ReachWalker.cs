using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace DSAExperimentation.Tests.Architecture;

// Walks one body for BaselineTrace, telling it each member, type and creation the body binds to. Its
// site is the member whose body it is, which is where a library name it meets is charged.
internal sealed class ReachWalker(BaselineTrace trace, ISymbol site, ReachMode mode) : OperationWalker
{
    public override void Visit(IOperation? operation)
    {
        if (operation?.Type is { } type)
        {
            trace.TypeBound(type, site, mode);
        }

        base.Visit(operation);
    }

    public override void VisitInvocation(IInvocationOperation operation)
    {
        trace.Bound(operation.TargetMethod, site, mode);
        base.VisitInvocation(operation);
    }

    public override void VisitObjectCreation(IObjectCreationOperation operation)
    {
        if (operation.Constructor is { } constructor)
        {
            trace.Bound(constructor, site, mode);
        }

        if (operation.Type is { } type)
        {
            trace.Instantiated(type, site);
        }

        base.VisitObjectCreation(operation);
    }

    public override void VisitMethodReference(IMethodReferenceOperation operation)
    {
        trace.Bound(operation.Method, site, mode);
        base.VisitMethodReference(operation);
    }

    public override void VisitFieldReference(IFieldReferenceOperation operation)
    {
        MemberRead(operation.Field);
        base.VisitFieldReference(operation);
    }

    public override void VisitPropertyReference(IPropertyReferenceOperation operation)
    {
        MemberRead(operation.Property);
        base.VisitPropertyReference(operation);
    }

    public override void VisitEventReference(IEventReferenceOperation operation)
    {
        trace.Bound(operation.Event, site, mode);
        base.VisitEventReference(operation);
    }

    public override void VisitTypeOf(ITypeOfOperation operation)
    {
        trace.TypeBound(operation.TypeOperand, site, mode);
        base.VisitTypeOf(operation);
    }

    public override void VisitIsType(IIsTypeOperation operation)
    {
        trace.TypeBound(operation.TypeOperand, site, mode);
        base.VisitIsType(operation);
    }

    // A declared local's type, whether written out or inferred from `var`.
    public override void VisitVariableDeclarator(IVariableDeclaratorOperation operation)
    {
        trace.TypeBound(operation.Symbol.Type, site, mode);
        base.VisitVariableDeclarator(operation);
    }

    public override void VisitDeclarationPattern(IDeclarationPatternOperation operation)
    {
        if (operation.MatchedType is { } matched)
        {
            trace.TypeBound(matched, site, mode);
        }

        base.VisitDeclarationPattern(operation);
    }

    public override void VisitTypePattern(ITypePatternOperation operation)
    {
        trace.TypeBound(operation.MatchedType, site, mode);
        base.VisitTypePattern(operation);
    }

    public override void VisitRecursivePattern(IRecursivePatternOperation operation)
    {
        trace.TypeBound(operation.MatchedType, site, mode);
        base.VisitRecursivePattern(operation);
    }

    // A benchmark field is read through every value its class assigns it. A benchmark property may
    // compute its value too, so it is also traced as a member.
    private void MemberRead(ISymbol member)
    {
        if (RepositoryTiers.IsBenchmark(member.OriginalDefinition))
        {
            trace.BenchmarkMemberRead(member, site);

            if (member is IFieldSymbol)
            {
                return;
            }
        }

        trace.Bound(member, site, mode);
    }
}
