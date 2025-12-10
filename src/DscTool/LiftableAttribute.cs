namespace DscTool;

[AttributeUsage(AttributeTargets.GenericParameter, Inherited = false, AllowMultiple = true)]
public sealed class LiftableAttribute : Attribute
{
    public string MemberPath {get;}
    public Type? UnionType {get;}
    public string? UnionTypeName {get;}
    public MemberPathResolveStrategy MemberPathResolveStrategy {get;init;} = MemberPathResolveStrategy.None;
    
    public LiftableAttribute(string memberPath, Type unionType)
    {
        MemberPath = memberPath;
        UnionType = unionType;
    }

    public LiftableAttribute(string memberPath, string unionTypeName)
    {
        MemberPath = memberPath;
        UnionTypeName = unionTypeName;
    }
}
