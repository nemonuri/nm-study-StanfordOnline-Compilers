namespace Nemonuri.LowLevel.Primitives.DotNet;

public class RuntimeTypeHandleEqualityComparer() : IEqualityComparer<RuntimeTypeHandle>
{
    public static RuntimeTypeHandleEqualityComparer Instance {get;} = new();

    public bool Equals(RuntimeTypeHandle x, RuntimeTypeHandle y) => x.Equals(y);

    public int GetHashCode(RuntimeTypeHandle obj) => obj.GetHashCode();
}

public class RuntimeFieldHandleEqualityComparer() : IEqualityComparer<RuntimeFieldHandle>
{
    public static RuntimeFieldHandleEqualityComparer Instance {get;} = new();

    public bool Equals(RuntimeFieldHandle x, RuntimeFieldHandle y) => x.Equals(y);

    public int GetHashCode(RuntimeFieldHandle obj) => obj.GetHashCode();
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct RuntimeFieldAndTypeHandle : IEquatable<RuntimeFieldAndTypeHandle>
{
    // '제네릭' 때문에, 필드와 타입 정보가 다 있어야, 정확한 'Field info' 를 얻을 수 있다.

    public readonly RuntimeFieldHandle RuntimeFieldHandle;

    public readonly RuntimeTypeHandle DeclaringTypeHandle;

    public RuntimeFieldAndTypeHandle(RuntimeFieldHandle runtimeFieldHandle, RuntimeTypeHandle declaringTypeHandle)
    {
        RuntimeFieldHandle = runtimeFieldHandle;
        DeclaringTypeHandle = declaringTypeHandle;
    }

    public bool Equals(RuntimeFieldAndTypeHandle other) => 
        RuntimeFieldHandleEqualityComparer.Instance.Equals(RuntimeFieldHandle, other.RuntimeFieldHandle) &&
        RuntimeTypeHandleEqualityComparer.Instance.Equals(DeclaringTypeHandle, other.DeclaringTypeHandle);
    
    public override bool Equals(object obj) => obj is RuntimeFieldAndTypeHandle v && Equals(v);
    
    public override int GetHashCode()
    {
        unchecked
        {
            return
                RuntimeFieldHandleEqualityComparer.Instance.GetHashCode(RuntimeFieldHandle) +
                (RuntimeTypeHandleEqualityComparer.Instance.GetHashCode(DeclaringTypeHandle) * 3079);
        }
    }

    public System.Reflection.FieldInfo DotNetFieldInfo => System.Reflection.FieldInfo.GetFieldFromHandle(RuntimeFieldHandle, DeclaringTypeHandle);
}